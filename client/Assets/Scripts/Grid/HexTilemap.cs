using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTilemap : MonoBehaviour {

    Grid grid;
    Dictionary<Vector3Int,HexTile> worldTiles = new Dictionary<Vector3Int, HexTile>();

    void Awake() {
        grid = GetComponent<Grid>();
        FindWorldTiles();
    }

    public Grid Grid { get; }

    public bool HasTile(Vector3Int pos) {
        return worldTiles.ContainsKey(pos);
    }

    public bool HasTileAtElevation(Vector3Int pos, Elevation el) {
        HexTile tile = null;
        worldTiles.TryGetValue(pos, out tile);
        return tile != null && tile.Elevation == el;
    }

    public HexTile GetTile(Vector3Int pos) {
        HexTile tile = null;
        worldTiles.TryGetValue(pos, out tile);
        return tile;
    }

    public HexTile GetTileAtElevation(Vector3Int pos, Elevation el) {
        HexTile tile = null;
        worldTiles.TryGetValue(pos, out tile);
        return (tile != null && tile.Elevation == el) ? tile : null;
    }

    public Dictionary<Vector3Int,HexTile> Tiles { get; }

    private void FindWorldTiles() {
        worldTiles.Clear();
        HexTile[] tiles = GetComponentsInChildren<HexTile>();
        for(int i=0; i < tiles.Length; i++) {
            // worldTiles.Add(grid.WorldToCell(tiles[i].position),tiles[i]);
        }
    }
    
}
