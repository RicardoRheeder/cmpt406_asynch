using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 649
public class Menus : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;
    private AudioManager audioManager;

    //Error Message object
    [SerializeField]
    private GameObject errorMessagePanel;
    [SerializeField]
    private TMP_Text errorMessageText;

    public void Start() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void StartScreenStartButton() {
        audioManager.Play(SoundName.ButtonPress);
        SceneManager.LoadScene("LoginScreen");
    }

    public void LoginScreenLoginButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;

        //The user entered a username or password that is invalid
        if (!StringValidation.ValidateUsernamePassword(username, password)) {
            DisplayUserError("Login failed, invalid username or password.");
            audioManager.Play(SoundName.ButtonError);
            return;
        }

        if (!networkApi.LoginUser(username, password, encryptPassword:true)) {
            //For some reason login failed, we have to figure out what to do here
            DisplayUserError("Networking error, couldn't login.\nCheck your internet connection.");
            audioManager.Play(SoundName.ButtonError);
            return;
        }
        audioManager.Play(SoundName.ButtonPress);
        SceneManager.LoadScene("MainMenu");
    }

    public void LoginScreenCreateUserButton() {
        string username = GameObject.Find("UsernameField").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;

        //The user entered a username or password that is invalid
        if(!StringValidation.ValidateUsernamePassword(username, password)) {
            DisplayUserError("Create user failed, invalid username or password.\nUsernames must be between " + StringValidation.CREDENTIALS_LOWER_LIMIT + " and " + StringValidation.CREDENTIALS_UPPER_LIMIT + " characters and contain no special characters or spaces.\nIf the username is valid, it may already be taken.");
            audioManager.Play(SoundName.ButtonError);
            return;
        }

        //Something went wrong when creating a username with this information
        //We need to figure out the reason and inform the user
        if (!networkApi.CreateUser(username, password)) {
            DisplayUserError("Networking error, couldn't create an account.\nCheck your internet connection.");
            audioManager.Play(SoundName.ButtonError);
            return;
        }
        audioManager.Play(SoundName.ButtonPress);
        SceneManager.LoadScene("MainMenu");
    }

    public void LoginScreenQuitButton() {
        audioManager.Play(SoundName.ButtonQuit);
        Application.Quit();
    }

    private void DisplayUserError(string message) {
        errorMessageText.SetText(message);
        errorMessagePanel.SetActive(true);
    }
}
