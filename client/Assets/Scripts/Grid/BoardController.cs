using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardController {

    Tilemap tilemap;

    void Initialize() {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
    }

    public Tilemap GetTilemap() {
        return tilemap;
    }

    public Vector3Int WorldToCell(Vector3 position) {
        return tilemap.WorldToCell(new Vector3(position.x,position.z,0));
    }

    public Vector3 CellToWorld(Vector3Int position) {
        Vector3 worldPosition = tilemap.CellToWorld(position);
        return new Vector3(worldPosition.x,0,worldPosition.y);
    }

    public HexTile GetHexTile(Vector3Int position) {
        return tilemap.GetTile(position) as HexTile;
    }

    public Elevation GetElevation(Vector3Int position) {
        HexTile tile = GetHexTile(position);
        return tile != null ? tile.elevation : 0;
    }

    public List<TileAttribute> GetTileAttributes(Vector3Int position) {
        HexTile tile = GetHexTile(position);
        return tile != null ? tile.attributes : new List<TileAttribute>();
    }

    public bool CheckIfAttributeExists(Vector3Int position, TileAttribute attribute) {
        List<TileAttribute> attributes = GetTileAttributes(position);
        for(int i = 0; i < attributes.Count; i++) {
            if(attributes[i] == attribute) {
                return true;
            }
        }
        return false;
    }
}
