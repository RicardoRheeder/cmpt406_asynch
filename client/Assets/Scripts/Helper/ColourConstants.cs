using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColourConstants {

    public static Color BUTTON_ACTIVE = Color.gray;
    public static Color BUTTON_DEFAULT = new Color(0.247f, 0.247f, 0.247f);
    public static Color BUTTON_INACTIVE = Color.black;

    public static Dictionary<SpawnPoint, Color> SpawnColours = new Dictionary<SpawnPoint, Color>() {
        {SpawnPoint.Player1, Color.HSVToRGB(0.0f, 1.0f, 1.0f) },
        {SpawnPoint.Player2, Color.HSVToRGB(0.67f, 1.0f, 1.0f) },
        {SpawnPoint.Player3, Color.HSVToRGB(0.33f, 1.0f, 1.0f) },
        {SpawnPoint.Player4, Color.HSVToRGB(0.13f, 1.0f, 1.0f) },
        {SpawnPoint.Player5, Color.HSVToRGB(0.85f, 1.0f, 1.0f) },
        {SpawnPoint.Player6, Color.HSVToRGB(0.5f, 1.0f, 1.0f) } 
    };
}
