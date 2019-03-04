using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWarController : MonoBehaviour {

    Tilemap fogTilemap;
    Tilemap baseTilemap;

    public void InitializeFogOfWar(Tilemap tilemap) {
        if(tilemap == null) {
            return;
        }

        if(fogTilemap == null) {
            GameObject newTilemap = new GameObject();
            newTilemap.AddComponent(typeof(Tilemap));
            fogTilemap = newTilemap.GetComponent<Tilemap>();
        }
        fogTilemap.transform.SetParent(tilemap.transform.parent);
        fogTilemap.transform.position = tilemap.transform.position;
    }

    public void ClearFogAtPosition(Vector2Int position) {

    }

}
