using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRotationTester : MonoBehaviour {

    Unit unit;
    BoardController board;

    int direction = 0;

    void Awake(){
        unit = GetComponent<Unit>();
        board = new BoardController();
        board.Initialize();
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)) {
            int oldDirection = direction;
            direction += 1;
            direction = (int)Mathf.Repeat(direction,6);

            Debug.Log("oldDirection: " + oldDirection.ToString() + " direction: " + direction.ToString());

            transform.rotation = Quaternion.AngleAxis(60*(oldDirection - direction),transform.up)*transform.rotation;
        }
    }
}
