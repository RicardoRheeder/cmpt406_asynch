using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWarController : MonoBehaviour {

    public GameObject fogObject;
    public bool generateFog = true;
    Tilemap fogTilemap;
    Tilemap baseTilemap;

    BoardController boardController;

    void Start() {
        Tilemap tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        if(generateFog) {
            InitializeFogOfWar(tilemap);
        }
        
        boardController = new BoardController();
        boardController.Initialize();
    }

    void Update() {
        if(Input.GetMouseButton(0)) {
            ClearFogAtPosition(boardController.MousePosToCell());
        }
    }

    public void InitializeFogOfWar(Tilemap tilemap) {
        if(tilemap == null) {
            return;
        }

        if(fogTilemap == null) {
            GameObject newTilemap = new GameObject();
            newTilemap.AddComponent(typeof(Tilemap));
            newTilemap.AddComponent(typeof(TilemapRenderer));
            newTilemap.name = "FogTilemap";
            fogTilemap = newTilemap.GetComponent<Tilemap>();
        }
        fogTilemap.ClearAllTiles();
        fogTilemap.transform.SetParent(tilemap.transform.parent);
        fogTilemap.transform.SetPositionAndRotation(tilemap.transform.position,tilemap.transform.rotation);

        foreach(Vector3Int pos in tilemap.cellBounds.allPositionsWithin) {
            if(tilemap.HasTile(pos)) {
                FogTile fogTile = ScriptableObject.CreateInstance<FogTile>();
                fogTile.tileModel = fogObject;
                fogTilemap.SetTile(pos, fogTile);
            }
        }
        fogTilemap.RefreshAllTiles();
    }

    public void ClearFogAtPosition(Vector2Int position) {
        if(fogTilemap == null) {
            return;
        }
        Debug.Log("clear fog at " + position.ToString());
        fogTilemap.SetTile((Vector3Int)position, null);
        List<Vector2Int> neighbors = HexUtility.GetNeighborPositions(position);
        foreach(Vector3Int neighbor in neighbors) {
            FogTile tile = fogTilemap.GetTile(neighbor) as FogTile;
            if(tile != null) {
                tile.transparency = 0.5f;
                fogTilemap.RefreshTile(neighbor);
            }
        }
    }

    public void ClearFogInArea(List<Vector2Int> positions) {
        if(fogTilemap == null) {
            return;
        }

        for(int i=0; i<positions.Count; i++) {
            fogTilemap.SetTile((Vector3Int)positions[i], null);
        }
    }

    public void ClearAllFog() {
        if(fogTilemap == null) {
            return;
        }

        fogTilemap.ClearAllTiles();
    }

}
