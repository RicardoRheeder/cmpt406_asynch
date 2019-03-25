using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnMetadata {

    public static Dictionary<SpawnPoint, float> SpawnColours = new Dictionary<SpawnPoint, float>() {
        {SpawnPoint.Player1, 0.15f },
        {SpawnPoint.Player2, 0.3f },
        {SpawnPoint.Player3, 0.45f },
        {SpawnPoint.Player4, 0.6f },
        {SpawnPoint.Player5, 0.75f },
        {SpawnPoint.Player6, 0.9f }
    };
}
