//NOTE: THIS IS STILL A DRAFT

//This class will have to ahndle the logic of the following:
//  providing an API that other places within the game can send/receive messages from the server
//  be able to handle server disconnects/reconnects
//  Allow to connect to the "mock" of the server
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

//This class has a monobehaviour attached so that the "DebugMode" can be easily toggled on and off without going into the code
public class Client : MonoBehaviour, INetwork {

    //Event System to enable/disable event system
    GameObject eventSystem;

    //Client cache
    private Credentials user;
    public PlayerMetadata UserInformation { get; private set; }

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
    private const string GET_PUBLIC_GAMES = "/GetPublicGamesSummary"; //Used to get a number of public games
    private const string ADD_ARMY_PRESET = "/AddArmyPreset"; //Used to import army presets
    private const string REMOVE_ARMY_PRESET = "/RemoveArmyPreset"; //Used to delete existing presets
    private const string ACCEPT_GAME = "/AcceptGame";
    private const string DECLINE_GAME = "/DeclineGame";
    private const string BACKOUT_GAME = "/BackOutGame";
    private const string FORFEIT_GAME = "/ForfeitGame";
    private const string READY_UNITS = "/ReadyUnits";
    private const string MAKE_MOVE = "/MakeMove";
    private const string GET_ALL_COMPLETED_GAMES = "/GetCompletedGames";

    //Networking constants
    private const string JSON_TYPE = "application/json";



    //Make sure this object is not destroyed on scene transitions
    public void Start() {
        DontDestroyOnLoad(this.gameObject);
        eventSystem = GameObject.Find("EventSystem");
        SceneManager.sceneLoaded += FindEventSystem;
    }
    
    public PlayerMetadata GetUserInformation(){
        return this.UserInformation;
    }
    
    private void FindEventSystem(Scene scene, LoadSceneMode mode) {
        eventSystem = GameObject.Find("EventSystem");
    }

    private void BeginRequest() {
        if (eventSystem != null) {
            eventSystem.SetActive(false);
            return;
        }
        Debug.LogError("Event System is null");
    }

    private void EndRequest() {
        if (eventSystem != null) {
            eventSystem.SetActive(true);
            return;
        }
        Debug.LogError("Event System is null");
    }

    //Requests
    //Sends new user information to the server
    public bool CreateUser(string username, string password) {
        BeginRequest();
        user = new Credentials(username, password);
        HttpWebRequest request = CreatePostRequestWithoutAuth(CREATE_USER);
        string requestJson = JsonConversion.ConvertObjectToJson<Credentials>(user);
        AddJsonToRequest(requestJson, ref request);

        //We might want to do more logic here depending on what the various status codes we have mean
        //This will come in place later once more functionality is in place
        try {
            request.GetResponse();
            UserInformation = new PlayerMetadata {
                Username = username
            };
        }
        catch (WebException e) {
            user = null;
            PrettyPrint(CREATE_USER, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
        EndRequest();
        return true;
    }

    //Logs the specified user in
    //This function should cache the username within the client so that we can use it in future functions
    public bool LoginUser(string username, string password) {
        BeginRequest();
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
            UserInformation = JsonConversion.CreateFromJson<PlayerMetadata>(responseJson, typeof(PlayerMetadata));
            UserInformation.Username = username;
            user = new Credentials(username, password);
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(GET_USER_INFO, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    //Make sure any cached is cleared so that another user can login
    public void LogoutUser() {
        user = null;
        UserInformation = null;
        ArmyBuilder.Clear();
    }

    //This method is used to get the summary of the games that are considered pending for the logged in user
    public Tuple<bool, GameStateCollection> GetPendingGamesInformation() {
        if(this.LoginUser(user.username, user.password)) {
            if ((UserInformation.PendingPrivateGames != null && UserInformation.PendingPrivateGames.Count > 0) ||
                (UserInformation.PendingPublicGames != null && UserInformation.PendingPublicGames.Count > 0)) {
                return GetGameStateCollectionHelper(new GameIds(UserInformation.PendingPublicGames, UserInformation.PendingPrivateGames));
            }
        }
        else {
            //shit went bad
        }
        return new Tuple<bool, GameStateCollection>(false, null);
    }

    public Tuple<bool, GameStateCollection> GetActiveGamesInformation() {
        if(LoginUser(user.username, user.password)) {
            if (UserInformation.ActiveGames.Count > 0) {
                return GetGameStateCollectionHelper(new GameIds(UserInformation.ActiveGames));
            }
        }
        else {
            //shit went bad
        }
        return new Tuple<bool, GameStateCollection>(false, null);
    }

    public Tuple<bool, GameStateCollection> GetCompletedGamesInformation() {
        if (LoginUser(user.username, user.password)) {
            if (UserInformation.CompletedGames.Count > 0) {
                return GetGameStateCollectionHelper(new GameIds(UserInformation.CompletedGames));
            }
        }
        else {
            // bad shit happened
        }
        return new Tuple<bool, GameStateCollection>(false, null);
    }

    private Tuple<bool, GameStateCollection> GetGameStateCollectionHelper(GameIds ids) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(GET_GAME_STATE_MULTI);
        string requestJson = JsonConversion.ConvertObjectToJson<GameIds>(ids);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            string responseJson;
            using (var reader = new StreamReader(response.GetResponseStream())) {
                responseJson = reader.ReadToEnd();
            }
            GameStateCollection states = JsonConversion.CreateFromJson<GameStateCollection>(responseJson, typeof(GameStateCollection));
            EndRequest();
            return new Tuple<bool, GameStateCollection>(true, states);
        }
        catch (WebException e) {
            PrettyPrint(GET_GAME_STATE_MULTI, (HttpWebResponse)e.Response);
            EndRequest();
            return new Tuple<bool, GameStateCollection>(false, null);
        }
    }

    public Tuple<bool, GameStateCollection> GetPublicGames() {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(GET_PUBLIC_GAMES);
        string requestJson = JsonConversion.GetJsonForSingleInt("limit", 100);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            string responseJson;
            using (var reader = new StreamReader(response.GetResponseStream())) {
                responseJson = reader.ReadToEnd();
            }
            GameStateCollection states = JsonConversion.CreateFromJson<GameStateCollection>(responseJson, typeof(GameStateCollection));
            EndRequest();
            return new Tuple<bool, GameStateCollection>(true, states);
        }
        catch (WebException e) {
            PrettyPrint(GET_PUBLIC_GAMES, (HttpWebResponse)e.Response);
            EndRequest();
            return new Tuple<bool, GameStateCollection>(false, null);
        }
    }

    public Tuple<bool, GameStateCollection> GetAllCompletedGames() {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(GET_ALL_COMPLETED_GAMES);
        string requestJson = JsonConversion.GetJsonForSingleInt("limit", 1000);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            string responseJson;
            using (var reader = new StreamReader(response.GetResponseStream())) {
                responseJson = reader.ReadToEnd();
            }
            GameStateCollection states = JsonConversion.CreateFromJson<GameStateCollection>(responseJson, typeof(GameStateCollection));

            EndRequest();
            return new Tuple<bool, GameStateCollection>(true, states);
        }
        catch (WebException e) {
            PrettyPrint(GET_ALL_COMPLETED_GAMES, (HttpWebResponse)e.Response);
            EndRequest();
            return new Tuple<bool, GameStateCollection>(false, null);
        }
    }

