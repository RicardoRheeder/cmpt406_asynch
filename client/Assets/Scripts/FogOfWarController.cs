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

        TileBase[] tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        for(int x = 0; x < tilemap.cellBounds.x; x++) {
            for(int y = 0; y < tilemap.cellBounds.y; y++) {
                if(tiles[x + y * tilemap.cellBounds.size.x] != null) {
                    fogTilemap.SetTile(new Vector3Int(x,y,0), new FogTile());
                }
            }
        }
    }

    public void ClearFogAtPosition(Vector2Int position) {
        if(fogTilemap == null) {
            return;
        }

        fogTilemap.SetTile((Vector3Int)position, null);
    }

}
