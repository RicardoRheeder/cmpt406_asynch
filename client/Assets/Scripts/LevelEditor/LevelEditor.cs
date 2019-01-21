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

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow() {
        GetWindow<LevelEditor>("Level Editor");
    }

    void OnGUI() {
        GUILayout.Button("Save");
        GUILayout.Button("Load");

        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.EndHorizontal();
    }

    void Save() {

    }

    void Load() {

    }
}
