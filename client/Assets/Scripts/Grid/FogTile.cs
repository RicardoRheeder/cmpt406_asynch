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
    public float transparency = 1f;
    float fogChangeAmount = 0.3f;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
        if(go != null) {
            go.transform.rotation = Quaternion.Euler(-90,0,0);
            tileObject = go;
            go.transform.localScale = new Vector3(1.01f,5,1.01f);
            RefreshColor();
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

    void RefreshColor() {
        if(tileObject == null) {
            return;
        }

        MeshRenderer rend = tileObject.GetComponent<MeshRenderer>();
            if(rend != null) {
                Color color = rend.material.color;
                rend.material.color = new Color(color.r, color.g, color.b, transparency);
            }
    }

    public override void RefreshTile(Vector3Int location, ITilemap tilemap) {
        List<Vector2Int> neighbours = HexUtility.GetNeighborPositions((Vector2Int)location);
        // float lowestTransparency = 1f;
        int count = 0;
        foreach(Vector3Int neighbour in neighbours) {
            TileBase fogNeighbour = tilemap.GetTile(neighbour);
            if(fogNeighbour == null) {
                count++;
            }
            // if(fogNeighbour == null) {
            //     Debug.Log(neighbour.ToString() + " is null");
            //     // lowestTransparency = 0;
            // } else if((fogNeighbour as FogTile).transparency < lowestTransparency){
            //     Debug.Log("lower transparency");
            //     lowestTransparency = (fogNeighbour as FogTile).transparency;
            // }
        }
        transparency = (count >= 4) ? fogChangeAmount : 1f;
        if(transparency > 1f) {
            transparency = 1f;
        }
        RefreshColor();
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
