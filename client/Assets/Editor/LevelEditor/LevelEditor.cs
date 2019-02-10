using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

enum LevelEditorAction {
    None,
    Place,
    Erase,
}

public class LevelEditor : EditorWindow {

    LevelEditorAction currentAction = LevelEditorAction.None;
    Tilemap tilemap;
    TileBase customTile;
    List<GameObject> tileObjects = new List<GameObject>();

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow() {
        GetWindow<LevelEditor>("Level Editor");
    }

    void OnGUI() {
        GUILayout.Button("Save");

        DrawUILine(Color.gray);

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Place")) {
            currentAction = (currentAction == LevelEditorAction.Place) ? LevelEditorAction.None : LevelEditorAction.Place;
            Debug.Log("Place button clicked. CurrentAction:" + currentAction.ToString());
        }
        if(GUILayout.Button("Erase")) {
            currentAction = (currentAction == LevelEditorAction.Erase) ? LevelEditorAction.None : LevelEditorAction.Erase;
            Debug.Log("Erase button clicked. CurrentAction:" + currentAction.ToString());
        }
        EditorGUILayout.EndHorizontal();

        DrawUILine(Color.gray);

        if(GUILayout.Button("Clear All")) {
            if(tilemap != null) {
                tilemap.ClearAllTiles();
            }
        }

        DrawUILine(Color.gray);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Grid");
        tilemap = EditorGUILayout.ObjectField(tilemap, typeof(Tilemap),true) as Tilemap;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Tile");
        customTile = EditorGUILayout.ObjectField(customTile, typeof(TileBase),true) as TileBase;
        EditorGUILayout.EndHorizontal();
    }

    void OnFocus() {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;

        // TODO: might be better to replace this with physics overlap if colliders are added to tiles
        if(tileObjects.Count == 0) {
            tileObjects.AddRange(FindObjectsOfType(typeof(GameObject)) as GameObject[]);
        }
        if(tilemap != null) {
            Selection.activeObject = tilemap;
        }
    }

    void OnSceneGUI(SceneView sceneView) {
        if(currentAction == LevelEditorAction.Place) {
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, 500, 500), MouseCursor.ArrowPlus);
        } else if (currentAction == LevelEditorAction.Erase) {
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, 500, 500), MouseCursor.ArrowMinus);
        }

        Event e = Event.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        switch(e.GetTypeForControl(controlId)) {
            case EventType.MouseDown:
                // Debug.Log("mouseclick on scene");
                if(currentAction == LevelEditorAction.Place) {
                    CreateTile();
                } else if (currentAction == LevelEditorAction.Erase) {
                    EraseTile();
                }
                break;
        }
    }

    void Save() {
        // saves level as a prefab
    }

    void CreateTile() {
        if(tilemap == null) {
            return;
        }

        Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 mousePosition = mouseRay.GetPoint(0f);
        Vector3Int newPosition = tilemap.WorldToCell(mousePosition);
        HexTile randTile = ScriptableObject.CreateInstance<HexTile>();
        randTile.elevation = Elevation.High;
        tilemap.SetTile(newPosition,randTile);
    }

    void EraseTile() {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 mousePosition = mouseRay.GetPoint(0f);
    }

    static void DrawUILine(Color color, int thickness = 2, int padding = 10) {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y += padding/2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    void OnDestroy() {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }
}
