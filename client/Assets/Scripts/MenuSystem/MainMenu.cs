using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 649
public class MainMenu : MonoBehaviour {

    //Storage of the persistent objects
    private Client networkApi;
    private AudioManager audioManager;
    private GameManager manager;

    //Master Panels
    [SerializeField]
    private GameObject mainMenuPanel;
    [SerializeField]
    private GameObject armyBuilderPanel;
    [SerializeField]
    private GameObject armySelectorPanel;
    [SerializeField]
    private GameObject mapsContainerPanel;
    [SerializeField]
    private GameObject playContainerPanel;
    [SerializeField]
    private GameObject friendsListPanel;
    [SerializeField]
    private GameObject tutorialPanel;

    //Friends variables
    [SerializeField]
    private GameObject friendsListViewContent;
    [SerializeField]
    TMP_InputField friendsListInputField;
    private Dictionary<string, GameObject> friendsListDict;

    //Play Container Panel children
    [SerializeField]
    private GameObject createGamePanel;
    [SerializeField]
    private GameObject publicGamesPanel;
    [SerializeField]
    private GameObject yourGamesPanel;
    [SerializeField]
    private GameObject gameInvitesPanel;

    //Ui Cell Prefabs
    [SerializeField]
    private GameObject friendsListCellPrefab;
    [SerializeField]
    private GameObject gameListCellPrefab;

    //User Message object
    [SerializeField]
    private GameObject userMessagePanel;
    [SerializeField]
    private TMP_Text userMessageText;
    [SerializeField]
    private TMP_Text userPromptText;

    //Your Games variables
    [SerializeField]
    private GameObject yourGamesListViewContent;
    [SerializeField]
    private TMP_Text yourGamesMapName;
    [SerializeField]
    private TMP_Text yourGamesMaxPlayers;
    [SerializeField]
    private Button yourGamesJoinButton;

    //Game Invites variables
    [SerializeField]
    private GameObject gameInvitesListViewContent;
    [SerializeField]
    private TMP_Text gameInviteMapName;
    [SerializeField]
    private TMP_Text gameInviteCurrentPlayers;
    [SerializeField]
    private TMP_Text gameInviteMaxPlayers;
    [SerializeField]
    private Button gameInviteJoinButton;

    //Army Selector variables
    [SerializeField]
    private GameObject armySelectorListViewContent;
    [SerializeField]
    private TMP_Text armyName;
    [SerializeField]
    private TMP_Text generalName;
    [SerializeField]
    private TMP_Text stockNum;
    [SerializeField]
    private TMP_Text compensatorNum;
    [SerializeField]
    private TMP_Text foundationNum;
    [SerializeField]
    private TMP_Text reconNum;
    [SerializeField]
    private TMP_Text trooperNum;
    [SerializeField]
    private TMP_Text steamerNum;
    [SerializeField]
    private TMP_Text pewPewNum;
    [SerializeField]
    private TMP_Text midasNum;
    [SerializeField]
    private TMP_Text claymoreNum;
    [SerializeField]
    private TMP_Text powerSurgeNum;
    private GameState storedState = null;
    [SerializeField]
    private GameObject armySelectorObject;
    [SerializeField]
    private Button armySelectorPlaceUnits;
    [SerializeField]
    private Button armySelectorCreate;
    [SerializeField]
    private Button armySelectorDelete;
    [SerializeField]
    private TMP_InputField armyBuilderNameInput;

    //Variables for map display
    [SerializeField]
    private Sprite alphaChannel;
    [SerializeField]
    private Sprite theEye;
    [SerializeField]
    private Sprite snakeValley;
    [SerializeField]
    private Sprite valley;
    [SerializeField]
    private Sprite pinnacle;
    [SerializeField]
    private Sprite lowlands;
    [SerializeField]
    private Sprite wheel;
    [SerializeField]
    private Dropdown mapPreviewDropdown;
    [SerializeField]
    private TMP_Text mapPreviewSizeText;
    [SerializeField]
    private TMP_Text mapPreviewStockText;
    [SerializeField]
    private TMP_Text mapPreviewMaxPlayersText;
    [SerializeField]
    private Image mapPreview;

    private void Awake() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        friendsListDict = new Dictionary<string, GameObject>();

