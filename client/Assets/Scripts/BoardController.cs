﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class BoardController {

    public float tileHeight = 4.5f; // the height of the tile model. used to calculate the y world position
    public float elevationHeight = 2.4f; // the difference between each elevation level. used to calculate the y world position
    Tilemap tilemap; // the tilemap instance
    Plane plane;

    private List<GameObject> hightlightedTiles;
    private GameObject hoverHighlightedTile;
    private GameObject alreadyHighlightedTile;
    private GameObject singleHighlitedTile;
    private int alreadyHighlightedTileColor; 
    private Vector2Int previousHoverTilePos;
    
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
        
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            GameObject.Find("RainParent").transform.GetChild(0).gameObject.SetActive(true);
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
        RaycastHit hit;

        // if a raycast hits a tile, use that position
        if(Physics.Raycast(ray,out hit)) {
            worldPoint = new Vector3(hit.point.x,hit.point.y,0);
        } else if(plane.Raycast(ray, out float enter)) {    // otherwise cast a ray at the flat plane to get position
            Vector3 hitPoint = ray.GetPoint(enter);
            Debug.DrawRay(ray.origin,ray.direction * enter,Color.green);
            worldPoint = new Vector3(hitPoint.x,hitPoint.y,0);
        }

        return (Vector2Int)tilemap.WorldToCell(worldPoint);
    }

    // Converts a cell (tile/grid) position to world position, adding the elevation if a tile exists at that position
    public Vector3 CellToWorld(Vector2Int position) {
        if(tilemap == null) {   // throw exception if tilemap is null
            throw new MissingComponentException("Tilemap is missing");
        }

        Vector3 worldPosition = tilemap.CellToWorld((Vector3Int)position);
        HexTile tile = tilemap.GetTile((Vector3Int)position) as HexTile;
        if(tile != null) { // if tile exists, add elevation to world position
            return new Vector3(worldPosition.x,worldPosition.y,worldPosition.z-(tileHeight+((int)tile.elevation*elevationHeight)));
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

                if (tileObject.GetComponent<cakeslice.Outline>() == null) {
                    tileObject.AddComponent<cakeslice.Outline>();
                }
                hightlightedTiles.Add(tileObject);
                tileObject.GetComponent<cakeslice.Outline>().enabled = true;
            }
        }
    }

    //Checks if mouse position is changed to a new tile
    private bool IsMousePositionChanged(Vector2Int tilePosition) {
        if (tilePosition.Equals(previousHoverTilePos)) {
            return false;
        } else {
            previousHoverTilePos = tilePosition;
            return true;
        }
    }

   //Checks if the Outline script component is attached to the tile object
    private bool IsOutlineComponentAttached(GameObject tileObject) {
        return tileObject.GetComponent<cakeslice.Outline>() != null;
    }

    //Checks if the Outline script component is enabled on the tile object
    private bool IsOutlineComponentEnabled(GameObject tileObject) {
        if (!IsOutlineComponentAttached(tileObject)) {
            throw new MissingComponentException("Outline Component is missing");
        }
        return tileObject.GetComponent<cakeslice.Outline>().enabled;
    }

    //Attatch the Outline script component to the tile object if it is not
    private void AttachOutlineComponent(GameObject tileObject) {
        if (!IsOutlineComponentAttached(tileObject)) {
            tileObject.AddComponent<cakeslice.Outline>();
        }
    }

    //Enables the Outline script component on the object and changes the colo
    private void EnableOutlineComponentAndChangeColor(GameObject tileObject, int colorNum) {
        tileObject.GetComponent<cakeslice.Outline>().enabled = true;
        tileObject.GetComponent<cakeslice.Outline>().color = colorNum; 
    }

    //Highlight the tile object selected unit is on and disable the previous one
   public void HighlightSingleTile(Vector2Int tilePosition) {
       if (this.HasHexTile(tilePosition)) {
           GameObject tileObject;

           if (singleHighlitedTile != null) {
               singleHighlitedTile.GetComponent<cakeslice.Outline>().enabled = false;
               singleHighlitedTile.GetComponent<cakeslice.Outline>().color = 0;
           }

           HexTile tile = this.GetHexTile(tilePosition); //get the Hex tile using Vector2Int position
           tileObject = tile.GetTileObject(); //get the tile game object 

           if (IsOutlineComponentAttached(tileObject)) {
               if (IsOutlineComponentEnabled(tileObject)) {
                   tileObject.GetComponent<cakeslice.Outline>().color = 2;
               }
               else {
                   EnableOutlineComponentAndChangeColor(tileObject, 2);
               }
           }
           else {
               AttachOutlineComponent(tileObject);
               EnableOutlineComponentAndChangeColor(tileObject, 2);
           }
           singleHighlitedTile = tileObject;
       }
   }

    //This function highlights tiles on mouse over and disables when mouse leaves the tile. It does not work on tile already highlighted -- but it
    //should should the mouse over effect on already highligted tiles
    public void HoverHighlight(Vector2Int tilePosition) {
        //this is to check if cursor is moved, we dont want to keep checking if its in the same position
        if (IsMousePositionChanged(tilePosition)) {
            if (hoverHighlightedTile != null) {//the first time this is false
                if (hoverHighlightedTile.GetComponent<cakeslice.Outline>().color != 2) {
                    hoverHighlightedTile.GetComponent<cakeslice.Outline>().enabled = false;
                    hoverHighlightedTile.GetComponent<cakeslice.Outline>().color = 0;
                }
            }

            if (alreadyHighlightedTile != null) {
                alreadyHighlightedTile.GetComponent<cakeslice.Outline>().color = alreadyHighlightedTileColor;
            }

            //previousHoverTile = tilePosition;
            GameObject tileObject;

            if (this.HasHexTile(tilePosition)) {
                HexTile tile = this.GetHexTile(tilePosition); //get the Hex tile using Vector2Int position
                tileObject = tile.GetTileObject(); //get the tile game object 

                //this checks for tiles that are already highlited 
                if (IsOutlineComponentAttached(tileObject)) {
                    if (IsOutlineComponentEnabled(tileObject)) {
                        alreadyHighlightedTile = tileObject;
                        alreadyHighlightedTileColor = alreadyHighlightedTile.GetComponent<cakeslice.Outline>().color;
                        alreadyHighlightedTile.GetComponent<cakeslice.Outline>().color = 1;
                        return; //if they are highlighted than return
                    }

                    //if they only have the outline component but are not highlighted than save into the hoverHighlightedTile and enable the highlight
                    hoverHighlightedTile = tileObject;
                    //only change the color if the tile color is not 2 otherwise leave tile color as is for the unit selected on that tile
                    if (hoverHighlightedTile.GetComponent<cakeslice.Outline>().color != 2) {
                        EnableOutlineComponentAndChangeColor(hoverHighlightedTile, 1);
                    }
                }
                //this is for if its a tile that is not already higlighted and does not have the outline component
                else {
                    hoverHighlightedTile = tileObject;
                    if (!IsOutlineComponentAttached(hoverHighlightedTile)) {//outline component not attached then attach it
                        AttachOutlineComponent(hoverHighlightedTile);
                    }
                    //only change the color if the tile color is not 2 otherwise leave tile color as is for the unit selected on that tile
                    if (hoverHighlightedTile.GetComponent<cakeslice.Outline>().color != 2) {
                        EnableOutlineComponentAndChangeColor(hoverHighlightedTile, 1);
                    }
                }
            }
        }
    }

    //leave this here for now
    //This function highligts tiles on mouse over and disables when mouse leaves the tile. It does not work on tile already highlighted -- but it
    //should should the mouse over effect on already highligted tiles
    //public void HoverHighlight(Vector2Int tilePosition)
    //{
    //    if (!tilePosition.Equals(previousHoverTile)){ //this is to check if cursor is moved, we dont want to keep checking if its in the same position
    //        if (hoverHighlightedTile != null){ //the first time this is false
    //            hoverHighlightedTile.GetComponent<cakeslice.Outline>().enabled = false;
    //        }

    //        previousHoverTile = tilePosition;
    //        GameObject tileObject;

    //        if (this.HasHexTile(tilePosition)){
    //            HexTile tile = this.GetHexTile(tilePosition); //get the Hex tile using Vector2Int position
    //            tileObject = tile.GetTileObject(); //get the tile game object 
                
    //            //this checks for tiles that are already highlited 
    //            if (tileObject.GetComponentsInChildren<cakeslice.Outline>().Length > 0){ 
    //                if (tileObject.GetComponentsInChildren<cakeslice.Outline>()[0].enabled == true){ 

    //                    return; //if they are highlighted than return
    //                }
    //                //if they only have the outline component but are not highlighted than save into the hoverHighlightedTile and enable the highlight
    //                hoverHighlightedTile = tileObject;
    //                hoverHighlightedTile.GetComponent<cakeslice.Outline>().enabled = true;
    //                tileObject.GetComponent<cakeslice.Outline>().color = 1; //change the color to yellow
    //            }
    //            //this is for if its a tile that is not already higlighted and does not have the outline component
    //            else {
    //                hoverHighlightedTile = tileObject;
    //                if (hoverHighlightedTile.GetComponentsInChildren<cakeslice.Outline>().Length <= 0){
    //                        hoverHighlightedTile.AddComponent<cakeslice.Outline>();
    //                }
    //                hoverHighlightedTile.GetComponent<cakeslice.Outline>().enabled = true;
    //                tileObject.GetComponent<cakeslice.Outline>().color = 1;
    //            }
    //        }
    //    }
    //}


    public void ClearHighlighting() {
        foreach (var tile in hightlightedTiles) {
            tile.GetComponent<cakeslice.Outline>().enabled = false;
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
        return HexUtility.HexReachable(startingPos, range, tilemap, true);
    }

    public List<Vector2Int> GetTilesWithinMovementRange(Vector2Int startingPos, int movementSpeed) {
        return HexUtility.HexReachable(startingPos, movementSpeed, tilemap, false);
    }
}
