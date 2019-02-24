using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class BoardController {

    public float tileHeight = 4.5f; // the height of the tile model. used to calculate the y world position
    public float elevationHeight = 2.4f; // the difference between each elevation level. used to calculate the y world position
    Tilemap tilemap; // the tilemap instance
    Plane plane;

    // Initializes the board controller. Must be called before other methods can function
    public void Initialize() {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        if(tilemap == null) {
            Debug.Log("Tilemap is null. This will result in problems");
        } else {
            plane = new Plane(Vector3.up, Vector3.zero); // creates a flat horizontal plane at y = 0
        }
    }

    // Getter for direct access to the tilemap. Use other board controller methods unless otherwise needed
    public Tilemap GetTilemap() {
        return tilemap;
    }

    // Converts a world position to a cell (tile/grid) position
    public Vector3Int WorldToCell(Vector3 position) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }
        return tilemap.WorldToCell(position);
    }

    // Converts the mouse position to a cell position - the argument should be Input.mousePosition
    // Fires a raycast and returns the position of the tile it hits,
    // otherwise it will account for the camera position/rotation and calculate the tile position
    [System.Obsolete("Use MousePosToCell() instead")]
    public Vector3Int MousePosToCell(Vector3 position) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        Vector3 worldPoint = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(position); // create a raycast from the mouse position
        RaycastHit hit;

        // if a raycast hits a tile, use that position
        if(Physics.Raycast(ray,out hit)) {
            worldPoint = new Vector3(hit.point.x,0,hit.point.z);
        } else {    // otherwise take our best calculated guess
            Transform camera = Camera.main.transform;
            Vector3 screenPoint = new Vector3(position.x, position.y - camera.position.z, camera.position.y);
            // TODO: this algorithm isn't completely accurate, try to improve it
            worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        }

        return tilemap.WorldToCell(worldPoint);
    }

    // Converts the mouse position to a cell position
    // Fires a raycast and returns the position of the tile it hits,
    // otherwise it will account for the camera position/rotation and calculate the tile position
    public Vector3Int MousePosToCell() {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        Vector3 position = Input.mousePosition; // get mouse position
        Vector3 worldPoint = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(position); // create a raycast from the mouse position
        RaycastHit hit;

        // if a raycast hits a tile, use that position
        if(Physics.Raycast(ray,out hit)) {
            worldPoint = new Vector3(hit.point.x,0,hit.point.z);
        } else if(plane.Raycast(ray, out float enter)) {    // otherwise cast a ray at the flat plane to get position
            Vector3 hitPoint = ray.GetPoint(enter);
            worldPoint = new Vector3(hitPoint.x,0,hitPoint.z);
        }

        return tilemap.WorldToCell(worldPoint);
    }

    // Converts a cell (tile/grid) position to world position, adding the elevation if a tile exists at that position
    public Vector3 CellToWorld(Vector3Int position) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        Vector3 worldPosition = tilemap.CellToWorld(position);
        HexTile tile = tilemap.GetTile(position) as HexTile;
        if(tile != null) { // if tile exists, add elevation to world position
            return new Vector3(worldPosition.x,worldPosition.y+tileHeight+((int)tile.elevation*elevationHeight),worldPosition.z);
        }
        return worldPosition;
    }

    // Returns true if a HexTile instance exists at position, false otherwise
    public bool HasHexTile(Vector3Int position) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        return tilemap.GetTile(position) is HexTile;
    }

    // Returns the HexTile instance at position, can be null
    public HexTile GetHexTile(Vector3Int position) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        return tilemap.GetTile(position) as HexTile;
    }

    // Returns the elevation at a position, returns lowest elevation if tile is null
    public Elevation GetElevation(Vector3Int position) {
        HexTile tile = GetHexTile(position);
        return tile != null ? tile.elevation : Elevation.Low;
    }

    // Returns a list of attributes of the tile at a position, or empty list if tile doesn't exist
    public List<TileAttribute> GetTileAttributes(Vector3Int position) {
        HexTile tile = GetHexTile(position);
        return tile != null ? tile.attributes : new List<TileAttribute>();
    }

    // returns true if the tile at position contains the given attribute, false otherwise
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
