﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

public class BoardController {

    public float tileHeight = 4.5f; // the height of the tile model. used to calculate the y world position
    public float elevationHeight = 2.4f; // the difference between each elevation level. used to calculate the y world position
    Tilemap tilemap; // the tilemap instance
    Plane plane;
    LineRenderer pathLine;

    private List<GameObject> hightlightedTiles;
    private List<GameObject> hoverHighlightedTiles = new List<GameObject>();
    private Tuple<Vector2Int,int> previousHoverTilePos;
    private Tuple<Vector2Int,Vector2Int> renderPathCache;

    private GameObject fogPlane;

    // Initializes the board controller. Must be called before other methods can function
    public void Initialize() {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        if(tilemap == null) {
            Debug.Log("Tilemap is null. This will result in problems");
        }
        else {
            plane = new Plane(tilemap.transform.forward, tilemap.transform.position); // creates a flat horizontal plane at y = 0
        }
        this.hightlightedTiles = new List<GameObject>();
        this.pathLine = GameObject.Instantiate(Resources.Load<LineRenderer>("PathLine"));
        
        if (fogPlane != null) {
            return;
        }
        fogPlane = GameObject.Find("Fog Plane");
        int randInt = UnityEngine.Random.Range(0, 2);

        if (randInt == 1) {
            GameObject.Find("RainParent").transform.GetChild(0).gameObject.SetActive(true);
            fogPlane.SetActive(true);
            fogPlane.GetComponent<FogSystem>().setMistColor(2);
        }
        else if (randInt == 2) {
            fogPlane.SetActive(true);
            fogPlane.GetComponent<FogSystem>().setMistColor((int)UnityEngine.Random.Range(0,1));
        }
        else {
            fogPlane.SetActive(false);
        }
    }

    // Getter for direct access to the tilemap. Use other board controller methods unless otherwise needed
    public Tilemap GetTilemap() {
        return tilemap;
    }

