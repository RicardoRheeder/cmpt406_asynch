using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTilemap : MonoBehaviour {

    Grid grid;
    Dictionary<Vector3Int,ElevationTile> worldTiles = new Dictionary<Vector3Int, ElevationTile>();

    public bool hasTile(Vector3Int pos) {
        return worldTiles.ContainsKey(pos);
    }

    public bool hasTileAtElevation(Vector3Int pos, Elevation el) {
        ElevationTile tile = null;
        worldTiles.TryGetValue(pos, out tile);
        return tile != null && tile.Elevation == el;
    }
    
}
