using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileObject : MonoBehaviour {
    
    void Awake() {
        Collider col = GetComponent<Collider>();
        if(col == null) {
            gameObject.AddComponent(typeof(BoxCollider));
        }
    }
}
