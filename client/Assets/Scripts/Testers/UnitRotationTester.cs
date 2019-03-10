using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRotationTester : MonoBehaviour {

    Unit unit;

    int direction = 0;

    void Awake(){
        unit = GetComponent<Unit>();
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)) {
            if(direction == 5) {
                direction = 0;
            } else {
                direction += 1;
            }

            transform.rotation = Quaternion.Euler(-90,0,0);
            transform.Rotate(new Vector3(0,60*direction,0), Space.Self);
        }
    }
}
