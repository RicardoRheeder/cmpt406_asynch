using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardController : MonoBehaviour {

    Tilemap tilemap;

    void Awake(){
        tilemap = GetComponent<Tilemap>();
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
}
