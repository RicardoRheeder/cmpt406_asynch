using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Tilemap))]
public class TilemapEditor : Editor {
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(Tilemap tilemap, GizmoType gizmoType){
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.yellow;  
        foreach(Vector3Int position in tilemap.cellBounds.allPositionsWithin) {
            HexTile tile = tilemap.GetTile(position) as HexTile;
            if(tile != null && tile.attributes.Contains(TileAttribute.Obstacle)) {
                Vector3 worldPosition = tilemap.CellToWorld(position);
                Handles.Label(new Vector3(worldPosition.x,worldPosition.y,worldPosition.z - (tile.GetElevationMultiplier() * (int)tile.elevation)), "O", style);
            }
        }
    }
}
