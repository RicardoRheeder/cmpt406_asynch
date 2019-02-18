using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardController {

    Tilemap tilemap;

    public void Initialize() {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        if(tilemap == null) {
            Debug.Log("Tilemap is null. This will result in problems");
        }
    }

    public Tilemap GetTilemap() {
        return tilemap;
    }

    public Vector3Int WorldToCell(Vector3 position) {
        Transform camera = Camera.main.transform;
        Vector3 correctedPosition = camera.rotation * (new Vector3(0,camera.position.y,camera.position.z) - position);
        return tilemap.WorldToCell(correctedPosition);
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
