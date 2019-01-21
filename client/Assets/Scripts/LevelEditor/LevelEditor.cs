using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

enum LevelEditorAction {
    None,
    Place,
    Erase,
}

public class LevelEditor : EditorWindow {

    LevelEditorAction currentAction = LevelEditorAction.None;
    Grid grid;

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow() {
        GetWindow<LevelEditor>("Level Editor");
    }

    void OnGUI() {
        GUILayout.Button("Save");

        DrawUILine(Color.gray);
        if(GUILayout.Button("Create")) {
            currentAction = (currentAction == LevelEditorAction.Place) ? LevelEditorAction.None : LevelEditorAction.Place;
            Debug.Log("Create button clicked. CurrentAction:" + currentAction.ToString());
        }
        DrawUILine(Color.gray);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Grid");
        grid = EditorGUILayout.ObjectField(grid, typeof(Grid),true) as Grid;
        EditorGUILayout.EndHorizontal();
    }

    void OnFocus() {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView) {
        Event e = Event.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        switch(e.GetTypeForControl(controlId)) {
            case EventType.MouseDown:
                Debug.Log("mouseclick on scene");
                if(currentAction == LevelEditorAction.Place) {
                    CreateTile();
                }
                break;
        }
    }

    void Save() {
        // saves level as a prefab
    }

    void CreateTile() {
        if(grid == null) {
            return;
        }

        GameObject tile = PrefabUtility.InstantiatePrefab(Resources.LoadAll("")[0]) as GameObject;
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 mousePosition = mouseRay.GetPoint(0f);
        tile.transform.parent = grid.transform;
        tile.transform.position = grid.CellToWorld(grid.WorldToCell(mousePosition));
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
