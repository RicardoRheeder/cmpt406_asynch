using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;

    //Main Menu panels
    private GameObject pendingGamesPanel;
    private GameObject activeGamesPanel;
    private GameObject createGamePanel;
    private GameObject joinGamePanel;

    private void Awake() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
        pendingGamesPanel = GameObject.Find("PendingGamesPanel");
        activeGamesPanel = GameObject.Find("ActiveGamesPanel");
        createGamePanel = GameObject.Find("CreateGamePanel");
        joinGamePanel = GameObject.Find("JoinGamePanel");
    }

    // Start is called before the first frame update
    void Start() {
        SetInitialMenuState();
    }

    //Helper function to enable/disable menus with boolean flags
    private void SetMenuState(bool pendingState, bool activeState, bool createState, bool joinState) {
        pendingGamesPanel.SetActive(pendingState);
        activeGamesPanel.SetActive(activeState);
        createGamePanel.SetActive(createState);
        joinGamePanel.SetActive(joinState);
    }

    //Helper function so that other buttons can easily return to this state when they are done in their sub menu
    public void SetInitialMenuState() {
        SetMenuState(false, false, false, false);
    }

    public void MainMenuJoinGameButton() {
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
        SetMenuState(false, false, true, false);
    }

    public void MainMenuActiveGamesButton() {
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
        SetMenuState(true, false, false, false);
        Tuple<bool, GameStateCollection> response = networkApi.GetPendingGamesInformation();
        if (response.First) {
            foreach(var state in response.Second.states) {
                Debug.Log(state);
            }
            //we can deal with displaying the game states
        }
        else {
            //the request failed, inform the user
        }
    }

    public void MainMenuAddFriendButton() {
        //Here we need to somehow get the string of the username we would like to add
        string userToAdd = "1";
        if (!networkApi.AddFriend(userToAdd)) {
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