        //populate the map selection with proper values
        List<string> mapNames = new List<string>();
        foreach (BoardType name in Enum.GetValues(typeof(BoardType))) {
            if ((int)name < BoardMetadata.TEST_BOARD_LIMIT)
                mapNames.Add(BoardMetadata.BoardDisplayNames[name]);
        }
        mapPreviewDropdown.AddOptions(mapNames);
        mapPreviewDropdown.onValueChanged.RemoveAllListeners();
        mapPreviewDropdown.onValueChanged.AddListener(delegate {
            audioManager.Play(SoundName.ButtonPress);
            MapSelection();
        });
    }

    // Start is called before the first frame update
    void Start() {
        SetMenuState(mainMenuState: true, playContainerState: true);

        List<string> userFriends = networkApi.UserInformation.Friends;
        foreach(string friend in userFriends) {
            AddFriendHelper(friend);
        }
    }

    //========================Helper Functions========================
    //Helper function to enable/disable menus with boolean flags
    private void SetMenuState(bool mainMenuState=false, bool armyBuilderState=false, bool armySelectorState=false, bool mapsContainerState=false, bool playContainerState=false, bool tutorialPanelState=false) {
        mainMenuPanel.SetActive(mainMenuState);
        armyBuilderPanel.SetActive(armyBuilderState);
        armySelectorPanel.SetActive(armySelectorState);
        mapsContainerPanel.SetActive(mapsContainerState);
        playContainerPanel.SetActive(playContainerState);
        tutorialPanel.SetActive(tutorialPanelState);
    }

    //Helper function to destroy all children gameobjects of a content variable
    private void DestroyChildrenInList(GameObject list) {
        int childrenCount = list.transform.childCount;
        for (int i = 0; i < childrenCount; i++) {
            Destroy(list.transform.GetChild(i).gameObject);
        }
    }

    private void DisplayUserMessage(string promptName, string message) {
        userMessageText.SetText(message);
        userPromptText.SetText(promptName);
        userMessagePanel.SetActive(true);
    }

    //========================Master Button Functionality========================
    public void MasterArmiesButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(armySelectorState: true);
        SetupArmySelector(10000, null);
    }

    public void MasterPlayButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(mainMenuState: true, playContainerState: true);
        SetPlayMenuState(); //resets them all to be inactive
    }

    public void MasterTutorialButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(tutorialPanelState: true);
    }

    //========================Logout Functionality========================
    public void LogoutButton() {
        audioManager.Play(SoundName.ButtonPress);
        networkApi.LogoutUser();
        SceneManager.LoadScene("LoginScreen");
    }

    //========================Play Menu Functionality========================
    //Helper Functions while in the play menu
    public void SetPlayMenuState(bool createGameState=false, bool publicGamesState = false, bool yourGamesState=false, bool gameInvitesState = false) {
        createGamePanel.SetActive(createGameState);
        publicGamesPanel.SetActive(publicGamesState);
        yourGamesPanel.SetActive(yourGamesState);
        gameInvitesPanel.SetActive(gameInvitesState);
    }

    //========================Create Game Functionality========================
    //Note: most of this logic is in the CreateGame script
    public void CreateGameButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetPlayMenuState(createGameState: true);
    }

    //========================Public Games Functionality========================
    public void PublicGamesButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetPlayMenuState(publicGamesState: true);
        //Public game code goes here
    }

    //========================Your Games Functionality========================
    public void YourGamesButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetPlayMenuState(yourGamesState: true);
        DestroyChildrenInList(yourGamesListViewContent);

        Tuple<bool, GameStateCollection> response = networkApi.GetActiveGamesInformation();
        if (response.First) {
            foreach (var state in response.Second.states) {
                if (state.UsersTurn == networkApi.UserInformation.Username) {
                    GameObject newGameCell = Instantiate(gameListCellPrefab);
                    Button cellButton = newGameCell.GetComponent<Button>();
                    cellButton.GetComponentsInChildren<TMP_Text>()[0].SetText(state.GetDescription());
                    cellButton.onClick.AddListener(() => YourGameCellButton(state));
                    newGameCell.transform.SetParent(yourGamesListViewContent.transform, false);
                }
            }
        }
        else {
            DisplayUserMessage("Error", "Failed to get game information from server.");
        }
    }

    public void YourGameCellButton(GameState state) {
        audioManager.Play(SoundName.ButtonPress);
        //Set up the display information
        yourGamesMapName.SetText(BoardMetadata.BoardDisplayNames[state.boardId]);
        yourGamesMaxPlayers.SetText("" + state.maxUsers);

        //Set up the join button to call the join function with the current state
        yourGamesJoinButton.onClick.RemoveAllListeners();
        yourGamesJoinButton.onClick.AddListener(() => {
            audioManager.Play(SoundName.ButtonPress);
            manager.LoadGame(state);
        });
    }

    //========================Game Invites Functionality========================
    public void GameInvitesButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetPlayMenuState(gameInvitesState: true);
        DestroyChildrenInList(gameInvitesListViewContent);

        Tuple<bool, GameStateCollection> response = networkApi.GetPendingGamesInformation();
        if (response.First) {
            foreach (var state in response.Second.states) {
                if (!state.isPublic && !state.ReadyUsers.Contains(networkApi.UserInformation.Username)) {
                    GameObject newGameCell = Instantiate(gameListCellPrefab);
                    Button cellButton = newGameCell.GetComponent<Button>();
                    cellButton.GetComponentsInChildren<TMP_Text>()[0].SetText(state.GetDescription());
                    cellButton.onClick.AddListener(() => GameInvitesCellButton(state, !state.AcceptedUsers.Contains(networkApi.UserInformation.Username)));
                    newGameCell.transform.SetParent(gameInvitesListViewContent.transform, false);
                }
            }
        }
        else {
            DisplayUserMessage("Error", "Failed to get game information from server.");
        }
    }

    public void GameInvitesCellButton(GameState state, bool needToAccept) {
        audioManager.Play(SoundName.ButtonPress);
        //Set up the display information
        gameInviteMapName.SetText(BoardMetadata.BoardDisplayNames[state.boardId]);
        gameInviteCurrentPlayers.SetText("" + (state.maxUsers - state.ReadyUsers.Count));
        gameInviteMaxPlayers.SetText("" + state.maxUsers);

        //Set up the join button to call the join function with the current state
        gameInviteJoinButton.onClick.RemoveAllListeners();
        gameInviteJoinButton.onClick.AddListener(() => JoinPendingGameCellButton(state, needToAccept));
    }

    //TODO
    private void JoinPendingGameCellButton(GameState state, bool needToAccept) {
        audioManager.Play(SoundName.ButtonPress);
        if (needToAccept) {
            audioManager.Play(SoundName.ButtonPress);
            networkApi.AcceptGame(state.id);
        }
        SetMenuState(armySelectorState: true);
        SetupArmySelector(BoardMetadata.CostDict[state.boardId], state);
    }

    //========================Army Menu Functionality========================
    //========================Army Menu Helpers========================
    private void SetupArmySelector(int maxCost, GameState state) {
        if (state != null) {
            storedState = state;
        }
        else {
            storedState = null;
        }
        armySelectorObject.SetActive(false);
        DestroyChildrenInList(armySelectorListViewContent);

        if (!networkApi.RefreshUserData()) {
            DisplayUserMessage("Error", "Failed to get user information from server.");
            return;
        }

        List<ArmyPreset> presets = ArmyBuilder.GetPresetsUnderCost(maxCost);
        foreach (var preset in presets) {
            GameObject armyCell = Instantiate(gameListCellPrefab);
            Button armyButton = armyCell.GetComponent<Button>();
            armyButton.GetComponentsInChildren<TMP_Text>()[0].SetText(preset.GetDescription());
            armyButton.onClick.RemoveAllListeners();
            armyCell.transform.SetParent(armySelectorListViewContent.transform, false);
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

        foreach (int unit in preset.Units) {
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

        if (state != null) {
            armySelectorObject.SetActive(true);
            armySelectorPlaceUnits.onClick.RemoveAllListeners();
            armySelectorPlaceUnits.onClick.AddListener(() => {
                audioManager.Play(SoundName.ButtonPress);
                manager.PlaceUnits(state, preset);
            });
        }
    }

    //========================Army Builder Functionality========================
    //Note: the actual army builder logic is in the ArmyBuilderUI script
    public void ArmyBuilderCreate() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(armyBuilderState: true);
    }

    public void ArmyBuilderSave() {
        audioManager.Play(SoundName.ButtonPress);
        string armyName = armyBuilderNameInput.text;
        if (!StringValidation.ValidateArmyName(armyName)) {
            audioManager.Play(SoundName.ButtonError);
            DisplayUserMessage("Error", "Invalid army name.\nArmy Names must be between " + StringValidation.ARMY_NAME_LOWER_LIMIT + " and " + StringValidation.ARMY_NAME_UPPER_LIMIT + " characters and contain no special characters.");
            return;
        }
        ArmyPreset createdPreset = this.GetComponent<ArmyBuilderUI>().selectedArmy;
        createdPreset.Name = armyName;
        networkApi.RegisterArmyPreset(createdPreset);
        SetMenuState(armySelectorState: true);
        if (storedState == null) {
            SetupArmySelector(10000, null);
        }
        else {
            SetupArmySelector(BoardMetadata.CostDict[storedState.boardId], storedState);
        }
    }

    //========================Friend Functionality========================
    private void AddFriendHelper(string username) {
        GameObject friendText = Instantiate(friendsListCellPrefab);
        friendText.GetComponent<TMP_Text>().text = username;
        friendText.transform.SetParent(friendsListViewContent.transform, false);
        friendsListDict.Add(username, friendText);
    }

    public void AddFriendButton() {
        string userToAdd = friendsListInputField.text;
        if (StringValidation.ValidateUsername(userToAdd)){
            if (networkApi.AddFriend(userToAdd)) {
                audioManager.Play(SoundName.ButtonPress);
                AddFriendHelper(userToAdd);
                friendsListInputField.text = "";
            }
            else {
                audioManager.Play(SoundName.ButtonError);
                DisplayUserMessage("Error", "Either the friend you're trying to add doesn't exist or you already have that friend.\nRemember usernames are case sensitive!");
            }
        }
        else {
            audioManager.Play(SoundName.ButtonError);
            DisplayUserMessage("Error", "Either the friend you're trying to add doesn't exist or you already have that friend.\nUsernames must be between " + StringValidation.CREDENTIALS_LOWER_LIMIT + " and " + StringValidation.CREDENTIALS_UPPER_LIMIT + " characters and contain no special characters or spaces.\nRemember usernames are case sensitive!");
        }
    }

    public void RemoveFriendButton() {
        string userToRemove = friendsListInputField.text;
        if (networkApi.RemoveFriend(userToRemove)) {
            audioManager.Play(SoundName.ButtonPress);
            Destroy(friendsListDict[userToRemove]);
            friendsListDict.Remove(userToRemove);
            friendsListInputField.text = "";
        }
    }

    //========================Tutorial Functionality========================
    //Most of the tutorial functionality is in its own script: TutorialUI.cs
    //========================Map Screen Functionality========================
    public void MapsButton() {
        audioManager.Play(SoundName.ButtonPress);
        SetMenuState(mapsContainerState: true, mainMenuState: true);
        MapSelection();
    }
    
    public void MapSelection() {
        BoardType type = BoardMetadata.BoardDisplayNamesReverse[mapPreviewDropdown.options[mapPreviewDropdown.value].text];

        switch (type) {
            case BoardType.AlphaChannel:
                mapPreview.sprite = alphaChannel;
                mapPreviewSizeText.SetText("Small");
                break;
                
            case BoardType.Lowlands:
                mapPreview.sprite = lowlands;
                mapPreviewSizeText.SetText("Medium");
                break;
                
            case BoardType.Pinnacle:
                mapPreview.sprite = pinnacle;
                mapPreviewSizeText.SetText("Medium");
                break;
                
            case BoardType.SnakeValley:
                mapPreview.sprite = snakeValley;
                mapPreviewSizeText.SetText("Small");
                break;
                
            case BoardType.TheEye:
                mapPreview.sprite = theEye;
                mapPreviewSizeText.SetText("Small");
                break;
                
            case BoardType.Valley:
                mapPreview.sprite = valley;
                mapPreviewSizeText.SetText("Medium");
                break;
                
            case BoardType.Wheel:
                mapPreview.sprite = wheel;
                mapPreviewSizeText.SetText("Large");
                break;

            default:
                mapPreview.sprite = null;
                mapPreviewSizeText.SetText("missing");
                break;
        }
        mapPreviewMaxPlayersText.SetText(BoardMetadata.MaxPlayersDict[type].ToString());
        mapPreviewStockText.SetText(BoardMetadata.CostDict[type].ToString());
    }
}
