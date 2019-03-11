using System.Collections.Generic;

//Must be > 0
public enum BoardType {
    //small maps from 1 to 99
    AlphaChannel = 1,
    SnakeValley = 2,
    TheEye = 3,

    //medium maps from 100 to 199
    Lowlands = 100,
    Pinnacle = 101,
    Valley = 102,

    //large maps from 200 to 299
    Wheel = 200,

    //Random test maps
    SampleMap = 301,
    Sandbox = 420
}

public static class BoardMetadata {

    public static readonly int TEST_BOARD_LIMIT = 300;

    public static Dictionary<BoardType, int> MaxPlayersDict = new Dictionary<BoardType, int>() {
        { BoardType.AlphaChannel, 2 },
        { BoardType.SnakeValley, 2 },
        { BoardType.TheEye, 2 },

        { BoardType.Pinnacle, 2 },
        { BoardType.Valley, 4 },
        { BoardType.Lowlands, 4 },

        { BoardType.Wheel, 6 },
    };

    public static Dictionary<BoardType, int> CostDict = new Dictionary<BoardType, int>() {
        { BoardType.AlphaChannel, 12 },
        { BoardType.SnakeValley, 25 },
        { BoardType.TheEye, 25 },

        { BoardType.Pinnacle, 20 },
        { BoardType.Valley, 10 },
        { BoardType.Lowlands, 16 },

        { BoardType.Wheel, 40 },
    };

    public static Dictionary<BoardType, string> BoardSceneNames = new Dictionary<BoardType, string>() {
        { BoardType.AlphaChannel, "Map_Small_AlphaChannel" },
        { BoardType.SnakeValley, "Map_Small_SnakeValley" },
        { BoardType.TheEye, "Map_Small_TheEye" },

        { BoardType.Pinnacle, "Map_Med_Pinnacle" },
        { BoardType.Valley, "Map_Med_Valley" },
        { BoardType.Lowlands, "Map_Med_Lowlands" },

        { BoardType.Wheel, "Map_Large_Wheel" },


        { BoardType.SampleMap, "SampleMap" },
        { BoardType.Sandbox, "Sandbox" },
    };

    public static Dictionary<BoardType, string> BoardDisplayNames = new Dictionary<BoardType, string>() {
        { BoardType.AlphaChannel, "Alpha Channel" },
        { BoardType.SnakeValley, "Snake Valley" },
        { BoardType.TheEye, "The Eye" },

        { BoardType.Pinnacle, "Pinnacle" },
        { BoardType.Valley, "Valley" },
        { BoardType.Lowlands, "Lowlands" },

        { BoardType.Wheel, "Wheel" },
    };

    public static Dictionary<string, BoardType> BoardDisplayNamesReverse = new Dictionary<string, BoardType>() {
        { "Alpha Channel", BoardType.AlphaChannel },
        { "Snake Valley", BoardType.SnakeValley },
        { "The Eye", BoardType.TheEye },

        { "Pinnacle", BoardType.Pinnacle },
        { "Valley", BoardType.Valley },
        { "Lowlands", BoardType.Lowlands },

        { "Wheel", BoardType.Wheel },
    };
}
