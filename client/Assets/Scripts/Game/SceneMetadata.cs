using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneMetadata {
    public static Dictionary<BoardType, string> BoardNames = new Dictionary<BoardType, string>() {
        {BoardType.first, "someSceneName" }
    };
}
