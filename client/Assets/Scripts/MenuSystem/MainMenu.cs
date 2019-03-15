using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 649
public class MainMenu : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;

    //Store reference to the audio manager
    private AudioManager audioManager;

    //Main Menu panels
    private GameObject mainMenuContainer;
    private GameObject pendingGamesPanel;
    private GameObject activeGamesPanel;
    private GameObject createGamePanel;
    private GameObject joinGamePanel;
    private GameObject armyBuilderPanel;
    private GameObject armySelectorPanel;
    private GameObject mapsPanel;
    private Dropdown mapSelect;

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
    
    //Stuff for Map Preview
    private TMP_Text sizeText;
    private TMP_Text stockText;
    private TMP_Text maxText;
    private Image preview;
    public Sprite alphaChannel;
    public Sprite theEye;
    public Sprite snakeValley;
    public Sprite valley;
    public Sprite pinnacle;
    public Sprite lowlands;
    public Sprite wheel;
    private Dropdown mapdown;
    
    private void Awake() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();

        mainMenuContainer = GameObject.Find("MainMenuContainer");
        pendingGamesPanel = GameObject.Find("PendingGamesPanel");
        activeGamesPanel = GameObject.Find("ActiveGamesPanel");
        createGamePanel = GameObject.Find("CreateGamePanel");
        joinGamePanel = GameObject.Find("JoinGamePanel");
        armyBuilderPanel = GameObject.Find("ArmyBuilder");
        armySelectorPanel = GameObject.Find("ArmySelector");
        mapsPanel = GameObject.Find("MapsContainer");
        mapSelect = GameObject.Find("Mapdown").GetComponent<Dropdown>();
        preview = GameObject.Find("preview").GetComponent<Image>();

        friendsListInputField = GameObject.Find("FriendsInputField").GetComponent<TMP_InputField>();
        friendsListDict = new Dictionary<string, GameObject> {};
        joinGameViewContent = GameObject.Find("JoinPublicGameViewport");
        activeGamesViewContent = GameObject.Find("ActiveGamesViewport");
        pendingGamesViewContent = GameObject.Find("PendingGamesViewport");
        friendsViewContent = GameObject.Find("FriendsListViewport");

        pendingMapName = GameObject.Find("PendingGameMap").GetComponent<TMP_Text>();
        pendingCurrentPlayers = GameObject.Find("PendingNumberPlayers").GetComponent<TMP_Text>();
        pendingMaxPlayers = GameObject.Find("PendingMaxPlayers").GetComponent<TMP_Text>();
        pendingJoinButton = GameObject.Find("PendingJoinButton").GetComponent<Button>();

        activeMapName = GameObject.Find("ActiveGameMap").GetComponent<TMP_Text>();
        activeCurrentPlayers = GameObject.Find("ActiveNumberPlayers").GetComponent<TMP_Text>();
        activeMaxPlayers = GameObject.Find("ActiveMaxPlayers").GetComponent<TMP_Text>();
        activeJoinButton = GameObject.Find("ActiveJoinButton").GetComponent<Button>();
        
        sizeText = GameObject.Find("sizeText").GetComponent<TMP_Text>();
        stockText = GameObject.Find("stockText").GetComponent<TMP_Text>();
        maxText = GameObject.Find("maxText").GetComponent<TMP_Text>();

        armyChooserViewport = GameObject.Find("ArmyChooserViewport");

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        //populate the map selection with proper values
        mapdown = GameObject.Find("Mapdown").GetComponent<Dropdown>();
        List<string> mapNames = new List<string>();
        foreach (BoardType name in Enum.GetValues(typeof(BoardType))) {
            if ((int)name < BoardMetadata.TEST_BOARD_LIMIT)
                mapNames.Add(BoardMetadata.BoardDisplayNames[name]);
        }
        mapdown.AddOptions(mapNames);
        mapdown.onValueChanged.RemoveAllListeners();
        mapdown.onValueChanged.AddListener(delegate {
            //play sound here
            MapSelection();
        });
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
        SetMenuState(false, false, false, false, false, false, false);
    }

    //Helper function to enable/disable menus with boolean flags
    private void SetMenuState(bool pendingState, bool activeState, bool createState, bool joinState, bool builderState, bool selectorState, bool mapState) {
        pendingGamesPanel.SetActive(pendingState);
        activeGamesPanel.SetActive(activeState);
        createGamePanel.SetActive(createState);
        joinGamePanel.SetActive(joinState);
        armyBuilderPanel.SetActive(builderState);
        armySelectorPanel.SetActive(selectorState);
        mapsPanel.SetActive(mapState);
    }

    public void PendingGameCellDetailsButton(GameState state, bool needToAccept) {
        audioManager.Play("ButtonPress");
        //Set up the display information
        pendingMapName.SetText(BoardMetadata.BoardDisplayNames[state.boardId]);
        pendingCurrentPlayers.SetText("" + (state.maxUsers - state.spotsAvailable));
        pendingMaxPlayers.SetText("" + state.maxUsers);

        //Set up the join button to call the join function with the current state
        pendingJoinButton.onClick.RemoveAllListeners();
        pendingJoinButton.onClick.AddListener(() => MainMenuJoinPendingGame(state, needToAccept));
    }

    public void ActiveGameCellDetailsButton(GameState state) {
        audioManager.Play("ButtonPress");
        //Set up the display information
        activeMapName.SetText(BoardMetadata.BoardDisplayNames[state.boardId]);
        activeCurrentPlayers.SetText("" + (state.maxUsers - state.spotsAvailable));
        activeMaxPlayers.SetText("" + state.maxUsers);

        //Set up the join button to call the join function with the current state
        activeJoinButton.onClick.RemoveAllListeners();
        activeJoinButton.onClick.AddListener(() => manager.LoadGame(state));
    }

    public void PublicGameCellDetailsButton(GameState state) {
        audioManager.Play("ButtonPress");
    }

    public void MainMenuCreateGameButton() {
        audioManager.Play("ButtonPress");
        SetMenuState(false, false, true, false, false, false, false);
    }

    public void MainMenuArmyBuilderButton() {
        audioManager.Play("ButtonPress");
        mainMenuContainer.SetActive(false);
        SetMenuState(false, false, false, false, true, false, false);
    }

    public void MainMenuArmyBuilderBack() {
        audioManager.Play("ButtonPress");
        mainMenuContainer.SetActive(true);
        SetMenuState(false, false, false, false, false, false, false);
    }

    public void MainMenuArmySelectorButton() {
        audioManager.Play("ButtonPress");
        mainMenuContainer.SetActive(false);
        SetMenuState(false, false, false, false, false, true, false);
    }

    public void MainMenuJoinGameButton() {
        audioManager.Play("ButtonPress");
        SetMenuState(false, false, false, true, false, false, false);
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
        audioManager.Play("ButtonPress");
        SetMenuState(false, true, false, false, false, false, false);
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
        audioManager.Play("ButtonPress");
        SetMenuState(true, false, false, false, false, false, false);
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
            audioManager.Play("ButtonPress");
            AddFriendHelper(userToAdd);
            friendsListInputField.text = "";
        }
        else {
            //adding a user failed
            audioManager.Play("ButtonError");
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
            audioManager.Play("ButtonPress");
            Destroy(friendsListDict[userToRemove]);
            friendsListDict.Remove(userToRemove);
        }
    }

    public void MainMenuLogoutButton() {
        audioManager.Play("ButtonPress");
        networkApi.LogoutUser();
        SceneManager.LoadScene("LoginScreen");
    }

    public void MainMenuJoinPendingGame(GameState state, bool needToAccept) {
        if(needToAccept) {
            audioManager.Play("ButtonPress");
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
    
    public void MainMenuMapsButton() {
        audioManager.Play("ButtonPress");
        SetMenuState(false, false, false, false, false, false, true);
        MapSelection();
    }
    
    public void MapSelection() {
        BoardType type = BoardMetadata.BoardDisplayNamesReverse[mapdown.options[mapdown.value].text];

        switch (type) {
            case BoardType.AlphaChannel:
                preview.sprite = alphaChannel;
                sizeText.SetText("Small");
                break;
                
            case BoardType.Lowlands:
                preview.sprite = lowlands;
                sizeText.SetText("Medium");
                break;
                
            case BoardType.Pinnacle:
                preview.sprite = pinnacle;
                sizeText.SetText("Medium");
                break;
                
            case BoardType.SnakeValley:
                preview.sprite = snakeValley;
                sizeText.SetText("Small");
                break;
                
            case BoardType.TheEye:
                preview.sprite = theEye;
                sizeText.SetText("Small");
                break;
                
            case BoardType.Valley:
                preview.sprite = valley;
                sizeText.SetText("Medium");
                break;
                
            case BoardType.Wheel:
                preview.sprite = wheel;
                sizeText.SetText("Large");
                break;

            default:
                preview.sprite = null;
                sizeText.SetText("missing");
                break;
        }
        maxText.SetText(BoardMetadata.MaxPlayersDict[type].ToString());
        stockText.SetText(BoardMetadata.CostDict[type].ToString());
    }

    public void MapsBackButton () {
        audioManager.Play("ButtonPress");
        mainMenuContainer.SetActive(true);
        SetMenuState(false, false, false, false, false, false, false);
    }
    
    public void MainMenuArmySelectorBack() {
        audioManager.Play("ButtonPress");
        mainMenuContainer.SetActive(true);
        SetMenuState(true, false, false, false, false, false, false);
    }
}
