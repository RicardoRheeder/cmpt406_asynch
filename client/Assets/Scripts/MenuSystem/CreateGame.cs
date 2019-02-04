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

    // Start is called before the first frame update
    void Start() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
        opponents = new List<string>();

        turnDuration = GameObject.Find("DurationTimeDisplay").GetComponent<TMP_Text>();
        turnSlider = GameObject.Find("DurationSlider").GetComponent<Slider>();
        forfeitDuration = GameObject.Find("ForfeitTimeDisplay").GetComponent<TMP_Text>();
        forfeitSlider = GameObject.Find("ForfeitTimeSlider").GetComponent<Slider>();
        invitePlayer = GameObject.Find("InvitePlayerInputField").GetComponent<TMP_InputField>();
        mapSelection = GameObject.Find("MapDropdown").GetComponent<TMP_Dropdown>();
    }

    //TODO: input validation both client and backend side
    public void ConfirmCreate() {
        string gameName = GameObject.Find("GameNameInputField").GetComponent<TMP_InputField>().text;
        if (!StringValidation.ValidateGameName(gameName)) {
            Debug.Log("invalid game name");
            //Something has to inform the user here
        }

        int turnTime = (int)turnSlider.value;
        int forfeitTime = (int)forfeitSlider.value;

        int boardId = 1; //Hardcoded, properly parse this in the future

        networkApi.CreatePrivateGame(gameName, turnTime, forfeitTime, opponents, boardId); //Hardcoded to create a private game now
        opponents.Clear();
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
