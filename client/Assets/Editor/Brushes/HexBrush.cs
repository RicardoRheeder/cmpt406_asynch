using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace UnityEditor {
    [CustomGridBrush(false, false, false, "Hex Brush")]
    public class HexBrush : GridBrush {

        public Elevation currentElevation; // the elevation to place a tile at
        public GameObject model; // the tile prefab to place
        public SpawnPoint spawnPoint; // set if the tile is a spawn point
        public List<TileAttribute> attributes = new List<TileAttribute>();
        
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position) {
            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            // Do not allow editing palettes, or if no tilemap exists
            if(tilemap == null || brushTarget.layer == 31) {
                return;
            }

            if(model.GetComponent<HexTileObject>() == null) {
                throw new MissingComponentException("Tile must have HexTileObject attached");
            }
            
            //create a new instance of a HexTile
            HexTile tile = ScriptableObject.CreateInstance<HexTile>();
            //create a new serialized object from new tile instance
            SerializedObject serializedTile = new SerializedObject(tile);
            //set values for the serialized object
            serializedTile.FindProperty("tileModel").objectReferenceValue = model;
            serializedTile.FindProperty("elevation").intValue = (int)currentElevation;
            serializedTile.FindProperty("spawnPoint").intValue = (int)spawnPoint;
            SerializeAttributes(serializedTile.FindProperty("attributes"));
            //apply the modified properties so that unity saves the instance
            serializedTile.ApplyModifiedProperties();
            //add the tile at current paint position
            tilemap.SetTile(position,serializedTile.targetObject as HexTile);
        }

        private void SerializeAttributes(SerializedProperty arr) {
            if(arr == null || !arr.isArray) {
                return;
            }

            arr.ClearArray();
            for(int i = 0; i < attributes.Count; i++) {
                arr.InsertArrayElementAtIndex(i);
                arr.GetArrayElementAtIndex(i).intValue = (int)attributes[i];
            }
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            // TODO erase stacked tiles
            base.Erase(gridLayout,brushTarget,position);
        }

        public override void Rotate(GridBrushBase.RotationDirection direction, GridLayout.CellLayout layout) {
            // TODO allow rotation
        }
    }

    [CustomEditor(typeof(HexBrush))]
    public class HexBrushEditor : GridBrushEditor {
        
        private HexBrush hexBrush { get { return target as HexBrush; } }

        private SerializedObject serializedBrush;
        private SerializedObject serializedAttributeList;

        protected override void OnEnable() {
            base.OnEnable();
            serializedBrush = new SerializedObject(target);
        }

        public override void OnPaintInspectorGUI() {
            // get the latest representation of the brush
            serializedBrush.UpdateIfRequiredOrScript();
            GameObject model = EditorGUILayout.ObjectField(hexBrush.model, typeof(GameObject), true) as GameObject;
            if(model.GetComponent<HexTileObject>() == null || PrefabUtility.GetPrefabAssetType(model) == PrefabAssetType.NotAPrefab) {
                model = null;
                throw new UnityException("MAP EDITOR: tile must be a prefab with HexTileObject");
            } else {
                hexBrush.model = model;
            }
            hexBrush.currentElevation = (Elevation)EditorGUILayout.EnumPopup("Elevation: ", hexBrush.currentElevation);
            hexBrush.spawnPoint = (SpawnPoint)EditorGUILayout.EnumPopup("Spawn Point: ", hexBrush.spawnPoint);
            for (int i = 0; i < hexBrush.attributes.Count; i++) {
                hexBrush.attributes[i] = (TileAttribute)EditorGUILayout.EnumPopup("Attribute " + i + ": ", hexBrush.attributes[i]);
            }
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("+ attribute")) {
                hexBrush.attributes.Add(TileAttribute.Normal);
            }
            if(GUILayout.Button("- attribute") && hexBrush.attributes.Count > 0) {
                hexBrush.attributes.RemoveAt(hexBrush.attributes.Count - 1);
            }
            GUILayout.EndHorizontal();
            
            //save any changes to the brush
            serializedBrush.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}