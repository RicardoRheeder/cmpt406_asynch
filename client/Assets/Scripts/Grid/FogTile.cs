using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum FogState { Visible, Edge, Cleared, MapEdgeVisible }

public class FogTile : TileBase {
    [HideInInspector]
    public GameObject tileModel;
    public Sprite spritePreview;

    GameObject tileObject;
    ParticleSystem particleSystem;
    public float transparency = 1f;
    float fogChangeAmount = 0.3f;

    FogState fogState = FogState.Visible;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
        if(go != null) {
            go.transform.rotation = Quaternion.Euler(-90,0,0);
            go.transform.localScale = new Vector3(1.01f,5,1.01f);
            tileObject = go;
            particleSystem = tileObject.GetComponent<ParticleSystem>();
            if(fogState == FogState.Cleared) {
                particleSystem.Stop();
            } else if(fogState == FogState.Edge || fogState == FogState.MapEdgeVisible) {
                particleSystem.Play();
            }
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

    public void ClearFog() {
        transparency = 0f;
        fogState = FogState.Cleared;
        particleSystem.Stop();
    }

    public void ShowFog(bool isMapEdge) {
        transparency = 1f;
        fogState = isMapEdge ? FogState.MapEdgeVisible : FogState.Visible;
    }

    public void SetAsEdge() {
        transparency = 0.5f;
        fogState = FogState.Edge;
        particleSystem.Play();
    }

    public FogState GetFogState() {
        return fogState;
    }

    public override void RefreshTile(Vector3Int location, ITilemap tilemap) {
        // TODO: transparency changes
        // transparency = 1f;
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
}
