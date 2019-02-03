using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Hex brush", menuName = "Brushes/Hex brush")]
[CustomGridBrush(false, true, false, "Hex Brush")]
public class HexBrush : GridBrushBase {

    public Elevation currentElevation;
    public GameObject model;
    
    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position) {
        // Do not allow editing palettes
        if (brushTarget.layer == 31)
            return;
        
        Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
        if(tilemap == null) {
            return;
        }
        
        Debug.Log("hex paint");
        HexTile tile = ScriptableObject.CreateInstance<HexTile>();
        tile.Elevation = Elevation.High;
        tile.tileModel = model;
        Debug.Log("is tile gameObject null? " + tile.Elevation.ToString());
        tilemap.SetTile(position,tile);

        if(tile.tileObject != null) {
            tile.tileObject.transform.position = new Vector3(tile.tileObject.transform.position.x,tile.tileObject.transform.position.y, (tile.tileObject.transform.position.z - (2.5f) * (int)tile.Elevation));
        }

        Undo.RecordObject(tile, "HexTile");

        TileBase theTile = tilemap.GetTile(position);
        Debug.Log("actual tile elevation: " + (theTile != null ? (theTile as HexTile).Elevation.ToString() : null));
    }

    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
        base.Erase(gridLayout,brushTarget,position);
    }

    public override void Rotate(GridBrushBase.RotationDirection direction, GridLayout.CellLayout layout) {

    }
}
