using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
    Temporary class that tests grid functionality
 */
public class GridTester : MonoBehaviour {

    public Tilemap tilemap;

    // Update is called once per frame
    void Update(){
       if(Input.GetMouseButtonDown(0)) {
           CheckHasTile();
       } 
    }

    void CheckHasTile() {
        if(!tilemap) {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
    }
}
