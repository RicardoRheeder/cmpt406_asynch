using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HexTile: TileBase {

    public GameObject tileModel;
    public Sprite spritePreview;

    [SerializeField]
    Elevation elevation;
    [SerializeField]
    List<TileAttribute> attribute = new List<TileAttribute>();

    public Elevation Elevation { get; set; }

    public TileAttribute Attribute { get; set; }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
        if(go) {
           go.transform.rotation = Quaternion.Euler(90,0,0); 
        }
        return base.StartUp(position,tilemap,go);
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
