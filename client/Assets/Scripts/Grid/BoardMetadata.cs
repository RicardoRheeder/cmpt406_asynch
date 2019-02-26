using System.Collections.Generic;

public static class BoardMetadata {
    public static Dictionary<BoardType, int> MaxPlayersDict = new Dictionary<BoardType, int>() {
        { BoardType.SampleMap, 1 },
        { BoardType.TestMap, 3 },
        { BoardType.ThirdMap, 5 }
    };

    public static Dictionary<BoardType, int> CostDict = new Dictionary<BoardType, int>() {
        { BoardType.SampleMap, 1000 },
        { BoardType.TestMap, 2000 },
        { BoardType.ThirdMap, 3000 },
    };
}
