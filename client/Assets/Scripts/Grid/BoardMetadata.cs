using System.Collections.Generic;

//Must be > 0
public enum BoardType {
    //small maps from 1 to 99
    Hawkpoint = 1,
    Ravine = 2,
    AlphaChannel = 3,
    SnakeValley = 4,

    //medium maps from 100 to 199
    Pinnacle = 100,
    Valley = 101,
    Lowlands = 102,

    //large maps from 200 to 299

    //Random test maps
    SampleMap = 301,
    Sandbox = 420
}

public static class BoardMetadata {

    public static readonly int TEST_BOARD_LIMIT = 300;

    public static Dictionary<BoardType, int> MaxPlayersDict = new Dictionary<BoardType, int>() {
        { BoardType.Ravine, 2},
        { BoardType.AlphaChannel, 2 },
        { BoardType.SnakeValley, 2 },
        { BoardType.Hawkpoint, 3 },
        { BoardType.Pinnacle, 4 },
        { BoardType.Valley, 4 },
        { BoardType.Lowlands, 4 },
    };

    public static Dictionary<BoardType, int> CostDict = new Dictionary<BoardType, int>() {
        { BoardType.Hawkpoint, 25 },
        { BoardType.AlphaChannel, 25 },
        { BoardType.SnakeValley, 25 },
        { BoardType.Ravine, 25 },
        { BoardType.Pinnacle, 25 },
        { BoardType.Valley, 25 },
        { BoardType.Lowlands, 25 },
    };

    public static Dictionary<BoardType, string> BoardSceneNames = new Dictionary<BoardType, string>() {
        { BoardType.Hawkpoint, "Map_Small_Hawkpoint" },
        { BoardType.AlphaChannel, "Map_Small_AlphaChannel" },
        { BoardType.Ravine, "Map_Small_Ravine" },
        { BoardType.SnakeValley, "Map_Small_SnakeValley" },
        { BoardType.Pinnacle, "Map_Med_Pinnacle" },
        { BoardType.Valley, "Map_Med_Valley" },
        { BoardType.Lowlands, "Map_Med_Lowlands" },


        { BoardType.SampleMap, "SampleMap" },
        { BoardType.Sandbox, "Sandbox" },
    };

    public static Dictionary<BoardType, string> BoardDisplayNames = new Dictionary<BoardType, string>() {
        { BoardType.Hawkpoint, "Hawkpoint" },
        { BoardType.AlphaChannel, "Alpha Channel" },
        { BoardType.Ravine, "Ravine" },
        { BoardType.Pinnacle, "Pinnacle" },
        { BoardType.Valley, "Valley" },
        { BoardType.Lowlands, "Lowlands" },
        { BoardType.SnakeValley, "Snake Valley" },
    };

    public static Dictionary<string, BoardType> BoardDisplayNamesReverse = new Dictionary<string, BoardType>() {
        { "Hawkpoint", BoardType.Hawkpoint },
        { "Alpha Channel", BoardType.AlphaChannel },
        { "Ravine", BoardType.Ravine },
        { "Pinnacle", BoardType.Pinnacle },
        { "Valley", BoardType.Valley },
        { "Lowlands", BoardType.Lowlands },
        { "Snake Valley", BoardType.SnakeValley },
    };
}
