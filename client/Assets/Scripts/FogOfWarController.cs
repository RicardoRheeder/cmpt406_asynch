using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class FogOfWarController {

    GameObject fogObject;
    Tilemap fogTilemap;
    Tilemap tilemap;

    Dictionary<Vector2Int,List<FogViewer>> clearedTiles = new Dictionary<Vector2Int, List<FogViewer>>();
    Dictionary<Vector2Int,FogTile> mapEdgeTiles = new Dictionary<Vector2Int, FogTile>();
    Dictionary<Vector2Int,List<FogViewer>> edgeTiles = new Dictionary<Vector2Int, List<FogViewer>>();
    List<FogViewer> viewers = new List<FogViewer>();

    public void InitializeFogOfWar(Tilemap tilemap) {
        if(tilemap == null) {
            Debug.Log("tilemap was null. Fog of war not initialized");
            return;
        }

        this.tilemap = tilemap;
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

        for(int i=0; i<viewer.GetAffectedTiles().Count;i++) {
            clearedTiles.TryGetValue(viewer.GetAffectedTiles()[i], out List<FogViewer> tileList);
            if(tileList != null && tileList.Count > 0) {
                tileList.Remove(viewer);
                clearedTiles[viewer.GetAffectedTiles()[i]] = tileList;
            } 
            if(tileList == null || tileList.Count == 0) {
                clearedTiles.Remove(viewer.GetAffectedTiles()[i]);
                FogTile tile = fogTilemap.GetTile((Vector3Int)viewer.GetAffectedTiles()[i]) as FogTile;
                mapEdgeTiles.TryGetValue(viewer.GetAffectedTiles()[i], out FogTile mapEdgeTile);
                tile.ShowFog(mapEdgeTile != null);
                fogTilemap.RefreshTile((Vector3Int)viewer.GetAffectedTiles()[i]);
                continue;
            }
        }

        for(int i=0; i<viewer.GetEdgeTiles().Count;i++) {
            edgeTiles.TryGetValue(viewer.GetEdgeTiles()[i], out List<FogViewer> edgeTileList);
            if (edgeTileList != null && edgeTileList.Count > 0) {
                edgeTileList.Remove(viewer);
                edgeTiles[viewer.GetEdgeTiles()[i]] = edgeTileList;
            }
            if(edgeTiles == null || edgeTiles.Count == 0) {
                edgeTiles.Remove(viewer.GetEdgeTiles()[i]);
                FogTile tile = fogTilemap.GetTile((Vector3Int)viewer.GetEdgeTiles()[i]) as FogTile;
                mapEdgeTiles.TryGetValue(viewer.GetEdgeTiles()[i], out FogTile mapEdgeTile);
                tile.ShowFog(mapEdgeTile != null);
                fogTilemap.RefreshTile((Vector3Int)viewer.GetEdgeTiles()[i]);
                continue;
            }
        }

        // clear new tiles
        Tuple<List<Vector2Int>,List<Vector2Int>> tiles = HexUtility.FindTilesInVision(viewer.GetPosition(),viewer.GetRadius(),tilemap,false);
        List<Vector2Int> clearTiles = tiles.First;
        List<Vector2Int> viewerEdgeTiles = tiles.Second;
        List<Vector2Int> newTiles = new List<Vector2Int>();
        for(int i=0; i < clearTiles.Count; i++) {
            FogTile tile = fogTilemap.GetTile((Vector3Int)clearTiles[i]) as FogTile;
            if(tile != null) {
                tile.ClearFog();
                fogTilemap.RefreshTile((Vector3Int)clearTiles[i]);
                newTiles.Add(clearTiles[i]);
                clearedTiles.TryGetValue(clearTiles[i],out List<FogViewer> tileList);
                if(tileList == null || tileList.Count == 0) {
                    tileList = new List<FogViewer>();
                } 
                tileList.Add(viewer);
                clearedTiles[clearTiles[i]] = tileList;
            }
        }
        viewer.SetAffectedTiles(newTiles);
        viewer.SetEdgeTiles(new List<Vector2Int>());
        for(int i=0; i < viewerEdgeTiles.Count; i++) {
            clearedTiles.TryGetValue(viewerEdgeTiles[i],out List<FogViewer> tileList);
            edgeTiles.TryGetValue(viewerEdgeTiles[i],out List<FogViewer> edgeTileList);
            FogTile tile = fogTilemap.GetTile((Vector3Int)viewerEdgeTiles[i]) as FogTile;
            if((tileList == null || tileList.Count == 0) && tile != null) {
                tile.SetAsEdge();
                fogTilemap.RefreshTile((Vector3Int)viewerEdgeTiles[i]);
                if(edgeTileList == null || edgeTileList.Count == 0) {
                    edgeTileList = new List<FogViewer>();
                } 
                edgeTileList.Add(viewer);
                edgeTiles[viewerEdgeTiles[i]] = edgeTileList;
                viewer.GetEdgeTiles().Add(viewerEdgeTiles[i]);
            }
        }
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

    public void DeleteFogAtPosition(Vector2Int position) {
        if(fogTilemap == null) {
            return;
        }

        if(fogTilemap.HasTile((Vector3Int)position)) {
            fogTilemap.SetTile((Vector3Int)position,null);
            fogTilemap.RefreshTile((Vector3Int)position);
            List<Vector2Int> neighbors = HexUtility.GetNeighborPositions(position);
            for(int i=0; i<neighbors.Count; i++) {
                FogTile neighbor = fogTilemap.GetTile((Vector3Int)neighbors[i]) as FogTile;
                if(neighbor != null && !mapEdgeTiles.ContainsKey(neighbors[i])) {
                    mapEdgeTiles.Add(neighbors[i],neighbor);
                    if(neighbor.GetFogState() != FogState.Cleared) {
                        neighbor.SetAsMapEdge();
                    }
                    fogTilemap.RefreshTile((Vector3Int)neighbors[i]);
                }
            }
        }
    }

    public void DeleteFogAtSpawnPoint(SpawnPoint spawnPoint, ref BoardController board) {
        List<Vector2Int> spawnTileList = new List<Vector2Int>();
        Tilemap tilemap = board.GetTilemap();
        foreach(Vector2Int pos in tilemap.cellBounds.allPositionsWithin) {
            HexTile tile = tilemap.GetTile((Vector3Int)pos) as HexTile;
            if(tile != null && tile.spawnPoint == spawnPoint) {
                DeleteFogAtPosition(pos);
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

    public bool CheckIfTileHasFog(Vector2Int pos) {
        return !clearedTiles.ContainsKey(pos);
    }

}
