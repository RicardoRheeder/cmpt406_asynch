﻿//NOTE: THIS IS STILL A DRAFT

//This class will have to ahndle the logic of the following:
//  providing an API that other places within the game can send/receive messages from the server
//  be able to handle server disconnects/reconnects
//  Allow to connect to the "mock" of the server
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

//This class has a monobehaviour attached so that the "DebugMode" can be easily toggled on and off without going into the code
public class Client : MonoBehaviour {

    [SerializeField]
    private bool debugMode = false;

    //Client cache
    private Credentials user;

    //Server information
    private const string URL = "https://async-406.appspot.com";
    private const string CREATE_USER = "/CreateUser"; //Used for creating a new user
    private const string GET_USER_INFO = "/GetUser"; //Used for logging in

    //Networking constants
    private const string JSON_TYPE = "application/json";
    
    //Make sure this object is not destroyed on scene transitions
    public void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    //Sends a message to the server to get the requested game state
    //If we are using mock, it communicates with the static mock object
    //Request type: GET
    //Authorization: username/password required
    //URL parameters: gameId
    //Request Body: none
    //Return: a tuple containing the success of the query and the gamestate
    public Tuple<bool, GameState> GetGameState(string gameId) {
        if(debugMode) {

        }
        else {

        }
        return new Tuple<bool, GameState>(false, null);
    }

    //Sends a the updated game state to the server
    //Hopefull we can design this in such a way that we only have to send the difference, not the entire game state
    //If we are using the mock server, it has to be set up to handle it
    //Request type: POST
    //Authorization: username/password required
    //URL parameters: game ID
    //Request Body: JSON of game state
    //Return: true if the request succeeded, false otherwise
    public bool SendGameState(GameState currentState, string gameId) {
        if (debugMode) {

        }
        else {

        }
        return false;
    }

    //Sends new user information to the server
    //Request type: POST
    //Authorization: none
    //URL parameters: none
    //Request Body: JSON of username and password
    //Return: true if the request succeeded, false otherwise
    public bool CreateUser(string username, string password) {
        user = new Credentials(username, password);
        if (debugMode) {

        }
        else {
            string json = JsonConversion.ConvertObjectToJson(typeof(Credentials), (System.Object) user);
            byte[] bytes = Encoding.ASCII.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + CREATE_USER);
            request.ContentType = JSON_TYPE;
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestData = request.GetRequestStream();
            requestData.Write(bytes, 0, bytes.Length);

            //We might want to do more logic here depending on what the various status codes we have mean
            //This will come in place later once more functionality is in place
            return ((HttpWebResponse)request.GetResponse()).StatusCode ==  HttpStatusCode.OK;
        }
        return false;
    }

    //Logs the specified user in
    //This function should cache the username within the client so that we can use it in future functions
    //Request type: GET
    //Authorization: none
    //Request Body: JSON of username and password
    //Return: true if the user exists, false otherwise
    public bool LoginUser(string username, string password) {
        if (debugMode) {
        }
        else {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + GET_USER_INFO);
            request.Method = "GET";
            request.Headers["Authorization"] = "Basic " + System.Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

            //We might want to do more logic here depending on what the various status codes we have mean
            //This will come in place later once more functionality is in place
            //We also might need to cache the response in order to populate the next page
            if( ((HttpWebResponse)request.GetResponse()).StatusCode == HttpStatusCode.OK) {
                user = new Credentials(username, password);
                return true;
            }
        }
        return false;
    }

    //Make sure the cache is cleared so that another user can login
    public bool LogoutUser() {
        user = null;
        return true;
    }

    //Get a list of all available games that the user is a part of from the server
    //This list should include:
    //  Games that it's currently the users turn in
    //  Games that the user is a part of but it's not their turn
    //  Games that are still waiting to start
    //Request type: GET
    //Authorization: username/password required
    //Request Body: JSON of username and password
    //Return: the requested game information attached to the user
    public string GetUserInformation(string username) {
        if (debugMode) {
            
        }
        else {

        }
        return "";
    }

    //A method to send a game request to the specified user
    //Request type: GET
    //Authorization: username/password required
    //Request Body: JSON of user that is being invited, and the id of the game they are being invited to
    //Return: the requested game information attached to the user
    public bool SendGameRequest(string gameId, string userToInvite) {
        if (debugMode) {

        }
        else {

        }
        return false;
    }

    //A method to tell the server we are accepting the game invite
    //Request type: GET
    //Authorization: username/password required
    //Request Body: JSON of the id of the game that is being joined
    //Return: the requested game information attached to the user
    public bool AcceptGameInvite(string gameId) {
        if (debugMode) {

        }
        else {

        }
        return false;
    }
}
