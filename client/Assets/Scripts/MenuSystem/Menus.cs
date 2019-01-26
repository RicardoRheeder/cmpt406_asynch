using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menus : MonoBehaviour {

    public Client networkApi;

    public void Start() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
    }

    public void StartScreenStartButton() {
        SceneManager.LoadScene("LoginScreen");
    }

    public void LoginScreenLoginButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;
        if(!networkApi.LoginUser(username, password)) {
            //For some reason login failed, we have to figure out what to do here
            Debug.Log("Creating an account failed, the username must already exist");
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void LoginScreenCreateUserButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;
        
        if (!validateUsernamePassword(username, password) || !networkApi.CreateUser(username, password)) {
            //For some reason login failed, this is likely due to an invalid username or password, so we should display something
            Debug.Log("Logging in failed, likely because of an invalid username or password");
        }
        SceneManager.LoadScene("MainMenu");
    }

    //Make sure the username/password are only strings we consider valid
    private bool validateUsernamePassword(string username, string password) {
        return true;
    }

    public void LoginScreenQuitButton() {
        Application.Quit();
    }

    public void MainMenuAddFriendButton() {

    }

    public void MainMenuRemoveUserButton() {

    }

    public void MainMenuJoinGameButton() {

    }

    public void MainMenuCreateGameButton() {

    }

    public void MainMenuActiveGamesButton() {

    }

    public void MainMenuGameInvitesButton() {

    }

    public void MainMenuLogoutButton() {
        if(networkApi.LogoutUser()) {
            SceneManager.LoadScene("LoginScreen");
        }
        else {
            //We failed to logout for some reason, this should never happen.
        }
    }
}
