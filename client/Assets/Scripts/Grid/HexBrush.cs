using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace UnityEditor {
    [CreateAssetMenu(fileName = "Hex brush", menuName = "Brushes/Hex brush")]
    [CustomGridBrush(false, true, false, "Hex Brush")]
    public class HexBrush : GridBrush {

        public Elevation currentElevation;
        public GameObject model;
        private GameObject prev_brushTarget;
        
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position) {
            // Do not allow editing palettes
            if (brushTarget != null && brushTarget.layer == 31) {
                return;
            }
            
            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            if(tilemap == null) {
                return;
            }
            
            HexTile tile = ScriptableObject.CreateInstance<HexTile>();
            SerializedObject serialTile = new SerializedObject(tile);
            serialTile.FindProperty("elevation").intValue = (int)currentElevation;
            serialTile.FindProperty("tileModel").objectReferenceValue = model;
            serialTile.ApplyModifiedProperties();
            Vector3Int newPos = new Vector3Int(position.x,position.y, (int)tile.Elevation);
            tilemap.SetTile(newPos,serialTile.targetObject as HexTile);
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            base.Erase(gridLayout,brushTarget,position);
        }

        public override void Rotate(GridBrushBase.RotationDirection direction, GridLayout.CellLayout layout) {

        }
    }

    [CustomEditor(typeof(HexBrush))]
    public class HexBrushEditor : GridBrushEditor
    {
        private HexBrush hexBrush { get { return target as HexBrush; } }

        private SerializedObject m_SerializedObject;

        protected override void OnEnable() {
            base.OnEnable();
            m_SerializedObject = new SerializedObject(target);
        }

        public override void OnPaintInspectorGUI() {
            m_SerializedObject.UpdateIfRequiredOrScript();
            hexBrush.model = EditorGUILayout.ObjectField(hexBrush.model, typeof(GameObject), true) as GameObject;
            hexBrush.currentElevation = (Elevation)EditorGUILayout.EnumPopup("Elevation: ", hexBrush.currentElevation);
            m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}