using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateGame : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;

    private List<string> opponents;

    //Cached ui elements
    TMP_Text turnDuration;
    Slider turnSlider;
    TMP_Text forfeitDuration;
    Slider forfeitSlider;
    TMP_InputField invitePlayer;
    TMP_Dropdown mapSelection;

    GameManager manager;

    //We need to find these on awake, since the "Menus.cs" file disables components on start
    public void Awake() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();

        turnDuration = GameObject.Find("DurationTimeDisplay").GetComponent<TMP_Text>();
        turnSlider = GameObject.Find("DurationSlider").GetComponent<Slider>();
        forfeitDuration = GameObject.Find("ForfeitTimeDisplay").GetComponent<TMP_Text>();
        forfeitSlider = GameObject.Find("ForfeitTimeSlider").GetComponent<Slider>();
        invitePlayer = GameObject.Find("InvitePlayerInputField").GetComponent<TMP_InputField>();
        mapSelection = GameObject.Find("MapDropdown").GetComponent<TMP_Dropdown>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //populate the map selection with proper values
    }

    void Start() {
        opponents = new List<string>();
    }

    //TODO: input validation both client and backend side
    public void ConfirmCreate() {
        //Check if we are creating a public or a private game
        string gameName = GameObject.Find("GameNameInputField").GetComponent<TMP_InputField>().text;
        if (!StringValidation.ValidateGameName(gameName)) {
            Debug.Log("invalid game name");
            //Something has to inform the user here
        }

        int turnTime = (int)turnSlider.value;
        int forfeitTime = (int)forfeitSlider.value;

        int boardId = 1; //Hardcoded, properly parse this in the future

        if (networkApi.CreatePrivateGame(gameName, turnTime, forfeitTime, opponents, boardId)) {
            opponents.Clear();
            GameObject.Find("Canvas").GetComponent<MainMenu>().SetInitialMenuState();
            //Maybe pop up a game created message that fades out?
        }
        else {
            //Inform the user that it failed for some reason
        }
    }

    public void InvitePlayer() {
        Debug.Log("Adding: " + invitePlayer.text);
        opponents.Add(invitePlayer.text);
        invitePlayer.text = "";
    }

    public void UpdateTurnDuration(string value) {
        turnDuration.text = "" + turnSlider.value;
    }

    public void UpdateForfeitDuration(string value) {
        forfeitDuration.text = "" + forfeitSlider.value;
    }
}
