using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTester : MonoBehaviour {

    BoardController boardController;
    FogOfWarController fogOfWarController;

    void Awake(){
        fogOfWarController = GetComponent<FogOfWarController>();
        boardController = new BoardController();
    }

    void Start() {
        fogOfWarController.InitializeFogOfWar(boardController.GetTilemap());
    }

    void Update() {
        if(Input.GetMouseButton(0)) {
            fogOfWarController.ClearFogAtPosition(boardController.MousePosToCell());
        }
    }
}
