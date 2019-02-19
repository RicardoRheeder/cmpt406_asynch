using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

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
        return tilemap.WorldToCell(position);
    }

    public Vector3Int MousePosToCell(Vector3 position) {
        Vector3 worldPoint = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        // if a raycast hits a tile, use that position
        if(Physics.Raycast(ray,out hit)) {
            worldPoint = new Vector3(hit.point.x,0,hit.point.z);
        } else {    // otherwise take our best guess
            Transform camera = Camera.main.transform;
            Vector3 screenPoint = new Vector3(position.x, position.y - camera.position.z, camera.position.y);
            // TODO: this algorithm isn't completely accurate, try to improve it
            worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        }

        return tilemap.WorldToCell(worldPoint);
    }

    public Vector3 CellToWorld(Vector3Int position) {
        Vector3 worldPosition = tilemap.CellToWorld(position);
        // TODO: take elevation into account
        return worldPosition;
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
