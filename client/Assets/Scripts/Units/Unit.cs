using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The actual script on the unit that handles animations
public class Unit : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
       transform.rotation = Quaternion.Euler(-90,0,0); 
    }

    // Update is called once per frame
    void Update() {
        
    }


    //Method used to handle the attack animation
    public void Attack() {

    }

    //Method used to handle the movement animation
    public void MoveTo(List<Vector2Int> path) {

    }

    public void Kill() {
        Destroy(this.gameObject);
    }
}
