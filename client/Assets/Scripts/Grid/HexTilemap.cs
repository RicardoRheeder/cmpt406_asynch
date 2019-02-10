using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 API class for easy access of common grid data, 
 and custom data such as elevation and attributes
 */
public class HexTilemap : MonoBehaviour {

    Tilemap tilemap;
    Dictionary<Vector3Int,HexTile> worldTiles = new Dictionary<Vector3Int, HexTile>();

    void Awake() {
        tilemap = GetComponent<Tilemap>();
    }

    public Tilemap Tilemap { get; }

    public bool HasTile(Vector3Int pos) {
        return tilemap.HasTile(pos);
    }

    public HexTile GetHexTile(Vector3Int pos) {
        return tilemap.GetTile(pos) as HexTile;
    }

    public bool HasTileAtElevation(Vector3Int pos, Elevation el) {
        return GetTileAtElevation(pos,el) != null;
    }

    public HexTile GetTileAtElevation(Vector3Int pos, Elevation el) {
        HexTile tile = GetHexTile(pos);
        return (tile != null && tile.elevation == el) ? tile : null;
    }
    
}
