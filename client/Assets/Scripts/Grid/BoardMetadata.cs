using System.Collections.Generic;

//Must be > 0
public enum BoardType {
    //small maps from 0 to 99
    Hawkpoint = 1,
	Ravine = 2,

    //medium maps from 100 to 199
    Pinnacle = 100,
    Valley = 101,

    //large maps from 200 to 299

    //Random test map
    SampleMap = 102
}

public static class BoardMetadata {
    public static Dictionary<BoardType, int> MaxPlayersDict = new Dictionary<BoardType, int>() {
        { BoardType.Ravine, 2},
		{ BoardType.Hawkpoint, 3 },
        { BoardType.Pinnacle, 4 },
        { BoardType.Valley, 4 },
        { BoardType.SampleMap, 6 }
    };

    public static Dictionary<BoardType, int> CostDict = new Dictionary<BoardType, int>() {
        { BoardType.Hawkpoint, 15 },
		{ BoardType.Ravine, 15 },
        { BoardType.Pinnacle, 25 },
        { BoardType.Valley, 25 },
        { BoardType.SampleMap, 40 }
    };

    public static Dictionary<BoardType, string> BoardNames = new Dictionary<BoardType, string>() {
        { BoardType.Hawkpoint, "Map_Small_Hawkpoint" },
		{ BoardType.Ravine, "Map_Small_Ravine" },
        { BoardType.Pinnacle, "Map_Med_Pinnacle" },
        { BoardType.Valley, "Map_Med_Valley" },
        { BoardType.SampleMap, "SampleMap" }
    };
}
