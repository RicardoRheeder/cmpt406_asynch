using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

// [CustomEditor(typeof(HexBrush))]
public class HexBrushEditor : GridBrushEditor {

    private HexBrush lineBrush { get { return target as HexBrush; } }
        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing){
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
            PaintPreview(grid, brushTarget, position.min);
        }

}
