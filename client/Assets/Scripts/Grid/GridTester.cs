﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum GridTestType { 
    HasTile, 
    Pathfinding,
    GetTile 
}

/*
    Temporary class that tests grid functionality
 */
public class GridTester : MonoBehaviour {

    public GridTestType currentTestType = GridTestType.HasTile;

    Vector2Int currTilePosition;
    BoardController boardController;

    void Start() {
        boardController = new BoardController();
        boardController.Initialize();
        currTilePosition = boardController.WorldToCell(transform.position);
        transform.position = boardController.CellToWorld(currTilePosition);
        transform.rotation = Quaternion.Euler(-90,0,0);
        Debug.Log("Starting tile position: " + currTilePosition);
    }

    void Update() {
       if(Input.GetMouseButtonDown(0)) {
           switch(currentTestType) {
                case GridTestType.HasTile:
                    CheckHasTile();
                    break;
                case GridTestType.Pathfinding:
                    TestPathfinding();
                    break;
                case GridTestType.GetTile:
                    TestGetTile();
                    break;
                default:
                    break;
            }
       } 
    }

    void CheckHasTile() {
        Vector2Int cellPosition = boardController.MousePosToCell();
        Debug.Log("hasHexTile: " + boardController.HasHexTile(cellPosition) + " " + cellPosition.ToString());
    }

    void TestGetTile() {
        Vector2Int cellPosition = boardController.MousePosToCell();
        HexTile tile = boardController.GetHexTile(cellPosition);
        if(tile != null) {
            Debug.Log(tile.GetTileObject());
        }
    }

    void TestPathfinding() {
        Vector2Int endPosition = boardController.MousePosToCell();
        Debug.Log("mouse: " + Input.mousePosition.ToString() + " cell: " + endPosition.ToString());
        List<Vector2Int> path = HexUtility.Pathfinding(currTilePosition,endPosition,boardController.GetTilemap(),false);
        StartCoroutine(PathMovement(path, 5f));
    }

    IEnumerator PathMovement(List<Vector2Int> path, float speed) {
         float step = speed * Time.fixedDeltaTime;
         float t = 0;
         Vector3 prevPos = transform.position;
         for(int i = 0; i < path.Count; i++) {
            currTilePosition = path[i];
            Vector3 worldPos = boardController.CellToWorld(currTilePosition);
            t = 0;
            while (t <= 1.0f) {
                t += step;
                transform.position = Vector3.Lerp(prevPos, worldPos, t);
                yield return new WaitForFixedUpdate();
            }
            prevPos = worldPos;
            transform.position = worldPos;
        }
    }
}