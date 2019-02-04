using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGame : MonoBehaviour {

    //Storage of the network api persistant object
    private Client networkApi;

    // Start is called before the first frame update
    void Start() {
        networkApi = GameObject.Find("Networking").GetComponent<Client>();
    }

    public void ConfirmCreate() {
        networkApi.CreateGame();
    }
}
