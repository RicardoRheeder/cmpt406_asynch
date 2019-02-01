using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomGridBrush(true, false, true, "Hex Brush")]
public class HexBrush : GridBrushBase {

    public Elevation currentElevation;
    
    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position) {
        Debug.Log("paint " + brushTarget.ToString());
        base.Paint(grid, brushTarget, position);
    }
    

    public override void Rotate(GridBrushBase.RotationDirection direction, GridLayout.CellLayout layout) {

    }
}
