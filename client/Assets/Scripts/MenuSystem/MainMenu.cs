using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;

    //Main Menu panels
    private GameObject mainMenuContainer;
    private GameObject pendingGamesPanel;
    private GameObject activeGamesPanel;
    private GameObject createGamePanel;
    private GameObject joinGamePanel;
    private GameObject armyBuilderPanel;
    private GameObject armySelectorPanel;

    //Variables and prefabs to populate friends list
    [SerializeField]
    private GameObject friendsListCellPrefab;
    private GameObject friendsViewContent;
    TMP_InputField friendsListInputField;
    public Dictionary<string, GameObject> friendsListDict;

    //Variables and prefabs to populate game lists
    [SerializeField]
    private GameObject gameListCellPrefab;
    private GameObject joinGameViewContent;
    private GameObject activeGamesViewContent;
    private GameObject pendingGamesViewContent;

    //Variables for the Pending games display
    private TMP_Text pendingMapName;
    private TMP_Text pendingCurrentPlayers;
    private TMP_Text pendingMaxPlayers;
    private TMP_Text pendingTurnNumber;
    private Button pendingJoinButton;

    //Variables for the active games  display
    private TMP_Text activeMapName;
    private TMP_Text activeCurrentPlayers;
    private TMP_Text activeMaxPlayers;
    private TMP_Text activeTurnNumber;
    private Button activeJoinButton;

    //Variables for the army selector display
    private GameObject armyChooserViewport;

    //Reference to the game manager to start a game
    private GameManager manager;
    
    private void Awake() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();

        mainMenuContainer = GameObject.Find("MainMenuContainer");
        pendingGamesPanel = GameObject.Find("PendingGamesPanel");
        activeGamesPanel = GameObject.Find("ActiveGamesPanel");
        createGamePanel = GameObject.Find("CreateGamePanel");
        joinGamePanel = GameObject.Find("JoinGamePanel");
        armyBuilderPanel = GameObject.Find("ArmyBuilder");
        armySelectorPanel = GameObject.Find("ArmySelector");

        friendsListInputField = GameObject.Find("FriendsInputField").GetComponent<TMP_InputField>();
        friendsListDict = new Dictionary<string, GameObject> {};
        joinGameViewContent = GameObject.Find("JoinPublicGameViewport");
        activeGamesViewContent = GameObject.Find("ActiveGamesViewport");
        pendingGamesViewContent = GameObject.Find("PendingGamesViewport");
        friendsViewContent = GameObject.Find("FriendsListViewport");

        pendingMapName = GameObject.Find("PendingGameMap").GetComponent<TMP_Text>();
        pendingCurrentPlayers = GameObject.Find("PendingNumberPlayers").GetComponent<TMP_Text>();
        pendingMaxPlayers = GameObject.Find("PendingMaxPlayers").GetComponent<TMP_Text>();
        pendingTurnNumber = GameObject.Find("PendingTurnNumber").GetComponent<TMP_Text>();
        pendingJoinButton = GameObject.Find("PendingJoinButton").GetComponent<Button>();

        activeMapName = GameObject.Find("ActiveGameMap").GetComponent<TMP_Text>();
        activeCurrentPlayers = GameObject.Find("ActiveNumberPlayers").GetComponent<TMP_Text>();
        activeMaxPlayers = GameObject.Find("ActiveMaxPlayers").GetComponent<TMP_Text>();
        activeTurnNumber = GameObject.Find("ActiveTurnNumber").GetComponent<TMP_Text>();
        activeJoinButton = GameObject.Find("ActiveJoinButton").GetComponent<Button>();

        armyChooserViewport = GameObject.Find("ArmyChooserViewport");

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start() {
        SetInitialMenuState();

        List<string> userFriends = networkApi.UserInformation.Friends;
        foreach(string friend in userFriends) {
            AddFriendHelper(friend);
        }
    }

    //Helper function so that other buttons can easily return to this state when they are done in their sub menu
    public void SetInitialMenuState() {
        SetMenuState(false, false, false, false, false, false);
    }

    //Helper function to enable/disable menus with boolean flags
    private void SetMenuState(bool pendingState, bool activeState, bool createState, bool joinState, bool builderState, bool selectorState) {
        pendingGamesPanel.SetActive(pendingState);
        activeGamesPanel.SetActive(activeState);
        createGamePanel.SetActive(createState);
        joinGamePanel.SetActive(joinState);
        armyBuilderPanel.SetActive(builderState);
        armySelectorPanel.SetActive(selectorState);
    }

    public void PendingGameCellDetailsButton(GameState state, bool needToAccept) {
        //Set up the display information
        pendingMapName.SetText(BoardMetadata.BoardDisplayNames[state.boardId]);
        pendingCurrentPlayers.SetText("" + (state.maxUsers - state.spotsAvailable));
        pendingMaxPlayers.SetText("" + state.maxUsers);
        pendingTurnNumber.SetText("" + state.TurnNumber);

        //Set up the join button to call the join function with the current state
        pendingJoinButton.onClick.RemoveAllListeners();
        pendingJoinButton.onClick.AddListener(() => MainMenuJoinPendingGame(state, needToAccept));
    }

    public void ActiveGameCellDetailsButton(GameState state) {
        //Set up the display information
        activeMapName.SetText(BoardMetadata.BoardDisplayNames[state.boardId]);
        activeCurrentPlayers.SetText("" + (state.maxUsers - state.spotsAvailable));
        activeMaxPlayers.SetText("" + state.maxUsers);
        activeTurnNumber.SetText("" + state.TurnNumber);

        //Set up the join button to call the join function with the current state
        activeJoinButton.onClick.RemoveAllListeners();
        activeJoinButton.onClick.AddListener(() => manager.LoadGame(state));
    }

    public void PublicGameCellDetailsButton(GameState state) {
    }

    public void MainMenuCreateGameButton() {
        SetMenuState(false, false, true, false, false, false);
    }

    public void MainMenuArmyBuilderButton() {
        mainMenuContainer.SetActive(false);
        SetMenuState(false, false, false, false, true, false);
    }

    public void MainMenuArmyBuilderBack() {
        mainMenuContainer.SetActive(true);
        SetMenuState(false, false, false, false, false, false);
    }

    public void MainMenuArmySelectorButton() {
        mainMenuContainer.SetActive(false);
        SetMenuState(false, false, false, false, false, true);
    }

    public void MainMenuJoinGameButton() {
        SetMenuState(false, false, false, true, false, false);
        Tuple<bool, GameStateCollection> response = networkApi.GetPublicGames();
        if (response.First) {
            foreach (var state in response.Second.states) {
                if(state.isPublic && state.createdBy != networkApi.UserInformation.Username) {
                    GameObject newGameCell = Instantiate(gameListCellPrefab);
                    Button cellButton = newGameCell.GetComponent<Button>();
                    cellButton.GetComponentsInChildren<TMP_Text>()[0].SetText(state.GetDescription());
                    cellButton.onClick.AddListener(() => PublicGameCellDetailsButton(state));
                    newGameCell.transform.SetParent(joinGameViewContent.transform, false);
                }
            }
        }
        else {
            //the request failed, inform the user
        }
    }

    public void MainMenuActiveGamesButton() {
        SetMenuState(false, true, false, false, false, false);
        Tuple<bool, GameStateCollection> response = networkApi.GetActiveGamesInformation();
        if (response.First) {
            foreach (var state in response.Second.states) {
                if (state.UsersTurn == networkApi.UserInformation.Username) {
                    GameObject newGameCell = Instantiate(gameListCellPrefab);
                    Button cellButton = newGameCell.GetComponent<Button>();
                    cellButton.GetComponentsInChildren<TMP_Text>()[0].SetText(state.GetDescription());
                    cellButton.onClick.AddListener(() => ActiveGameCellDetailsButton(state));
                    newGameCell.transform.SetParent(activeGamesViewContent.transform, false);
                }
            }
        }
        else {
            //the request failed, inform the user
        }
    }

    public void MainMenuPendingGamesButton() {
        SetMenuState(true, false, false, false, false, false);
        Tuple<bool, GameStateCollection> response = networkApi.GetPendingGamesInformation();
        int childrenCount = pendingGamesViewContent.transform.childCount;
        for(int i = 1; i < childrenCount; i++) {
            Destroy(pendingGamesViewContent.transform.GetChild(i).gameObject);
        }
        if (response.First) {
            foreach(var state in response.Second.states) {
                if (!state.isPublic && !state.ReadyUsers.Contains(networkApi.UserInformation.Username)) {
                    GameObject newGameCell = Instantiate(gameListCellPrefab);
                    Button cellButton = newGameCell.GetComponent<Button>();
                    cellButton.GetComponentsInChildren<TMP_Text>()[0].SetText(state.GetDescription());
                    cellButton.onClick.AddListener(() => PendingGameCellDetailsButton(state, !state.AcceptedUsers.Contains(networkApi.UserInformation.Username)));
                    newGameCell.transform.SetParent(pendingGamesViewContent.transform, false);
                }
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
            AddFriendHelper(userToAdd);
            friendsListInputField.text = "";
        }
        else {
            //adding a user failed
        }
    }

    private void AddFriendHelper(string username) {
        GameObject friendText = Instantiate(friendsListCellPrefab);
        friendText.GetComponent<TMP_Text>().text = username;
        friendText.transform.SetParent(friendsViewContent.transform, false);
        friendsListDict.Add(username, friendText);
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

    public void MainMenuJoinPendingGame(GameState state, bool needToAccept) {
        if(needToAccept) {
            networkApi.AcceptGame(state.id);
        }
        MainMenuArmySelectorButton();
        int maxCost = BoardMetadata.CostDict[state.boardId];
        List<ArmyPreset> presets = ArmyBuilder.GetPresetsUnderCost(maxCost);
        foreach (var preset in presets) {
            GameObject armyCell = Instantiate(gameListCellPrefab);
            Button armyButton = armyCell.GetComponent<Button>();
            armyButton.GetComponentsInChildren<TMP_Text>()[0].SetText(preset.GetDescription());
            armyButton.onClick.AddListener(() => manager.PlaceUnits(state, preset));
            armyCell.transform.SetParent(armyChooserViewport.transform, false);
        }
    }

    public void MainMenuArmySelectorBack() {
        mainMenuContainer.SetActive(true);
        SetMenuState(true, false, false, false, false, false);
    }
}
