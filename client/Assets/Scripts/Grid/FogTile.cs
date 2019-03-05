using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FogTile : TileBase {
    [HideInInspector]
    public GameObject tileModel;
    public Sprite spritePreview;

    GameObject tileObject;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
        if(go) {
            Debug.Log("go");
            go.transform.rotation = Quaternion.Euler(0,0,0);
            tileObject = go;
            go.transform.localScale = new Vector3(1,5,1);
        }

        #if UNITY_EDITOR
        if (go != null){
            if (go.scene.name == null) {
                Debug.Log("DestroyImmediate");
                DestroyImmediate(go);
            }
        }
        #endif

        return true;
    }

    public override void RefreshTile(Vector3Int location, ITilemap tilemap) {
        base.RefreshTile(location,tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = spritePreview; // Do I need a sprite for it to be detected?
        tileData.colliderType = Tile.ColliderType.Grid;
        tileData.flags = TileFlags.None;
        tileData.gameObject = tileModel;
    }

    public GameObject GetTileObject() {
        return tileObject;
    }

    #if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a HexTile Asset
    [MenuItem("Assets/Create/HexTile")]
    public static void CreateFogTile(){
        string path = EditorUtility.SaveFilePanelInProject("Save Fog Tile", "New Fog Tile", "Asset", "Save Fog Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FogTile>(), path);
    }
    #endif
}
