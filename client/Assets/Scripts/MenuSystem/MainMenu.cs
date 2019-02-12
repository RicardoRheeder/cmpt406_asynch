using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;

    //Main Menu panels
    private GameObject pendingGamesPanel;
    private GameObject activeGamesPanel;
    private GameObject createGamePanel;
    private GameObject joinGamePanel;

    //Variables and prefabs to populate friends list
    [SerializeField]
    private GameObject friendsListCell;
    private GameObject friendsViewContent;
    TMP_InputField friendsListInputField;
    public Dictionary<string, GameObject> friendsListDict;

    //Variables and prefabs to populate game lists
    private GameObject gameListCell;
    private GameObject joinGameViewContent;
    private GameObject activeGamesViewContent;
    private GameObject pendingGamesViewContent;
    private GameObject mapName;
    private GameObject playersNum;
    private GameObject maxPlayersNum;
    private GameObject turnNum;
    public Dictionary<string, GameState> gamesListCellDict;

    private void Awake() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
        pendingGamesPanel = GameObject.Find("PendingGamesPanel");
        activeGamesPanel = GameObject.Find("ActiveGamesPanel");
        createGamePanel = GameObject.Find("CreateGamePanel");
        joinGamePanel = GameObject.Find("JoinGamePanel");
        friendsViewContent = GameObject.Find("FriendsListViewport");
        friendsListInputField = GameObject.Find("FriendsInputField").GetComponent<TMP_InputField>();
        friendsListDict = new Dictionary<string, GameObject> {};
        mapName = GameObject.Find("ThisMap");
        playersNum = GameObject.Find("ThisPlayersNum");
        maxPlayersNum = GameObject.Find("ThisMaxPlayersNum");
        turnNum = GameObject.Find("ThisTurnNum");
        joinGameViewContent = GameObject.Find("JoinPublicGameViewport");
        activeGamesViewContent = GameObject.Find("ActiveGamesViewport");
        pendingGamesViewContent = GameObject.Find("PendingGamesViewport");
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

    public void GameCellDetailsButton(){
        GameObject GameToDisplay;//grab the game from the details button.
        string map;
        string players;
        string maxPlayers;
        string turn;
        //set the text of the info panel elements (This******)

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
        string gameId;
        if (response.First) {
            foreach(var state in response.Second.states) {
                //we can deal with displaying the game states
                GameObject newGameCell = Instantiate(gameListCell);
                newGameCell.transform.SetParent(pendingGamesViewContent.transform, false);
                gameId = state.id;
                gamesListCellDict.Add(gameId, state);

            }
        }
        else {
            //the request failed, inform the user
        }
    }

    public void MainMenuAddFriendButton() {
        //Here we need to somehow get the string of the username we would like to add
        string userToAdd = friendsListInputField.text;
        if (StringValidation.ValidateUsername(userToAdd) && networkApi.AddFriend(userToAdd)) {
            GameObject friendText = Instantiate(friendsListCell);
            friendText.GetComponent<TMP_Text>().text = userToAdd;
            friendText.transform.SetParent(friendsViewContent.transform, false);
            friendsListDict.Add(userToAdd, friendText);
        }
        else {
            //adding a user failed
        }
    }

    public void MainMenuRemoveUserButton() {
        //Here we need to somehow get the string of the username we would like to add
        string userToRemove = friendsListInputField.text;
        if (networkApi.RemoveFriend(userToRemove)) {
            Destroy(friendsListDict[userToRemove]);
            friendsListDict.Remove(userToRemove);
        }

    }

    public void MainMenuLogoutButton() {
        networkApi.LogoutUser();
        SceneManager.LoadScene("LoginScreen");
    }
}