    // Converts a world position to a cell (tile/grid) position
    public Vector2Int WorldToCell(Vector3 position) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }
        return (Vector2Int)tilemap.WorldToCell(position);
    }

    // Converts the mouse position to a cell position
    // Fires a raycast and returns the position of the tile it hits,
    // otherwise it will account for the camera position/rotation and calculate the tile position
    public Vector2Int MousePosToCell() {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        Vector3 position = Input.mousePosition; // get mouse position
        Vector3 worldPoint = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(position); // create a raycast from the mouse position

        // if a raycast hits a tile, use that position
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            worldPoint = new Vector3(hit.point.x, hit.point.y, 0);
        }
        else if (plane.Raycast(ray, out float enter)) {    // otherwise cast a ray at the flat plane to get position
            Vector3 hitPoint = ray.GetPoint(enter);
            Debug.DrawRay(ray.origin, ray.direction * enter, Color.green);
            worldPoint = new Vector3(hitPoint.x, hitPoint.y, 0);
        }

        return (Vector2Int)tilemap.WorldToCell(worldPoint);
    }

    // Converts a cell (tile/grid) position to world position, adding the elevation if a tile exists at that position
    public Vector3 CellToWorld(Vector2Int position) {
        return CellToWorld(position, 0f);
    }

    // Converts a cell (tile/grid) position to world position, adding the elevation if a tile exists at that position
    public Vector3 CellToWorld(Vector2Int position, float zOffset) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        Vector3 worldPosition = tilemap.CellToWorld((Vector3Int)position);
        HexTile tile = tilemap.GetTile((Vector3Int)position) as HexTile;
        if(tile != null) { // if tile exists, add elevation to world position
            return new Vector3(worldPosition.x,worldPosition.y,worldPosition.z-(tileHeight+((int)tile.elevation*elevationHeight)+zOffset));
        }
        return worldPosition;
    }

    // Returns true if a HexTile instance exists at position, false otherwise
    public bool HasHexTile(Vector2Int position) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        return tilemap.GetTile((Vector3Int)position) is HexTile;
    }

    // Returns the HexTile instance at position, can be null
    public HexTile GetHexTile(Vector2Int position) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        return tilemap.GetTile((Vector3Int)position) as HexTile;
    }

    // Returns the elevation at a position, returns lowest elevation if tile is null
    public Elevation GetElevation(Vector2Int position) {
        HexTile tile = GetHexTile(position);
        return tile != null ? tile.elevation : Elevation.Low;
    }

    public Vector2Int GetCenterSpawnTile(SpawnPoint player) {
        if (tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }
        List<Vector2Int> spawnTileList = new List<Vector2Int>();
        foreach(Vector2Int tilePos in tilemap.cellBounds.allPositionsWithin) {
            HexTile tile = tilemap.GetTile((Vector3Int)tilePos) as HexTile;
            if (tile != null && tile.spawnPoint == player) {
                spawnTileList.Add(tilePos);
            }
        }
        return spawnTileList.Count > 0 ? spawnTileList[spawnTileList.Count / 2] : Vector2Int.zero;
    }

    public void HighlightSpawnZone(SpawnPoint player) {
        if (tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }
        List<Vector2Int> spawnTileList = new List<Vector2Int>();
        foreach (HexTile tile in tilemap.GetTilesBlock(tilemap.cellBounds)) {
            if (tile != null && tile.spawnPoint == player) {
                spawnTileList.Add(WorldToCell(tile.GetTileObject().transform.position));
            }
        }
        HighlightTiles(spawnTileList);
    }
    
    //This function highlights a list of tiles. And disables the previous highlighted ones
    public void HighlightTiles(List<Vector2Int> tilePositions) {
        if (tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }
        ClearHighlighting();

        foreach (var tilePosition in tilePositions) {
            GameObject tileObject;
            if (this.HasHexTile(tilePosition)) {
                HexTile tile = this.GetHexTile(tilePosition); //get the Hex tile using Vector2Int position
                tileObject = tile.GetTileObject(); //get the tile game object 

                if(!IsOutlineComponentAttached(tileObject)) {
                    AttachOutlineComponent(tileObject);
                }
                hightlightedTiles.Add(tileObject);
                TileOutline tileOutline = tileObject.GetComponent<TileOutline>();
                tileOutline.enabled = true;
                if(tileOutline.outlineMode == OutlineMode.Hover) {
                    tileOutline.outlineMode = OutlineMode.HoverOverHighlight;
                } else {
                    tileOutline.outlineMode = OutlineMode.Highlight;
                }
            }
        }
    }

    //Checks if mouse position is changed to a new tile
    private bool IsMousePositionChanged(Tuple<Vector2Int,int> tilePosition) {
        if (previousHoverTilePos != null && tilePosition.Equals(previousHoverTilePos)) {
            return false;
        } else {
            previousHoverTilePos = tilePosition;
            return true;
        }
    }

   //Checks if the Outline script component is attached to the tile object
    private bool IsOutlineComponentAttached(GameObject tileObject) {
        return tileObject.GetComponent<TileOutline>() != null;
    }

    //Attatch the Outline script component to the tile object if it is not
    private void AttachOutlineComponent(GameObject tileObject) {
        if (!IsOutlineComponentAttached(tileObject)) {
            tileObject.AddComponent<TileOutline>();
        }
    }

    //This function highlights tiles on mouse over and disables when mouse leaves the tile. It does not work on tile already highlighted -- but it
    //should should the mouse over effect on already highligted tiles
    public void HoverHighlight(List<Vector2Int> tilePositions, Vector2Int centerTile) {
        if(tilePositions == null || tilePositions.Count == 0) {
            return;
        }
        //this is to check if cursor is moved, we dont want to keep checking if its in the same position
        if (IsMousePositionChanged(new Tuple<Vector2Int,int>(centerTile,tilePositions.Count))) {
            for(int i=0; i<hoverHighlightedTiles.Count; i++) {
                TileOutline tileOutline = hoverHighlightedTiles[i].GetComponent<TileOutline>();
                if(tileOutline.outlineMode == OutlineMode.Hover) {
                    tileOutline.enabled = false;
                    tileOutline.outlineMode = OutlineMode.None;
                } else if (tileOutline.outlineMode == OutlineMode.HoverOverHighlight) {
                    tileOutline.outlineMode = OutlineMode.Highlight;
                }
            }

            GameObject tileObject;
            hoverHighlightedTiles.Clear();

            for(int i=0; i<tilePositions.Count;i++) {
                if (this.HasHexTile(tilePositions[i])) {
                    HexTile tile = this.GetHexTile(tilePositions[i]); //get the Hex tile using Vector2Int position
                    tileObject = tile.GetTileObject(); //get the tile game object 
                    if (!IsOutlineComponentAttached(tileObject)) {//outline component not attached then attach it
                        AttachOutlineComponent(tileObject);
                    }
                    //if they only have the outline component but are not highlighted than save into the hoverHighlightedTile and enable the highlight
                    hoverHighlightedTiles.Add(tileObject);
                    TileOutline tileOutline = tileObject.GetComponent<TileOutline>();
                    tileOutline.enabled = true;
                    tileOutline.hoverColor = 2;
                    tileOutline.hoverOverHighlightColor = 1;
                    if(tileOutline.outlineMode == OutlineMode.Highlight) {
                        tileOutline.outlineMode = OutlineMode.HoverOverHighlight;
                    } else {
                        tileOutline.outlineMode = OutlineMode.Hover;
                    }
                }
            }
            previousHoverTilePos = new Tuple<Vector2Int,int>(centerTile,tilePositions.Count);
        }
    }

    public void ClearHighlighting() {
        foreach (var tile in hightlightedTiles) {
            TileOutline tileOutline = tile.GetComponent<TileOutline>();
            tileOutline.enabled = false;
            tileOutline.outlineMode = OutlineMode.None;
        }
        hightlightedTiles.Clear();
    }

    public bool CellIsSpawnTile(SpawnPoint player, Vector2Int pos) {
        if (tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }
        if (HasHexTile(pos)) {
            return GetHexTile(pos).spawnPoint == player;
        }
        return false;
    }

    // Returns a list of attributes of the tile at a position, or empty list if tile doesn't exist
    public List<TileAttribute> GetTileAttributes(Vector2Int position) {
        HexTile tile = GetHexTile(position);
        return tile != null ? tile.attributes : new List<TileAttribute>();
    }

    // returns true if the tile at position contains the given attribute, false otherwise
    public bool CheckIfAttributeExists(Vector2Int position, TileAttribute attribute) {
        List<TileAttribute> attributes = GetTileAttributes(position);
        for(int i = 0; i < attributes.Count; i++) {
            if(attributes[i] == attribute) {
                return true;
            }
        }
        return false;
    }

    public List<Vector2Int> GetTilesWithinAttackRange(Vector2Int startingPos, int range) {
        return HexUtility.GetTilePositionsInRange(tilemap,startingPos,range);
    }

    public List<Vector2Int> GetTilesWithinMovementRange(Vector2Int startingPos, int movementSpeed) {
        return HexUtility.HexReachable(startingPos, movementSpeed, tilemap, false);
    }

    public void RenderPath(Vector2Int startingPos, Vector2Int endPos) {
        Tuple<Vector2Int,Vector2Int> pathEndPositions = new Tuple<Vector2Int, Vector2Int>(startingPos,endPos);
        if(renderPathCache != null && renderPathCache.Equals(pathEndPositions)) {
            return;
        }
        List<Vector2Int> path = HexUtility.Pathfinding(startingPos,endPos,this.tilemap,false);
        List<Vector3> worldPosPath = new List<Vector3>();
        worldPosPath.Add(CellToWorld(startingPos,1f));
        foreach(Vector2Int pos in path) {
            worldPosPath.Add(CellToWorld(pos,1f));
        }
        pathLine.positionCount = worldPosPath.Count;
        pathLine.SetPositions(worldPosPath.ToArray());
        pathLine.endWidth = 0.5f;
        renderPathCache = pathEndPositions;
    }

    public void ClearRenderedPath() {
        pathLine.positionCount = 0;
        pathLine.SetPositions(new Vector3[0]);
        renderPathCache = null;
    }
}
