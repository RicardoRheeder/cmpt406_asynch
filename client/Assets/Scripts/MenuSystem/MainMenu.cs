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
    private GameObject gameInviteGamesViewContent;

    //Variables for the game invites display
    private TMP_Text gameInviteMapName;
    private TMP_Text gameInviteCurrentPlayers;
    private TMP_Text gameInviteMaxPlayers;
    private TMP_Text gameInviteTurnNumber;
    private Button gameInviteJoinButton;

    //Variables for the active games display
    private TMP_Text activeMapName;
    private TMP_Text activeCurrentPlayers;
    private TMP_Text activeMaxPlayers;
    private TMP_Text activeTurnNumber;
    private Button activeJoinButton;

    //Variables for the army selector display
    private GameObject armyChooserViewport;
    private Button armySelectorDelete;
    private GameObject armySelectorPlaceObject;
    private Button armySelectorPlaceButton;

    //Reference to the game manager to start a game
    private GameManager manager;

    //Army Builder Info Pane
    private TMP_Text armyName;
    private TMP_Text generalName;
    private TMP_Text stockNum;
    private TMP_Text compensatorNum;
    private TMP_Text foundationNum;
    private TMP_Text reconNum;
    private TMP_Text trooperNum;
    private TMP_Text steamerNum;
    private TMP_Text pewPewNum;
    private TMP_Text midasNum;
    private TMP_Text claymoreNum;
    private TMP_Text powerSurgeNum;
    private GameState storedState = null;

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
        gameInviteGamesViewContent = GameObject.Find("PendingGamesViewport");
        friendsViewContent = GameObject.Find("FriendsListViewport");

        gameInviteMapName = GameObject.Find("PendingGameMap").GetComponent<TMP_Text>();
        gameInviteCurrentPlayers = GameObject.Find("PendingNumberPlayers").GetComponent<TMP_Text>();
        gameInviteMaxPlayers = GameObject.Find("PendingMaxPlayers").GetComponent<TMP_Text>();
        gameInviteJoinButton = GameObject.Find("PendingJoinButton").GetComponent<Button>();

        activeMapName = GameObject.Find("ActiveGameMap").GetComponent<TMP_Text>();
        activeCurrentPlayers = GameObject.Find("ActiveNumberPlayers").GetComponent<TMP_Text>();
        activeMaxPlayers = GameObject.Find("ActiveMaxPlayers").GetComponent<TMP_Text>();
        activeJoinButton = GameObject.Find("ActiveJoinButton").GetComponent<Button>();
        
        sizeText = GameObject.Find("sizeText").GetComponent<TMP_Text>();
        stockText = GameObject.Find("stockText").GetComponent<TMP_Text>();
        maxText = GameObject.Find("maxText").GetComponent<TMP_Text>();

        armyChooserViewport = GameObject.Find("ArmyChooserViewport");
        armySelectorDelete = GameObject.Find("ASDeleteButton").GetComponent<Button>();

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        armyName = GameObject.Find("ArmyName").GetComponent<TMP_Text>();
        generalName = GameObject.Find("GeneralName").GetComponent<TMP_Text>();
        stockNum = GameObject.Find("StockNum").GetComponent<TMP_Text>();
        compensatorNum = GameObject.Find("CompensatorNum").GetComponent<TMP_Text>();
        foundationNum = GameObject.Find("FoundationNum").GetComponent<TMP_Text>();
        reconNum = GameObject.Find("ReconNum").GetComponent<TMP_Text>();
        trooperNum = GameObject.Find("TrooperNum").GetComponent<TMP_Text>();
        steamerNum = GameObject.Find("SteamerNum").GetComponent<TMP_Text>();
        pewPewNum = GameObject.Find("PewPewNum").GetComponent<TMP_Text>();
        midasNum = GameObject.Find("MidasNum").GetComponent<TMP_Text>();
        claymoreNum = GameObject.Find("ClaymoreNum").GetComponent<TMP_Text>();
        powerSurgeNum = GameObject.Find("PowerSurgeNum").GetComponent<TMP_Text>();
        armySelectorPlaceObject = GameObject.Find("ASPlaceUnitsButton");
        armySelectorPlaceButton = armySelectorPlaceObject.GetComponent<Button>();
        armySelectorPlaceObject.SetActive(false);

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
        SetMenuState(false, false, false, false, false, false);
        armyBuilderPanel.SetActive(false);
    }

    //Helper function to enable/disable menus with boolean flags
    private void SetMenuState(bool pendingState, bool activeState, bool createState, bool joinState, bool selectorState, bool mapState) {
        pendingGamesPanel.SetActive(pendingState);
        activeGamesPanel.SetActive(activeState);
        createGamePanel.SetActive(createState);
        joinGamePanel.SetActive(joinState);
        armySelectorPanel.SetActive(selectorState);
        mapsPanel.SetActive(mapState);
    }

    //========================Game Invites Functionality========================
    public void MainMenuGamesInvitesButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(true, false, false, false, false, false);
        Tuple<bool, GameStateCollection> response = networkApi.GetPendingGamesInformation();
        int childrenCount = gameInviteGamesViewContent.transform.childCount;
        for (int i = 1; i < childrenCount; i++) {
            Destroy(gameInviteGamesViewContent.transform.GetChild(i).gameObject);
        }
        if (response.First) {
            foreach (var state in response.Second.states) {
                if (!state.isPublic && !state.ReadyUsers.Contains(networkApi.UserInformation.Username)) {
                    GameObject newGameCell = Instantiate(gameListCellPrefab);
                    Button cellButton = newGameCell.GetComponent<Button>();
                    cellButton.GetComponentsInChildren<TMP_Text>()[0].SetText(state.GetDescription());
                    cellButton.onClick.AddListener(() => GameInviteButton(state, !state.AcceptedUsers.Contains(networkApi.UserInformation.Username)));
                    newGameCell.transform.SetParent(gameInviteGamesViewContent.transform, false);
                }
            }
        }
        else {
            //the request failed, inform the user
        }
    }

    public void GameInviteButton(GameState state, bool needToAccept) {
        audioManager.Play(SoundName.ButtonPress);
        //Set up the display information
        gameInviteMapName.SetText(BoardMetadata.BoardDisplayNames[state.boardId]);
        gameInviteCurrentPlayers.SetText("" + (state.maxUsers - state.spotsAvailable));
        gameInviteMaxPlayers.SetText("" + state.maxUsers);

        //Set up the join button to call the join function with the current state
        gameInviteJoinButton.onClick.RemoveAllListeners();
        gameInviteJoinButton.onClick.AddListener(() => MainMenuJoinPendingGame(state, needToAccept));
    }

    public void MainMenuJoinPendingGame(GameState state, bool needToAccept) {
        if (needToAccept) {
            audioManager.Play(SoundName.ButtonPress);
            networkApi.AcceptGame(state.id);
        }
        MainMenuArmySelectorButton();
        SetupArmySelector(BoardMetadata.CostDict[state.boardId], state);
    }

    //========================Active Games Functionality========================
    public void MainMenuActiveGamesButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(false, true, false, false, false, false);
        Tuple<bool, GameStateCollection> response = networkApi.GetActiveGamesInformation();
        int childrenCount = activeGamesViewContent.transform.childCount;
        for (int i = 1; i < childrenCount; i++) {
            Destroy(activeGamesViewContent.transform.GetChild(i).gameObject);
        }
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

    public void ActiveGameCellDetailsButton(GameState state) {
        audioManager.Play(SoundName.ButtonPress);
        //Set up the display information
        activeMapName.SetText(BoardMetadata.BoardDisplayNames[state.boardId]);
        activeCurrentPlayers.SetText("" + (state.maxUsers - state.spotsAvailable));
        activeMaxPlayers.SetText("" + state.maxUsers);

        //Set up the join button to call the join function with the current state
        activeJoinButton.onClick.RemoveAllListeners();
        activeJoinButton.onClick.AddListener(() => {
            manager.LoadGame(state);
        });
    }

    //========================Public Games Functionality========================
    public void MainMenuJoinGameButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(false, false, false, true, false, false);
        Tuple<bool, GameStateCollection> response = networkApi.GetPublicGames();
        if (response.First) {
            foreach (var state in response.Second.states) {
                if (state.isPublic && state.createdBy != networkApi.UserInformation.Username) {
                    GameObject newGameCell = Instantiate(gameListCellPrefab);
                    Button cellButton = newGameCell.GetComponent<Button>();
                    cellButton.GetComponentsInChildren<TMP_Text>()[0].SetText(state.GetDescription());
                    cellButton.onClick.AddListener(() => PublicGameCellDetailsButton(state, !state.AcceptedUsers.Contains(networkApi.UserInformation.Username)));
                    newGameCell.transform.SetParent(joinGameViewContent.transform, false);
                }
            }
        }
        else {
            //the request failed, inform the user
        }
    }

    public void PublicGameCellDetailsButton(GameState state, bool needToAccept) {
        if (needToAccept) {
            audioManager.Play(SoundName.ButtonPress);
            networkApi.AcceptGame(state.id);
        }
        MainMenuArmySelectorButton();
        SetupArmySelector(BoardMetadata.CostDict[state.boardId], state);
    }

    //========================Create Game Functionality========================
    //Note: most of this logic is in the CreateGame script
    public void MainMenuCreateGameButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(false, false, true, false, false, false);
    }

    //========================Army Builder/Selector Functionality========================
    //Note: the actual army builder logic is in the ArmyBuilderUI script
    public void MainMenuArmyBuilderButton() {
        audioManager.Play(SoundName.ButtonPress);
        mainMenuContainer.SetActive(false);
        SetMenuState(false, false, false, false, false, false);
        armyBuilderPanel.SetActive(true);
    }

    public void MainMenuArmyBuilderBack() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(false, false, false, false, true, false);
        armyBuilderPanel.SetActive(false);
        SetupArmySelector(10000, null);
    }

    public void MainMenuArmyBuilderSave() {
        string armyName = GameObject.Find("ABNameInput").GetComponent<TMP_InputField>().text;
        if (!StringValidation.ValidateArmyName(armyName)) {
            audioManager.Play(SoundName.ButtonError);
            Debug.Log("invalid army name");
            //Something has to inform the user here
            return;
        }
        ArmyPreset createdPreset = this.GetComponent<ArmyBuilderUI>().selectedArmy;
        createdPreset.Name = armyName;
        networkApi.RegisterArmyPreset(createdPreset);
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(false, false, false, false, true, false);
        armyBuilderPanel.SetActive(false);
        if(storedState == null) {
            SetupArmySelector(10000, null);
        }
        else {
            SetupArmySelector(BoardMetadata.CostDict[storedState.boardId], storedState);
        }
    }

    public void MainMenuArmySelectorButton() {
        audioManager.Play(SoundName.ButtonPress);
        mainMenuContainer.SetActive(false);
        SetMenuState(false, false, false, false, true, false);
        armySelectorDelete.onClick.RemoveAllListeners();
        SetupArmySelector(10000, null);
    }

    public void MainMenuArmySelectorBack() {
        audioManager.Play(SoundName.ButtonPress);
        mainMenuContainer.SetActive(true);
        SetMenuState(false, false, false, false, false, false);
        storedState = null;
    }

    private void SetupArmySelector(int maxCost, GameState state) {
        if(state != null) {
            storedState = state;
        }
        networkApi.RefreshUserData();
        int childrenCount = armyChooserViewport.transform.childCount;
        for (int i = 1; i < childrenCount; i++) {
            Destroy(armyChooserViewport.transform.GetChild(i).gameObject);
        }
        List<ArmyPreset> presets = ArmyBuilder.GetPresetsUnderCost(maxCost);
        foreach (var preset in presets) {
            GameObject armyCell = Instantiate(gameListCellPrefab);
            Button armyButton = armyCell.GetComponent<Button>();
            armyButton.GetComponentsInChildren<TMP_Text>()[0].SetText(preset.GetDescription());
            armyButton.onClick.RemoveAllListeners();
            armyCell.transform.SetParent(armyChooserViewport.transform, false);
            armyButton.onClick.AddListener(() => DisplayArmyInfo(armyCell, preset, state));
        }
    }

    private void DisplayArmyInfo(GameObject ownerObject, ArmyPreset preset, GameState state) {

        int compensatorCount = 0;
        int foundationCount = 0;
        int reconCount = 0;
        int trooperCount = 0;
        int steamerCount = 0;
        int pewpewCount = 0;
        int midasCount = 0;
        int claymoreCount = 0;
        int powerSurgeCount = 0;

        foreach(int unit in preset.Units) {
            UnitType type = (UnitType)unit;
            switch (type) {
                case UnitType.compensator:
                    compensatorCount++;
                    break;
                case UnitType.foundation:
                    foundationCount++;
                    break;
                case UnitType.recon:
                    reconCount++;
                    break;
                case UnitType.trooper:
                    trooperCount++;
                    break;
                case UnitType.steamer:
                    steamerCount++;
                    break;
                case UnitType.pewpew:
                    pewpewCount++;
                    break;
                case UnitType.midas:
                    midasCount++;
                    break;
                case UnitType.claymore:
                    claymoreCount++;
                    break;
                case UnitType.powerSurge:
                    powerSurgeCount++;
                    break;
                default:
                    Debug.Log("invalid unit type in preset");
                    break;
            }
        }

        armyName.SetText(preset.Name);
        compensatorNum.SetText(compensatorCount.ToString());
        foundationNum.SetText(foundationCount.ToString());
        reconNum.SetText(reconCount.ToString());
        trooperNum.SetText(trooperCount.ToString());
        steamerNum.SetText(steamerCount.ToString());
        pewPewNum.SetText(pewpewCount.ToString());
        midasNum.SetText(midasCount.ToString());
        claymoreNum.SetText(claymoreCount.ToString());
        powerSurgeNum.SetText(powerSurgeCount.ToString());
        generalName.SetText(UnitMetadata.ReadableNames[(UnitType)preset.General]);
        stockNum.SetText(preset.Cost.ToString());

        armySelectorDelete.onClick.RemoveAllListeners();
        armySelectorDelete.onClick.AddListener(() => {
            audioManager.Play(SoundName.ButtonPress);
            networkApi.RemoveArmyPreset(preset.Id);
            Destroy(ownerObject);
        });

        if(state != null) {
            armySelectorPlaceObject.SetActive(true);
            armySelectorPlaceButton.onClick.RemoveAllListeners();
            armySelectorPlaceButton.onClick.AddListener(() => {
                audioManager.Play(SoundName.ButtonPress);
                manager.PlaceUnits(state, preset);
            });
        }
    }

    //========================Friend Functionality========================
    public void MainMenuAddFriendButton() {
        //Here we need to somehow get the string of the username we would like to add
        string userToAdd = friendsListInputField.text;
        if (StringValidation.ValidateUsername(userToAdd) && networkApi.AddFriend(userToAdd)) {
            audioManager.Play(SoundName.ButtonPress);
            AddFriendHelper(userToAdd);
            friendsListInputField.text = "";
        }
        else {
            //adding a user failed
            audioManager.Play(SoundName.ButtonError);
        }
    }

    private void AddFriendHelper(string username) {
        GameObject friendText = Instantiate(friendsListCellPrefab);
        friendText.GetComponent<TMP_Text>().text = username;
        friendText.transform.SetParent(friendsViewContent.transform, false);
        friendsListDict.Add(username, friendText);
    }

    public void MainMenuRemoveUserButton() {
        string userToRemove = friendsListInputField.text;
        if (networkApi.RemoveFriend(userToRemove)) {
            audioManager.Play(SoundName.ButtonPress);
            Destroy(friendsListDict[userToRemove]);
            friendsListDict.Remove(userToRemove);
        }
    }

    //========================Logout Functionality========================
    public void MainMenuLogoutButton() {
        audioManager.Play(SoundName.ButtonPress);
        networkApi.LogoutUser();
        SceneManager.LoadScene("LoginScreen");
    }

    //========================Map Screen Functionality========================
    public void MainMenuMapsButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(false, false, false, false, false, true);
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
        audioManager.Play(SoundName.ButtonPress);
        mainMenuContainer.SetActive(true);
        SetMenuState(false, false, false, false, false, false);
    }
}
