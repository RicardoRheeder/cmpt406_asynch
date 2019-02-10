﻿//NOTE: THIS IS STILL A DRAFT

//This class will have to ahndle the logic of the following:
//  providing an API that other places within the game can send/receive messages from the server
//  be able to handle server disconnects/reconnects
//  Allow to connect to the "mock" of the server
using System.Collections.Generic;
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
    private PlayerMetadata userInformation;

    //Server information
    private const string URL = "https://async-406.appspot.com";
    private const string CREATE_USER = "/CreateUser"; //Used for creating a new user
    private const string GET_USER_INFO = "/GetUser"; //Used for logging in
    private const string GET_GAME_STATE_MULTI = "/GetGameStateMulti"; //Used to get the pending gamestates
    private const string ADD_FRIEND = "/AddFriend"; //Used to add a friend
    private const string REMOVE_FRIEND = "/RemoveFriend"; //Used to remove a friend
    private const string CREATE_PUBLIC_GAME = "/CreatePublicGame"; //Used to create a public game
    private const string CREATE_PRIVATE_GAME = "/CreatePrivateGame"; //Used to create a private game
    private const string GET_GAME_STATE = "/GetGameState"; //Used to get the state of a game

    //Networking constants
    private const string JSON_TYPE = "application/json";

    //Make sure this object is not destroyed on scene transitions
    public void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    //Sends new user information to the server
    public bool CreateUser(string username, string password) {
        user = new Credentials(username, password);
        if (debugMode) {
            //Interface with the mock server instead, should be used for testing only
        }
        else {
            HttpWebRequest request = CreatePostRequestWithoutAuth(CREATE_USER);
            string requestJson = JsonConversion.ConvertObjectToJson<Credentials>(typeof(Credentials), user);
            AddJsonToRequest(requestJson, ref request);

            //We might want to do more logic here depending on what the various status codes we have mean
            //This will come in place later once more functionality is in place
            try {
                request.GetResponse();
            }
            catch (WebException e) {
                PrettyPrint(CREATE_USER, (HttpWebResponse)e.Response);
                return false;
            }
            return true;
        }
        return false;
    }

    //Logs the specified user in
    //This function should cache the username within the client so that we can use it in future functions
    public bool LoginUser(string username, string password) {
        if (debugMode) {
            //Interface with the mock server instead, should be used for testing only
        }
        else {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + GET_USER_INFO);
            request.Method = "GET";
            request.Headers["Authorization"] = "Basic " + System.Convert.ToBase64String(Encoding.Default.GetBytes( username + ":" + password));

            //We might want to do more logic here depending on what the various status codes we have mean
            //This will come in place later once more functionality is in place
            try {
                var response = (HttpWebResponse)request.GetResponse();
                string responseJson;
                using (var reader = new StreamReader(response.GetResponseStream())) {
                    responseJson = reader.ReadToEnd();
                }
                userInformation = JsonConversion.CreateFromJson<PlayerMetadata>(responseJson, typeof(PlayerMetadata));
                user = new Credentials(username, password);
                return true;
            }
            catch (WebException e) {
                PrettyPrint(GET_USER_INFO, (HttpWebResponse)e.Response);
                return false;
            }
        }
        return false;
    }

    //Make sure any cached is cleared so that another user can login
    public void LogoutUser() {
        user = null;
        userInformation = null;
    }

    //This method is used to get the summary of the games that are considered pending for the logged in user
    public Tuple<bool, GameStateCollection> GetPendingGamesInformation() {
        if (debugMode) {
            //Interface with the mock server instead, should be used only for testing purposes
        }
        else {
            if ((userInformation.pendingPrivateGames != null && userInformation.pendingPrivateGames.Count > 0) ||
                (userInformation.pendingPublicGames != null && userInformation.pendingPublicGames.Count > 0)) {
                return GetGameStateCollectionHelper(new GameIds(userInformation.pendingPublicGames, userInformation.pendingPrivateGames));
            }
        }
        return new Tuple<bool, GameStateCollection>(false, null);
    }

    public Tuple<bool, GameStateCollection> GetActiveGamesInformation() {
        if (debugMode) {
            //Interface with the mock server instead, should be used only for testing purposes
        }
        else {
            if (userInformation.activeGames.Count > 0) {
                return GetGameStateCollectionHelper(new GameIds(userInformation.activeGames));
            }
        }
        return new Tuple<bool, GameStateCollection>(false, null);
    }

    public Tuple<bool, GameStateCollection> GetCompletedGamesInformation() {
        if (debugMode) {
            //Interface with the mock server instead, should be used only for testing purposes
        }
        else {
            if (userInformation.completedGames.Count > 0) {
                return GetGameStateCollectionHelper(new GameIds(userInformation.completedGames));
            }
        }
        return new Tuple<bool, GameStateCollection>(false, null);
    }

    private Tuple<bool, GameStateCollection> GetGameStateCollectionHelper(GameIds ids) {
        HttpWebRequest request = CreatePostRequest(GET_GAME_STATE_MULTI);
        string requestJson = JsonConversion.ConvertObjectToJson<GameIds>(typeof(GameIds), ids);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            string responseJson;
            using (var reader = new StreamReader(response.GetResponseStream())) {
                responseJson = reader.ReadToEnd();
            }
            GameStateCollection states = JsonConversion.CreateFromJson<GameStateCollection>(responseJson, typeof(GameStateCollection));
            return new Tuple<bool, GameStateCollection>(true, states);
        }
        catch (WebException e) {
            PrettyPrint(GET_GAME_STATE_MULTI, (HttpWebResponse)e.Response);
            return new Tuple<bool, GameStateCollection>(false, null);
        }
    }

    public bool AddFriend(string userToAdd) {
        if (debugMode) {
            //Interface with the mock server instead, should be used only for testing purposes
        }
        else {
            return FriendHelper(userToAdd, ADD_FRIEND);
        }
        return false;
    }

    public bool RemoveFriend(string userToAdd) {
        if (debugMode) {
            //Interface with the mock server instead, should be used only for testing purposes
        }
        else {
            return FriendHelper(userToAdd, REMOVE_FRIEND);
        }
        return false;
    }

    private bool FriendHelper(string targetUser, string endpoint) {
        HttpWebRequest request = CreatePostRequest(endpoint);
        string requestJson = JsonConversion.GetJsonForSingleField("username", targetUser);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            Debug.Log("friend request sent!");
            return true;
        }
        catch (WebException e) {
            PrettyPrint(endpoint, (HttpWebResponse)e.Response);
            return false;
        }
    }

    public Tuple<bool, GameState> GetGamestate(string id) {
        if(debugMode) {
            //Interface with the mock server instead, should be used only for testing purposes
        }
        else {
            HttpWebRequest request = CreatePostRequest(GET_GAME_STATE);
            string requestJson = JsonConversion.GetJsonForSingleField("gameId", id);
            AddJsonToRequest(requestJson, ref request);

            try {
                var response = (HttpWebResponse)request.GetResponse();
                string responseJson;
                using (var reader = new StreamReader(response.GetResponseStream())) {
                    responseJson = reader.ReadToEnd();
                }
                GameState state = JsonConversion.CreateFromJson<GameState>(responseJson, typeof(GameState));
                return new Tuple<bool, GameState>(true, state);
            }
            catch (WebException e) {
                PrettyPrint(GET_GAME_STATE, (HttpWebResponse)e.Response);
                return new Tuple<bool, GameState>(false, null);
            }
        }
        return new Tuple<bool, GameState>(false, null);
    }

    public bool CreatePrivateGame(string name, int turnTime, int forfeitTime, List<string> opponents, int boardId) {
        CreatePrivateGameState state = new CreatePrivateGameState(name, turnTime, forfeitTime, opponents, boardId);
        if(debugMode) {

        }
        else {
            //Setting up the request object
            HttpWebRequest request = CreatePostRequest(CREATE_PRIVATE_GAME);
            string requestJson = JsonConversion.ConvertObjectToJson<CreatePrivateGameState>(typeof(CreatePrivateGameState), state);
            AddJsonToRequest(requestJson, ref request);

            try {
                var response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException e) {
                PrettyPrint(CREATE_PRIVATE_GAME, (HttpWebResponse)e.Response);
                return false;
            }
        }
        return false;
    }

    //Helper methods for the API
    private HttpWebRequest CreatePostRequest(string endpoint) {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + endpoint);
        request.Method = "POST";
        request.Headers["Authorization"] = "Basic " + System.Convert.ToBase64String(Encoding.Default.GetBytes(user.username + ":" + user.password));
        request.ContentType = JSON_TYPE;
        return request;
    }

    private HttpWebRequest CreatePostRequestWithoutAuth(string endpoint) {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + endpoint);
        request.Method = "POST";
        request.ContentType = JSON_TYPE;
        return request;
    }

    private void AddJsonToRequest(string json, ref HttpWebRequest request) {
        Debug.Log("Adding Json to request: " + json);
        byte[] bytes = Encoding.ASCII.GetBytes(json);
        Stream requestData = request.GetRequestStream();
        requestData.Write(bytes, 0, bytes.Length);
    }

    private void PrettyPrint(string endpoint, HttpWebResponse response) {
        string responseJson;
        using (var reader = new StreamReader(response.GetResponseStream())) {
            responseJson = reader.ReadToEnd();
        }
        Debug.Log(string.Format("Call to endpoint {0} failed: {1} {2}; {3}",
            endpoint,
            (int)response.StatusCode,
            response.StatusCode,
            responseJson
        ));
    }
}
