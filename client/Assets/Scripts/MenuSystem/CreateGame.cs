﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
public class CreateGame : MonoBehaviour {

    //Constants
    private const int MINIMUM_TURN_TIME = 60; //1 minute in seconds
    private const int MINIMUM_FORFEIT_TIME = 6000; //10 minutes in seconds

    //Storage of the persistant object
    private Client networkApi;
    GameManager manager;
    AudioManager audioManager;

    //Cached ui elements
    [SerializeField]
    private TMP_Text forfeitDuration;
    [SerializeField]
    private Slider forfeitSlider;
    [SerializeField]
    private TMP_InputField invitePlayerInput;
    [SerializeField]
    private Dropdown mapSelection;
    [SerializeField]
    private Toggle privateToggle;
    [SerializeField]
    private TMP_InputField gameNameInput;
    [SerializeField]
    private Dropdown maxPlayersDropdown;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private GameObject maxPlayersContainer;
    [SerializeField]
    private GameObject invitePlayersContainer;
    [SerializeField]
    private GameObject invitedPlayersContainer;

    //User Message object
    [SerializeField]
    private GameObject userMessagePanel;
    [SerializeField]
    private TMP_Text userMessageText;
    [SerializeField]
    private TMP_Text userPromptText;

    //Variables needed to populate the invited players list
    [SerializeField]
    private GameObject invitedPlayersTextPrefab;
    [SerializeField]
    private GameObject invitedPlayersListViewContent;
    private List<string> opponents;
    Dictionary<string, GameObject> invitedPlayers = new Dictionary<string, GameObject>();

    //We need to find these on awake, since the "Menus.cs" file disables components on start
    public void Awake() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

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
            audioManager.Play(SoundName.ButtonPress);
            maxPlayersContainer.SetActive(false);
            invitedPlayersContainer.SetActive(true);
            invitePlayersContainer.SetActive(true);
            mapSelection.onValueChanged.RemoveAllListeners(); //Remove all of the listeners to make sure we don't cause problems
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => CreatePrivateGame());
        }
        else {
            audioManager.Play(SoundName.ButtonPress);
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
            audioManager.Play(SoundName.ButtonError);
            DisplayUserMessage("Error", "Invalid game name.\nGame name must be between " + StringValidation.GAME_NAME_LOWER_LIMIT + " and " + StringValidation.GAME_NAME_UPPER_LIMIT + " characters long and contain no special characters.");
            return;
        }

        int forfeitTime = (int)forfeitSlider.value;
        int boardId = (int)BoardMetadata.BoardDisplayNamesReverse[mapSelection.options[mapSelection.value].text];

        int maxPlayers = BoardMetadata.MaxPlayersDict[(BoardType)boardId];
        if (opponents.Count > maxPlayers - 1) {
            Debug.Log("game has too many players");
            foreach (var item in invitedPlayers) {
                Destroy(item.Value);
            }
            invitedPlayers.Clear();
            opponents.Clear();
            return;
        }

        if (networkApi.CreatePrivateGame(gameName, forfeitTime, opponents, boardId)) {
            foreach (var item in invitedPlayers) {
                Destroy(item.Value);
            }
            invitedPlayers.Clear();
            opponents.Clear();
            audioManager.Play(SoundName.ButtonPress);
            GameObject.Find("Canvas").GetComponent<MainMenu>().SetPlayMenuState();
            DisplayUserMessage("Incoming Message...", "Game created successfully.");
        }
        else {
            audioManager.Play(SoundName.ButtonError);
            DisplayUserMessage("Error", "Could not create game.\nCheck your internet connection and try again.");
        }
    }

    public void InvitePlayer() {
        string username = invitePlayerInput.text;
        if (StringValidation.ValidateUsername(username)) {
            GameObject invitedUser = Instantiate(invitedPlayersTextPrefab);
            invitedUser.GetComponent<TMP_Text>().text = username;
            invitedUser.transform.SetParent(invitedPlayersListViewContent.transform, false);
            opponents.Add(username);
            invitedPlayers.Add(username, invitedUser);
            invitePlayerInput.text = "";
        }
    }

    private void CreatePublicGame() {

        //Check if we are creating a public or a private game
        string gameName = gameNameInput.text;
        if (!StringValidation.ValidateGameName(gameName)) {
            DisplayUserMessage("Error", "Invalid game name.\nGame name must be between " + StringValidation.GAME_NAME_LOWER_LIMIT + " and " + StringValidation.GAME_NAME_UPPER_LIMIT + " characters long and contain no special characters.");
            return;
        }

        int forfeitTime = (int)forfeitSlider.value < MINIMUM_FORFEIT_TIME ? MINIMUM_FORFEIT_TIME : (int)forfeitSlider.value;
        int boardId = (int)BoardMetadata.BoardDisplayNamesReverse[mapSelection.options[mapSelection.value].text];
        int maxPlayers = Int32.Parse(maxPlayersDropdown.options[maxPlayersDropdown.value].text);

        if (networkApi.CreatePublicGame(gameName, forfeitTime, maxPlayers, boardId)) {
            GameObject.Find("Canvas").GetComponent<MainMenu>().SetPlayMenuState();
            foreach (var item in invitedPlayers) {
                Destroy(item.Value);
            }
            invitedPlayers.Clear();
            DisplayUserMessage("Incoming Message...", "Game created successfully.");
        }
        else {
            audioManager.Play(SoundName.ButtonError);
            DisplayUserMessage("Error", "Could not create game.\nCheck your internet connection and try agai.n");
        }
    }

    public void UpdateForfeitDuration() {
        forfeitDuration.text = forfeitSlider.value <= 0.0f ? "infinite" : "" + forfeitSlider.value;
    }

    private void DisplayUserMessage(string promptName, string message) {
        userMessageText.SetText(message);
        userPromptText.SetText(promptName);
        userMessagePanel.SetActive(true);
    }
}
