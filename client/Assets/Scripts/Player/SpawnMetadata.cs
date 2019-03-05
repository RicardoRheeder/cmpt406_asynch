using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnMetadata {

    public static Dictionary<SpawnPoint, Color> SpawnColours = new Dictionary<SpawnPoint, Color>() {
        {SpawnPoint.Player1, Color.red },
        {SpawnPoint.Player2, Color.blue },
        {SpawnPoint.Player3, Color.black },
        {SpawnPoint.Player4, Color.yellow },
        {SpawnPoint.Player5, Color.cyan },
        {SpawnPoint.Player6, new Color(0.5f, 0, 0.5f) }
    };
}
