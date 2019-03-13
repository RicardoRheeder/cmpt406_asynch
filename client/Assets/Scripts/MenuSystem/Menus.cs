using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;
    private AudioManager audioManager;

    public void Start() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void StartScreenStartButton() {
        audioManager.Play("ButtonPress");
        SceneManager.LoadScene("LoginScreen");
    }

    public void LoginScreenLoginButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;

        //The user entered a username or password that is invalid
        if (!StringValidation.ValidateUsernamePassword(username, password)) {
            //prompt this on the ui, and do nothing
            Debug.Log("username or password is invalid");
            audioManager.Play("ButtonError");
            return;
        }

        if (!networkApi.LoginUser(username, password, encryptPassword:true)) {
            //For some reason login failed, we have to figure out what to do here
            audioManager.Play("ButtonError");
            return;
        }
        audioManager.Play("ButtonPress");
        SceneManager.LoadScene("MainMenu");
    }

    public void LoginScreenCreateUserButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;
        
        //The user entered a username or password that is invalid
        if(!StringValidation.ValidateUsernamePassword(username, password)) {
            //prompt this on the ui, and do nothing
            Debug.Log("username or password is invalid");
            audioManager.Play("ButtonError");
            return;
        }

        //Something went wrong when creating a username with this information
        //We need to figure out the reason and inform the user
        if (!networkApi.CreateUser(username, password)) {
            Debug.Log("Logging in failed, likely because of an invalid username or password");
            audioManager.Play("ButtonError");
            return;
        }
        audioManager.Play("ButtonPress");
        SceneManager.LoadScene("MainMenu");
    }

    public void LoginScreenQuitButton() {
        audioManager.Play("ButtonQuit");
        Application.Quit();
    }
}
