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
        GameObject tile = PrefabUtility.InstantiatePrefab(Resources.LoadAll("")[0]) as GameObject;
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
        mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
        mousePosition.y = -mousePosition.y;
        tile.transform.position = mousePosition;
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
