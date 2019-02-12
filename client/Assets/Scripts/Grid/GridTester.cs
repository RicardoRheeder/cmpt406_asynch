using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum GridTestType { 
    HasTile, 
    Pathfinding 
}

/*
    Temporary class that tests grid functionality
 */
public class GridTester : MonoBehaviour {

    public Tilemap tilemap;
    public GridTestType currentTestType = GridTestType.HasTile;

    Vector3Int currTilePosition;

    void Start() {
        if(tilemap == null) {
            return;
        }

        currTilePosition = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.CellToWorld(currTilePosition);
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
                default:
                    break;
            }
       } 
    }

    void CheckHasTile() {
        if(!tilemap) {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
    }

    void TestPathfinding() {
        if(!tilemap) {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int endPosition = tilemap.WorldToCell(mousePosition);

        List<Vector3Int> path = HexUtility.Pathfinding(currTilePosition,endPosition,tilemap,false);
        StartCoroutine(PathMovement(path, 5f));
    }

    IEnumerator PathMovement(List<Vector3Int> path, float speed) {
         float step = speed * Time.fixedDeltaTime;
         float t = 0;
         Vector3 prevPos = transform.position;
         for(int i = 0; i < path.Count; i++) {
            currTilePosition = path[i];
            Vector3 worldPos = tilemap.CellToWorld(currTilePosition);
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
