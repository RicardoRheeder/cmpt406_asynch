//NOTE: THIS IS STILL A DRAFT

//This class will have to ahndle the logic of the following:
//  providing an API that other places within the game can send/receive messages from the server
//  be able to handle server disconnects/reconnects
//  Allow to connect to the "mock" of the 
public static class Client {

    private static bool triedConnection = false;
    private static bool usingMock = false;


    //Sends a message to the server to get the requested game state
    //If we are using mock, it communicates with the static mock object
    //Request type: GET
    //Authorization: username/password required
    //URL parameters: gameId
    //Request Body: none
    //Return: a tuple containing the success of the query and the gamestate
    public static Tuple<bool, GameState> GetGameState(string gameId) {
        if(!triedConnection) {
            TryConnectToServer();
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
    public static bool SendGameState(GameState currentState, string gameId) {
        if (!triedConnection) {
            TryConnectToServer();
        }
        return false;
    }

    //Sends new user information to the server
    //Request type: POST
    //Authorization: none
    //URL parameters: none
    //Request Body: JSON of username and password
    //Return: true if the request succeeded, false otherwise
    public static bool CreateUser(string username, string password) {
        if (!triedConnection) {
            TryConnectToServer();
        }
        return false;
    }

    //Logs the specified user in
    //This function should cache the username within the client so that we can use it in future functions
    //Request type: GET
    //Authorization: none
    //Request Body: JSON of username and password
    //Return: true if the user exists, false otherwise
    public static bool LoginUser(string username, string password) {
        if (!triedConnection) {
            TryConnectToServer();
        }
        return false;
    }

    //Make sure the cache is cleared so that another user can login
    //Request type: GET
    //Authorization: none
    //Request Body: JSON of username
    //Return: true if the user exists, false otherwise
    public static bool LogoutUser() {
        if (!triedConnection) {
            TryConnectToServer();
        }
        return false;
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
    public static string GetUserInformation(string username) {
        if (!triedConnection) {
            TryConnectToServer();
        }
        return "";
    }

    //A method to send a game request to the specified user
    //Request type: GET
    //Authorization: username/password required
    //Request Body: JSON of user that is being invited, and the id of the game they are being invited to
    //Return: the requested game information attached to the user
    public static bool SendGameRequest(string gameId, string userToInvite) {
        if (!triedConnection) {
            TryConnectToServer();
        }
        return false;
    }

    //A method to tell the server we are accepting the game invite
    //Request type: GET
    //Authorization: username/password required
    //Request Body: JSON of the id of the game that is being joined
    //Return: the requested game information attached to the user
    public static bool AcceptGameInvite(string gameId) {
        if (!triedConnection) {
            TryConnectToServer();
        }
        return false;
    }

    //A method used to try initial server connection, or set up to use the mock if we are in debug mode.
    private static void TryConnectToServer() {
        //if we are in debug mode, set usingMock to true and triedConnection to false
        //else, try to establish communication with the server
        //If we can connect to the server, set tried connection to true and using mock to false
        //If we cannot connect to the server, set tried connection to true and using mock to true
        triedConnection = true;
        usingMock = true;
    }
}
