using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;

    public void Start() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
    }

    public void StartScreenStartButton() {
        FindObjectOfType<AudioManager>().Play("ButtonPress");
        SceneManager.LoadScene("LoginScreen");
    }

    public void LoginScreenLoginButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;

        //The user entered a username or password that is invalid
        if (!StringValidation.ValidateUsernamePassword(username, password)) {
            //prompt this on the ui, and do nothing
            Debug.Log("username or password is invalid");
            FindObjectOfType<AudioManager>().Play("ButtonError");
            return;
        }

        if (!networkApi.LoginUser(username, password)) {
            //For some reason login failed, we have to figure out what to do here
            FindObjectOfType<AudioManager>().Play("ButtonError");
            return;
        }
        FindObjectOfType<AudioManager>().Play("ButtonPress");
        SceneManager.LoadScene("MainMenu");
    }

    public void LoginScreenCreateUserButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;
        
        //The user entered a username or password that is invalid
        if(!StringValidation.ValidateUsernamePassword(username, password)) {
            //prompt this on the ui, and do nothing
            Debug.Log("username or password is invalid");
            FindObjectOfType<AudioManager>().Play("ButtonError");
            return;
        }

        //Something went wrong when creating a username with this information
        //We need to figure out the reason and inform the user
        if (!networkApi.CreateUser(username, password)) {
            Debug.Log("Logging in failed, likely because of an invalid username or password");
            FindObjectOfType<AudioManager>().Play("ButtonError");
            return;
        }
        FindObjectOfType<AudioManager>().Play("ButtonPress");
        SceneManager.LoadScene("MainMenu");
    }

    public void LoginScreenQuitButton() {
        FindObjectOfType<AudioManager>().Play("ButtonQuit");
        Application.Quit();
    }
}
