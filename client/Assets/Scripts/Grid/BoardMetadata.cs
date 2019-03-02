using System.Collections.Generic;

//Must be > 0
public enum BoardType {
    //small maps from 0 to 99
    Hawkpoint = 1,

    //medium maps from 100 to 199
    Pinnacle = 100,
    Valley = 101,

    //large maps from 200 to 299
}

public static class BoardMetadata {
    public static Dictionary<BoardType, int> MaxPlayersDict = new Dictionary<BoardType, int>() {
        { BoardType.Hawkpoint, 3 },
        { BoardType.Pinnacle, 4 },
        { BoardType.Valley, 4 }
    };

    public static Dictionary<BoardType, int> CostDict = new Dictionary<BoardType, int>() {
        { BoardType.Hawkpoint, 15 },
        { BoardType.Pinnacle, 25 },
        { BoardType.Valley, 25 },
    };

    public static Dictionary<BoardType, string> BoardNames = new Dictionary<BoardType, string>() {
        { BoardType.Hawkpoint, "Map_Small_Hawkpoint" },
        { BoardType.Pinnacle, "Map_Med_Pinnacle" },
        { BoardType.Valley, "Map_Med_Valley" }
    };
}