    public bool AddFriend(string userToAdd) {
        return FriendHelper(userToAdd, ADD_FRIEND);
    }

    public bool RemoveFriend(string userToAdd) {
        return FriendHelper(userToAdd, REMOVE_FRIEND);
    }

    private bool FriendHelper(string targetUser, string endpoint) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(endpoint);
        string requestJson = JsonConversion.GetJsonForSingleField("username", targetUser);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(endpoint, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public Tuple<bool, GameState> GetGamestate(string id) {
        BeginRequest();
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
            EndRequest();
            return new Tuple<bool, GameState>(true, state);
        }
        catch (WebException e) {
            PrettyPrint(GET_GAME_STATE, (HttpWebResponse)e.Response);
            EndRequest();
            return new Tuple<bool, GameState>(false, null);
        }
    }

    public bool CreatePrivateGame(string name, int forfeitTime, List<string> opponents, int boardId) {
        BeginRequest();
        CreatePrivateGameState state = new CreatePrivateGameState(name, forfeitTime, opponents, boardId);
        //Setting up the request object
        HttpWebRequest request = CreatePostRequest(CREATE_PRIVATE_GAME);
        string requestJson = JsonConversion.ConvertObjectToJson<CreatePrivateGameState>(state);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(CREATE_PRIVATE_GAME, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public bool CreatePublicGame(string name, int forfeitTime, int maxPlayers, int boardId) {
        BeginRequest();
        CreatePublicGameState state = new CreatePublicGameState(name, forfeitTime, maxPlayers, boardId);
        //Setting up the request object
        HttpWebRequest request = CreatePostRequest(CREATE_PUBLIC_GAME);
        string requestJson = JsonConversion.ConvertObjectToJson<CreatePublicGameState>(state);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(CREATE_PRIVATE_GAME, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public bool RegisterArmyPreset(ArmyPreset preset) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(ADD_ARMY_PRESET);
        string requestJson = JsonConversion.ConvertObjectToJson<ArmyPreset>(preset);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(ADD_ARMY_PRESET, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public bool RemoveArmyPreset(string presetId) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(REMOVE_ARMY_PRESET);
        string requestJson = JsonConversion.GetJsonForSingleField("armyPresetId", presetId);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(REMOVE_ARMY_PRESET, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public bool AcceptGame(string gameId) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(ACCEPT_GAME);
        string requestJson = JsonConversion.GetJsonForSingleField("gameId", gameId);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(ACCEPT_GAME, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public bool DeclineGame(string gameId) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(DECLINE_GAME);
        string requestJson = JsonConversion.GetJsonForSingleField("gameId", gameId);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(DECLINE_GAME, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public bool BackOutGame(string gameId) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(BACKOUT_GAME);
        string requestJson = JsonConversion.GetJsonForSingleField("gameId", gameId);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(BACKOUT_GAME, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public bool ForfeitGame(string gameId) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(FORFEIT_GAME);
        string requestJson = JsonConversion.GetJsonForSingleField("gameId", gameId);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(FORFEIT_GAME, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public bool ReadyUnits(ReadyUnitsGameState state) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(READY_UNITS);
        string requestJson = JsonConversion.ConvertObjectToJson<ReadyUnitsGameState>(state);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(READY_UNITS, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
    }

    public bool EndTurn(EndTurnState state) {
        BeginRequest();
        HttpWebRequest request = CreatePostRequest(MAKE_MOVE);
        string requestJson = JsonConversion.ConvertObjectToJson<EndTurnState>(state);
        AddJsonToRequest(requestJson, ref request);

        try {
            var response = (HttpWebResponse)request.GetResponse();
            EndRequest();
            return true;
        }
        catch (WebException e) {
            PrettyPrint(MAKE_MOVE, (HttpWebResponse)e.Response);
            EndRequest();
            return false;
        }
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
