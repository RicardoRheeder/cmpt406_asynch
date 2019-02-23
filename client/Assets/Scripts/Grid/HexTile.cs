using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class HexTile: TileBase {

    [HideInInspector]
    public GameObject tileModel;
    public Sprite spritePreview;

    [SerializeField]
    [HideInInspector]
    public Elevation elevation;
    [SerializeField]
    [HideInInspector]
    public List<TileAttribute> attributes = new List<TileAttribute>();

    GameObject tileObject;

    const float ELEVATION_MULTIPLIER = 2.5f;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
        if(go) {
            go.transform.rotation = Quaternion.Euler(0,0,0);
            go.transform.position = new Vector3(go.transform.position.x,(go.transform.position.y + (ELEVATION_MULTIPLIER) * (int)elevation),go.transform.position.z);
            tileObject = go;
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
    public static void CreateHexTile(){
        string path = EditorUtility.SaveFilePanelInProject("Save Hex Tile", "New Hex Tile", "Asset", "Save Hex Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<HexTile>(), path);
    }
    #endif

}
