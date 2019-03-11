using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateGame : MonoBehaviour {

    //Constants
    private const int MINIMUM_TURN_TIME = 60; //1 minute in seconds
    private const int MINIMUM_FORFEIT_TIME = 6000; //10 minutes in seconds

    //Storage of the network api persistant object
    private Client networkApi;
    private List<string> opponents;

    //Cached ui elements
    private TMP_Text turnDuration;
    private Slider turnSlider;
    private TMP_Text forfeitDuration;
    private Slider forfeitSlider;
    private TMP_InputField invitePlayerInput;
    private Dropdown mapSelection;
    private Toggle privateToggle;
    private TMP_InputField gameNameInput;
    private Dropdown maxPlayersDropdown;
    private Button confirmButton;

    private GameObject maxPlayersContainer;
    private GameObject invitePlayersContainer;
    private GameObject invitedPlayersContainer;

    //Variables needed to populate the invited players list
    [SerializeField]
    private GameObject invitedPlayersTextPrefab;
    private GameObject inviedPlayersViewContent;

    GameManager manager;

    //We need to find these on awake, since the "Menus.cs" file disables components on start
    public void Awake() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();

        forfeitDuration = GameObject.Find("ForfeitTimeDisplay").GetComponent<TMP_Text>();
        forfeitSlider = GameObject.Find("ForfeitTimeSlider").GetComponent<Slider>();
        invitePlayerInput = GameObject.Find("InvitePlayerInputField").GetComponent<TMP_InputField>();
        mapSelection = GameObject.Find("MapDropdown").GetComponent<Dropdown>();
        inviedPlayersViewContent = GameObject.Find("invitedFriendsViewport");
        privateToggle = GameObject.Find("PrivateToggle").GetComponent<Toggle>();
        gameNameInput = GameObject.Find("GameNameInputField").GetComponent<TMP_InputField>();
        maxPlayersContainer = GameObject.Find("maxPlayers");
        invitedPlayersContainer = GameObject.Find("invitePlayerDisplay");
        invitePlayersContainer = GameObject.Find("invitePlayer");
        maxPlayersDropdown = GameObject.Find("NumOfPlayersDropdown").GetComponent<Dropdown>();
        confirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //populate the map selection with proper values
        List<string> mapNames = new List<string>();
        foreach(BoardType name in Enum.GetValues(typeof(BoardType))) {
            if((int)name < BoardMetadata.TEST_BOARD_LIMIT)
                mapNames.Add(BoardMetadata.BoardDisplayNames[name]);
        }
        mapSelection.AddOptions(mapNames);

        //Setup the onclick listener
        privateToggle.onValueChanged.AddListener(delegate {
            SetupCreateScreen(privateToggle);
        });

        //set up the default state
        SetupCreateScreen(privateToggle);
    }

    void Start() {
        opponents = new List<string>();
    }

    private void SetupCreateScreen(Toggle privateToggle) {
        if(privateToggle.isOn) {
            maxPlayersContainer.SetActive(false);
            invitedPlayersContainer.SetActive(true);
            invitePlayersContainer.SetActive(true);
            mapSelection.onValueChanged.RemoveAllListeners(); //Remove all of the listeners to make sure we don't cause problems
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => CreatePrivateGame());
        }
        else {
            maxPlayersContainer.SetActive(true);
            invitedPlayersContainer.SetActive(false);
            invitePlayersContainer.SetActive(false);
            PopulateMaxPlayers();
            mapSelection.onValueChanged.AddListener(delegate {
                PopulateMaxPlayers();
            });
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => CreatePublicGame());
        }
    }

    private void PopulateMaxPlayers() {
        BoardType type = BoardMetadata.BoardDisplayNamesReverse[mapSelection.options[mapSelection.value].text];
        int maxPlayers = BoardMetadata.MaxPlayersDict[type];
        List<string> playerOptions = new List<string>();
        for(int i = 2; i <= maxPlayers; i++) {
            playerOptions.Add("" + i);
        }
        maxPlayersDropdown.ClearOptions();
        maxPlayersDropdown.AddOptions(playerOptions);
    }

    private void CreatePrivateGame() {
        //Check if we are creating a public or a private game
        string gameName = gameNameInput.text;
        if (!StringValidation.ValidateGameName(gameName)) {
            Debug.Log("invalid game name");
            //Something has to inform the user here
            return;
        }

        int forfeitTime = (int)forfeitSlider.value;
        int boardId = (int)BoardMetadata.BoardDisplayNamesReverse[mapSelection.options[mapSelection.value].text];

        if (networkApi.CreatePrivateGame(gameName, forfeitTime, opponents, boardId)) {
            opponents.Clear();
            foreach (var item in invitedPlayers) {
                Destroy(item.Value);
            }
            GameObject.Find("Canvas").GetComponent<MainMenu>().SetInitialMenuState();
            //Maybe pop up a game created message that fades out?
        }
        else {
            //Inform the user that it failed for some reason
        }
    }

    Dictionary<string, GameObject> invitedPlayers = new Dictionary<string, GameObject>();
    public void InvitePlayer() {
        string username = invitePlayerInput.text;
        if (StringValidation.ValidateUsername(username)) {
            GameObject invitedUser = Instantiate(invitedPlayersTextPrefab);
            invitedUser.GetComponent<TMP_Text>().text = username;
            invitedUser.transform.SetParent(inviedPlayersViewContent.transform, false);
            opponents.Add(username);
            invitedPlayers.Add(username, invitedUser);
            invitePlayerInput.text = "";
        }
    }

    private void CreatePublicGame() {

        //Check if we are creating a public or a private game
        string gameName = gameNameInput.text;
        if (!StringValidation.ValidateGameName(gameName)) {
            Debug.Log("invalid game name");
            //Something has to inform the user here
            return;
        }

        int forfeitTime = (int)forfeitSlider.value < MINIMUM_FORFEIT_TIME ? MINIMUM_FORFEIT_TIME : (int)forfeitSlider.value;
        int boardId = (int)BoardMetadata.BoardDisplayNamesReverse[mapSelection.options[mapSelection.value].text];
        int maxPlayers = Int32.Parse(maxPlayersDropdown.options[maxPlayersDropdown.value].text);

        if (networkApi.CreatePublicGame(gameName, forfeitTime, maxPlayers, boardId)) {
            GameObject.Find("Canvas").GetComponent<MainMenu>().SetInitialMenuState();
            foreach (var item in invitedPlayers) {
                Destroy(item.Value);
            }
            invitedPlayers.Clear();
            //Maybe pop up a game created message that fades out?
        }
        else {
            //Inform the user that it failed for some reason
        }
    }

    public void UpdateTurnDuration() {
        turnDuration.text = turnSlider.value <= 0.0f ? "infinite" : "" + turnSlider.value;
    }

    public void UpdateForfeitDuration() {
        forfeitDuration.text = forfeitSlider.value <= 0.0f ? "infinite" : "" + forfeitSlider.value;
    }
}
