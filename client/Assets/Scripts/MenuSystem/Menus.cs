using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menus : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;

    //Variables used 
    private readonly string validationPattern = "^[a-zA-Z0-9_-]*$";
    private readonly int validationLowerLimit = 4;
    private readonly int validationUpperLimit = 20;

    public void Start() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
    }

    public void StartScreenStartButton() {
        SceneManager.LoadScene("LoginScreen");
    }

    public void LoginScreenLoginButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;

        //The user entered a username or password that is invalid
        if (!ValidateUsernamePassword(username, password)) {
            //prompt this on the ui, and do nothing
            Debug.Log("username or password is invalid");
            return;
        }

        if (!networkApi.LoginUser(username, password)) {
            //For some reason login failed, we have to figure out what to do here
            Debug.Log("Creating an account failed, the username must already exist");
            return;
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void LoginScreenCreateUserButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;
        
        //The user entered a username or password that is invalid
        if(!ValidateUsernamePassword(username, password)) {
            //prompt this on the ui, and do nothing
            Debug.Log("username or password is invalid");
            return;
        }

        //Something went wrong when creating a username with this information
        //We need to figure out the reason and inform the user
        if (!networkApi.CreateUser(username, password)) {
            Debug.Log("Logging in failed, likely because of an invalid username or password");
            return;
        }
        SceneManager.LoadScene("MainMenu");
    }

    //Make sure the username/password are only strings we consider valid
    private bool ValidateUsernamePassword(string username, string password) {
        return ValidateString(username) && ValidateString(password);
    }

    private bool ValidateString(string s) {
        if( s.Length > validationLowerLimit && s.Length <= validationUpperLimit) {
            return Regex.Match(s, validationPattern).Success;
        }
        return false;
    }

    public void LoginScreenQuitButton() {
        Application.Quit();
    }

    //Variables used specifically for the main menu
    private bool menuItemsCached = false;
    private GameObject pendingGamesPanel;
    private GameObject activeGamesPanel;
    private GameObject createGamePanel;
    private GameObject joinGamePanel;

    //Helper method to load the game objects once
    //should only be called the first time a main menu button is pressed
    private void CacheMenuItems() {
        menuItemsCached = true;
        pendingGamesPanel = GameObject.Find("pendingGamesPanel");
        activeGamesPanel = GameObject.Find("activeGamesPanel");
        createGamePanel = GameObject.Find("createGamePanel");
        joinGamePanel = GameObject.Find("joinGamePanel");
    }

    //Helper function to enable/disable menus with boolean flags
    private void SetMenuState(bool pendingState, bool activeState, bool createState, bool joinState) {
        pendingGamesPanel.SetActive(pendingState);
        activeGamesPanel.SetActive(activeState);
        createGamePanel.SetActive(createState);
        joinGamePanel.SetActive(joinState);
    }

    public void MainMenuJoinGameButton() {
        if (!menuItemsCached) {
            CacheMenuItems();
        }
        SetMenuState(false, false, false, true);
        Tuple<bool, GameState> response = networkApi.GetGamestate("3ed32d75-8d7c-45cc-bd2f-c79f98634172"); //Hardcoded for testing purposes
        if (response.First) {
            Debug.Log(JsonConversion.ConvertObjectToJson(typeof(GameState), response.Second));
        }
        else {
            Debug.Log("getting the gamestate failed");
            //the request failed, inform the user
        }
    }

    public void MainMenuCreateGameButton() {
        if (!menuItemsCached) {
            CacheMenuItems();
        }
        SetMenuState(false, false, true, false);
    }

    public void MainMenuActiveGamesButton() {
        if (!menuItemsCached) {
            CacheMenuItems();
        }
        SetMenuState(false, true, false, false);
        Tuple<bool, GameStateCollection> response = networkApi.GetActiveGamesInformation();
        if (response.First) {
            //we can deal with displaying the game states
        }
        else {
            //the request failed, inform the user
        }
    }

    public void MainMenuPendingGamesButton() {
        if (!menuItemsCached) {
            CacheMenuItems();
        }
        SetMenuState(true, false, false, false);
        Tuple<bool, GameStateCollection> response = networkApi.GetPendingGamesInformation();
        if(response.First) {
            //we can deal with displaying the game states
        }
        else {
            //the request failed, inform the user
        }
    }

    public void MainMenuAddFriendButton() {
        //Here we need to somehow get the string of the username we would like to add
        string userToAdd = "1";
        if(!networkApi.AddFriend(userToAdd)) {
            //adding a user failed
        }
    }

    public void MainMenuRemoveUserButton() {
        //Here we need to somehow get the string of the username we would like to add
        string userToRemove = "1";
        if (!networkApi.RemoveFriend(userToRemove)) {
            //removing a user failed
        }
    }

    public void MainMenuLogoutButton() {
        networkApi.LogoutUser();
        SceneManager.LoadScene("LoginScreen");
    }
}
