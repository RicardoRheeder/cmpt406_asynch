using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class FogOfWarController {

    GameObject fogObject;
    Tilemap fogTilemap;
    BoardController boardController;

    Dictionary<Vector2Int,FogTile> clearedTiles;
    Dictionary<Vector2Int,FogTile> mapEdgeTiles; // TODO: fill this in during init
    List<FogViewer> viewers;

    public void InitializeFogOfWar(Tilemap tilemap) {
        if(tilemap == null) {
            Debug.Log("tilemap was null. Fog of war not initialized");
            return;
        }

        fogObject = Resources.Load<GameObject>("Fog/FogTile");

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
            //TODO: determine if it is on edge of map or not
            if(tilemap.HasTile(pos)) {
                FogTile fogTile = ScriptableObject.CreateInstance<FogTile>();
                fogTile.tileModel = fogObject;
                fogTilemap.SetTile(pos, fogTile);
            }
        }
        fogTilemap.RefreshAllTiles();
    }

    public void UpdateAllFog() {
        for(int i = 0; i<viewers.Count; i++) {
            UpdateFogAtViewer(viewers[i]);
        }
    }

    public void AddFogViewer(FogViewer viewer) {
        viewers.Add(viewer);
        viewer.SetFogUpdateMethod(UpdateFogAtViewer);
        UpdateFogAtViewer(viewer);
    }

    public void RemoveFogViewer(FogViewer viewer) {
        viewers.Remove(viewer);
        List<Vector2Int> oldTiles = viewer.GetAffectedTiles();
        for(int i=0; i < oldTiles.Count; i++) {
            FogTile tile = fogTilemap.GetTile((Vector3Int)oldTiles[i]) as FogTile;
            tile.ShowFog();
            fogTilemap.RefreshTile((Vector3Int)oldTiles[i]); // might need to refresh this at a later point
        }
    }

    public void UpdateFogAtViewer(FogViewer viewer) {
        if(viewer == null || fogTilemap == null) {
            return;
        }

        //fill in any old cleared tiles
        List<Vector2Int> oldTiles = viewer.GetAffectedTiles();
        for(int i=0; i < oldTiles.Count; i++) {
            FogTile tile = fogTilemap.GetTile((Vector3Int)oldTiles[i]) as FogTile;
            tile.ShowFog();
            fogTilemap.RefreshTile((Vector3Int)oldTiles[i]); // might need to refresh this at a later point
        }

        // clear new tiles
        List<Vector2Int> tiles = HexUtility.GetTilePositionsInRange(fogTilemap,viewer.GetPosition(),viewer.GetRadius());
        List<Vector2Int> newTiles = new List<Vector2Int>();
        for(int i=0; i < tiles.Count; i++) {
            FogTile tile = fogTilemap.GetTile((Vector3Int)tiles[i]) as FogTile;
            if(tile != null) {
                tile.ClearFog();
                fogTilemap.RefreshTile((Vector3Int)tiles[i]);
                newTiles.Add(tiles[i]);
            }
        }
        viewer.SetAffectedTiles(newTiles);

        List<Vector2Int> edgePositions = HexUtility.FindRing(viewer.GetPosition(),viewer.GetRadius()+1,1);
        for(int i=0; i < edgePositions.Count; i++) {
            clearedTiles.TryGetValue(edgePositions[i],out FogTile edgeTile);
            FogTile tile = fogTilemap.GetTile((Vector3Int)edgePositions[i]) as FogTile;
            if(edgeTile == null && tile != null) {
                tile.SetAsEdge();
            }
        }
    }

    public void ClearFogAtPosition(Vector2Int position) {
        if(fogTilemap == null) {
            return;
        }
        FogTile fog = fogTilemap.GetTile((Vector3Int)position) as FogTile;
        if(fog != null) {
            fog.ClearFog();
            fogTilemap.RefreshTile((Vector3Int)position);
        }
    }

    public void DeleteFogInArea(List<Vector2Int> positions) {
        if(fogTilemap == null) {
            return;
        }

        for(int i=0; i<positions.Count; i++) {
            fogTilemap.SetTile((Vector3Int)positions[i], null);
        }
    }

    public void DeleteAllFog() {
        if(fogTilemap == null) {
            return;
        }

        fogTilemap.ClearAllTiles();
    }

}
