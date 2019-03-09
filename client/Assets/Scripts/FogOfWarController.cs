using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class FogOfWarController {

    GameObject fogObject;
    Tilemap fogTilemap;

    Dictionary<Vector2Int,FogTile> clearedTiles = new Dictionary<Vector2Int, FogTile>();
    Dictionary<Vector2Int,FogTile> mapEdgeTiles = new Dictionary<Vector2Int, FogTile>(); // TODO: fill this in during init
    List<FogViewer> viewers = new List<FogViewer>();

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
                if(HexUtility.IsEdgeTile((Vector2Int)pos,tilemap)) {
                    mapEdgeTiles.Add((Vector2Int)pos,fogTile);
                    fogTile.ShowFog(true);
                }
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
            mapEdgeTiles.TryGetValue(oldTiles[i], out FogTile madEdgeTile);
            tile.ShowFog(madEdgeTile != null);
            fogTilemap.RefreshTile((Vector3Int)oldTiles[i]); // might need to refresh this at a later point
        }
    }

    public void UpdateFogAtViewer(FogViewer viewer) {
        if(viewer == null || fogTilemap == null) {
            return;
        }

        //fill in any old cleared tiles
        List<Vector2Int> oldTiles = new List<Vector2Int>();
        oldTiles.AddRange(viewer.GetAffectedTiles());
        oldTiles.AddRange(viewer.GetEdgeTiles());
        for(int i=0; i < oldTiles.Count; i++) {
            FogTile tile = fogTilemap.GetTile((Vector3Int)oldTiles[i]) as FogTile;
            mapEdgeTiles.TryGetValue(oldTiles[i], out FogTile madEdgeTile);
            tile.ShowFog(madEdgeTile != null);
            fogTilemap.RefreshTile((Vector3Int)oldTiles[i]); // might need to refresh this at a later point
            clearedTiles.Remove(oldTiles[i]);
        }

        // clear new tiles
        List<Vector2Int> tiles = HexUtility.GetTilePositionsInRange(fogTilemap,viewer.GetPosition(),viewer.GetRadius());
        List<Vector2Int> newTiles = new List<Vector2Int>();
        for(int i=0; i < tiles.Count; i++) {
            FogTile tile = fogTilemap.GetTile((Vector3Int)tiles[i]) as FogTile;
            clearedTiles.TryGetValue(tiles[i],out FogTile clearedTile);
            if(tile != null && clearedTile == null) {
                tile.ClearFog();
                fogTilemap.RefreshTile((Vector3Int)tiles[i]);
                newTiles.Add(tiles[i]);
                clearedTiles.Add(tiles[i],tile);
            }
        }
        viewer.SetAffectedTiles(newTiles);

        List<Vector2Int> edgePositions = HexUtility.FindRing(viewer.GetPosition(),viewer.GetRadius()+1,1);
        List<Vector2Int> edgeTiles = new List<Vector2Int>();
        for(int i=0; i < edgePositions.Count; i++) {
            clearedTiles.TryGetValue(edgePositions[i],out FogTile edgeTile);
            FogTile tile = fogTilemap.GetTile((Vector3Int)edgePositions[i]) as FogTile;
            if(edgeTile == null && tile != null) {
                tile.SetAsEdge();
                fogTilemap.RefreshTile((Vector3Int)edgePositions[i]);
                edgeTiles.Add(edgePositions[i]);
            }
        }
        viewer.SetEdgeTiles(edgeTiles);
    }

    public void ClearFogAtPosition(Vector2Int position) {
        if(fogTilemap == null) {
            return;
        }
        
        if(fogTilemap.HasTile((Vector3Int)position)) {
            FogViewer newViewer = new FogViewer();
            newViewer.SetPosition(position);
            newViewer.SetRadius(0);
            viewers.Add(newViewer);
            fogTilemap.RefreshTile((Vector3Int)position);
        }
        
    }

    public void ClearFogAtSpawnPoint(SpawnPoint spawnPoint, ref BoardController board) {
        List<Vector2Int> spawnTileList = new List<Vector2Int>();
        Tilemap tilemap = board.GetTilemap();
        foreach(Vector2Int pos in tilemap.cellBounds.allPositionsWithin) {
            HexTile tile = tilemap.GetTile((Vector3Int)pos) as HexTile;
            if(tile != null && tile.spawnPoint == spawnPoint) {
                ClearFogAtPosition(pos);
            }
        }
    }

    public void DeleteFogInArea(List<Vector2Int> positions) {
        if(fogTilemap == null) {
            return;
        }

        for(int i=0; i<positions.Count; i++) {
            ClearFogAtPosition(positions[i]);
        }
    }

    public void DeleteAllFog() {
        if(fogTilemap == null) {
            return;
        }

        fogTilemap.ClearAllTiles();
    }

}
