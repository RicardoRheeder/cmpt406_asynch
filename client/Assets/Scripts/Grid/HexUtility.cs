using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public static class HexUtility {

    public const int ELEVATION_REACHABLE_DISTANCE = 2;

    // Converts a vector3Int position from oddr (coordinate system used for tilemap in the game)
    // to a cube coordinate. This is done to make algorithms easier and more efficient
    public static Vector3Int OddrToCube(Vector2Int hex) {
        int x = hex.x - (hex.y - (hex.y&1)) / 2;
        int z = hex.y;
        int y = -x-z;
        return new Vector3Int(x, y, z);
    }

    // Converts a Vector3Int position from the cube coordinate system to the oddr coordinate system
    // this method should be used before the return statement of hex algorithms
    public static Vector2Int CubeToOddr(Vector3Int hex) {
        int x = hex.x + (hex.z - (hex.z&1))/2;
        int y = hex.z;
        return new Vector2Int(x, y);
    }

    // returns the distance (number of tiles) between two hex positions
    public static float HexDistance(Vector2Int startPos, Vector2Int endPos) {
        Vector3Int dA = OddrToCube(startPos);
        Vector3Int dB = OddrToCube(endPos);
        return (Math.Abs(dA.x - dB.x) 
          + Math.Abs(dA.x + dA.y - dB.x - dB.y)
          + Math.Abs(dA.y - dB.y)) / 2;
    }

    // Returns a list of tiles that form a straight line between two tile positions
    public static List<Vector2Int> FindLine(Vector2Int startPos, Vector2Int endPos) {
        float distance = HexDistance( startPos, endPos );
        List<Vector3Int> resultsCube = new List<Vector3Int>();
        Vector3 cubeStart = OddrToCube( startPos );
        Vector3 cubeEnd = OddrToCube( endPos );
        if(cubeStart == cubeEnd) {
            resultsCube.Add(CubeRound(cubeStart));
        }
        else {
            Vector3 epsilonHex = new Vector3(1e-6f, 2e-6f, -3e-6f);
            if(cubeEnd.z-cubeStart.z>=0) {
                cubeEnd = cubeEnd + epsilonHex;
            } else {
                cubeEnd = cubeEnd - epsilonHex;
            }
            for( int i = 0; i <= distance; i++) {
                resultsCube.Add( CubeRound( CubeLerp( cubeStart, cubeEnd, (1.0f / distance)*i)));
            }
        }
        List<Vector2Int> tileCache = new List<Vector2Int>();
        for(int i = 0; i < resultsCube.Count; i++) {
            Vector2Int OffsetHex = CubeToOddr(resultsCube[i]);
            tileCache.Insert(0, OffsetHex);
        }
        if(tileCache == null || tileCache.Count == 0) {
            Debug.Log("tilecache is 0");
        }
        return tileCache;
    }

    // Finds the direction based on a starting and end position
    // Useful for facing characters the direction they are moving
    public static int FindDirection(Vector2Int startPos, Vector2Int endPos ) {
        float distance = HexDistance( startPos, endPos );
        Vector3Int cubeStart = OddrToCube( startPos );
        Vector3Int cubeEnd = OddrToCube( endPos );
        Vector3Int neighborCubeHex = CubeRound( CubeLerp(cubeStart, cubeEnd, 1.0f / (distance == 0f ? 1.0f : distance)));
        int direction = 0;
        for(int i = 0; i < cubeDirections.Count; i++) {
            if(neighborCubeHex == cubeStart + cubeDirections[i]) {
                direction = i;
            }
        }
        return direction;
    }

    //Finds a line, but excludes the original hex.
    public static List<Vector2Int> FindLineWithoutOrigin(Vector2Int starting, Vector2Int ending) {
        List<Vector2Int> fullLine = FindLine( starting, ending );
        fullLine.Remove( starting );
        return fullLine;
    }

    // Linear extrapolation
    public static float Lerp(float a, float b, float t) {
        return a+(b-a)*t;
    }

    // Linear extrapolates a value for each cubic direction, and places them in a Vector3.
    public static Vector3 CubeLerp(Vector3 a, Vector3 b, float t) {
        return new Vector3( Lerp( a.x, b.x, t), Lerp( a.y, b.y, t), Lerp( a.z, b.z, t));
    }

    // Round a Vector3 with floating values in order to find corresponding Vector3Int.
    // This is usually used to find which Hex a Vector3 value can be found in.
    public static Vector3Int CubeRound(Vector3 a) {
        int rx = Convert.ToInt32(a.x);
        int ry = Convert.ToInt32(a.y);
        int rz = Convert.ToInt32(a.z);

        float xDiff = Math.Abs((float)rx - a.x);
        float yDiff = Math.Abs((float)ry - a.y);
        float zDiff = Math.Abs((float)rz - a.z);

        if (xDiff > yDiff && xDiff > zDiff) {
            rx = -ry-rz;  
        }
        else if (yDiff > zDiff) {
            ry = -rx-rz;
        }
        else {
            rz = -rx-ry;
        }

        return new Vector3Int(rx, ry, rz);
    }

    //Given a tile and a direction integer, return the tile found in that direction.
    public static Vector2Int NeighborTile(Vector2Int startPos, int direction ) {
        direction = direction % 6;
        Vector2Int directionVector;
        if ( startPos.y % 2 == 0 ) {
            directionVector = evenYOffsetDirections[direction];
        } else {
            directionVector = oddYOffsetDirections[direction];
        }
        return startPos + directionVector;
    }

    //Given two tiles, determine whether they are neighbors.
    public static bool IsNeighbor(Vector2Int startingTile, Vector2Int possibleNeighbor){
        
        Vector3Int cubeStart = OddrToCube(startingTile);
        Vector3Int cubePosNeighbor = OddrToCube(possibleNeighbor);
        Vector3Int difference = cubePosNeighbor - cubeStart;
        foreach(Vector3Int direction in cubeDirections) {
            if(difference==direction) {
                return true;
            }
        }
        return false;
    }

    // Returns a list containing the positions of all immediate neighbours to a tile
    public static List<Vector2Int> GetNeighbors(Vector2Int hex, Tilemap tilemap, bool ignoreElevation) {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for(int i = 0; i<=5; i++){
            Vector2Int neighbor = NeighborTile(hex, i);
            HexTile tile = tilemap.GetTile((Vector3Int)neighbor) as HexTile;
            if(tile != null && !tile.attributes.Contains(TileAttribute.Obstacle) && (ignoreElevation || IsElevationReachable(hex,neighbor,tilemap))) {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    // Returns a list containing the positions of all immediate neighbours to a tile
    public static List<Vector2Int> GetNeighborsWithoutMap(Vector2Int hex, bool ignoreElevation) {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int i = 0; i <= 5; i++) {
            neighbors.Add(NeighborTile(hex, i));
        }
        return neighbors;
    }

    public static List<Vector2Int> GetNeighborPositions(Vector2Int hex) {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for(int i = 0; i<=5; i++){
            neighbors.Add(NeighborTile(hex, i));
        }
        return neighbors;
    }

    //This function will remove any tiles from the list of tiles that the board does not actually contain
    public static List<Vector2Int> GetTilePositionsInRange( Tilemap tilemap, Vector2Int startingPos, int range ) {
        List<Vector2Int> tileList = BuildCacheAndGetTiles( startingPos, range );
        List<Vector2Int> paddedList = new List<Vector2Int>();
        for(int i = 0; i < tileList.Count; i++) {
            if(!tilemap.HasTile((Vector3Int)tileList[i])) {
                paddedList.Add(tileList[i]);
            }
        }
        for(int i = 0; i < paddedList.Count; i++) {
            tileList.Remove(paddedList[i]);
        }
        return new List<Vector2Int>( tileList );
    }

    //This function will get all hex coordinates in range, despite the map having them or not
    public static List<Vector2Int> GetTilePositionsInRangeWithoutMap(Vector2Int startingPos, int range) {
        return BuildCacheAndGetTiles(startingPos, range);
    }

    //This function will get all hex coordinates in range, despite the map having them or not
    public static List<Vector2Int> GetTilePositionsInRangeWithoutMapWithoutStarting(Vector2Int startingPos, int range) {
        List<Vector2Int> tileList = GetTilePositionsInRangeWithoutMap(startingPos, range);
        tileList.Remove(startingPos);
        return new List<Vector2Int>(tileList);
    }

    //This function will remove any tiles from the list of tiles that the board does not actually contain
    public static List<Vector2Int> GetTilePositionsInRangeWithoutStarting( Tilemap tilemap, Vector2Int startingPos, int range ) {
        List<Vector2Int> tileList = GetTilePositionsInRange(tilemap, startingPos, range);
        tileList.Remove( startingPos );
        return new List<Vector2Int>( tileList );
    }

    //This function will return all positions in the range of the target and leave in the starting tile and the padded tiles.
    public static List<Vector2Int> GetTilePositionsInRangeWithStartingWithPaddingTiles( Vector2Int startingPos, int range ) {
        List<Vector2Int> tileList = BuildCacheAndGetTiles( startingPos, range );
        return new List<Vector2Int>( tileList );
    }

    //This function will return all positions in the range of the target and leave in the padded tiles, removing the starting tile
    public static List<Vector2Int> GetTilePositionsInRangeWithoutStartingWithPaddingTiles( Vector2Int startingPos, int range ) {
        List<Vector2Int> tileList = BuildCacheAndGetTiles( startingPos, range );
        tileList.Remove( startingPos );
        return new List<Vector2Int>( tileList );
    }

    // This function returns a list of hexes that can be reached, given a certain movement range.
    // Obstacles and the edge of the map are taken into account, and will prevent certain hexes from being reached.
    // if ignoreElevation is false, it won't include tiles with an elevation difference >= 2
    public static List<Vector2Int> HexReachable(Vector2Int startPos, int movementRange, Tilemap tilemap, bool ignoreElevation){
        List<Vector2Int> visited = new List<Vector2Int> {
            startPos
        };
        List<List<Vector2Int>> fringes = new List<List<Vector2Int>>();
        List<Vector2Int> startList = new List<Vector2Int> {
            startPos
        };
        fringes.Add(startList);
        for(int movement = 1; movement <= movementRange; movement++){
            fringes.Add(new List<Vector2Int>());
            foreach(Vector2Int hex in fringes[movement-1]) {
                for(int dir = 0; dir < 6; dir++){
                    Vector2Int neighbor = NeighborTile(hex, dir);
                    if(!visited.Contains(neighbor) && tilemap.HasTile((Vector3Int)neighbor) && (ignoreElevation || IsElevationReachable(hex,neighbor,tilemap))) {
                        visited.Add(neighbor);
                        fringes[movement].Add(neighbor);
                    }
                }
            }
        }

        return visited;
    }
    
    //Takes in a list of tiles, returns the list of tiles with all tiles outside of the tilemap removed
    public static List<Vector2Int> RemoveInvalidTiles(Tilemap tilemap, List<Vector2Int> tileList ){
        List<Vector2Int> targetListToRemove = new List<Vector2Int>();
        
        foreach(Vector2Int tileTarget in tileList){
            if (!tilemap.GetTile((Vector3Int)tileTarget)){
                targetListToRemove.Add(tileTarget);
            }
        }
        foreach(Vector2Int tileTarget in targetListToRemove){
            tileList.Remove(tileTarget);
        }
        return tileList;
    }

    // Finds a path between starting and ending position
    // only paths to non-null tiles on the argument tilemap
    // if ignoreElevation is false, it won't path to an elevation difference >= 2
    public static List<Vector2Int> Pathfinding(Vector2Int start, Vector2Int end, Tilemap tilemap, bool ignoreElevation){
        PriorityQueue<PriorityTuple> frontier = new PriorityQueue<PriorityTuple>();
        frontier.Enqueue(new PriorityTuple(0,start));
        Dictionary<Vector2Int,Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();
        
        cameFrom.Add(start, start);
        costSoFar.Add(start, 0);
        while(frontier.Count()>0){
            Vector2Int current = frontier.Dequeue().GetHex();
            if (current == end){
                break;
            }
            List<Vector2Int> neighbors = GetNeighbors(current, tilemap, ignoreElevation);
            foreach(Vector2Int next in neighbors){
                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]){
                    costSoFar[next] = newCost;
                    int heuristicFunction = (int)HexDistance(next, end);
                    int evalFunction = newCost + heuristicFunction;
                    frontier.Enqueue(new PriorityTuple(evalFunction, next));
                    cameFrom[next] = current;
                }
            }
        }
        List<Vector2Int> path = new List<Vector2Int> ();
        Vector2Int stepBack = end;
        while(cameFrom.ContainsKey(stepBack) && cameFrom[stepBack] != stepBack){
            path.Insert(0, stepBack);
            stepBack = cameFrom[stepBack];
        }

        return path;
    }

    public static List<Vector2Int> FindTilesInVision(Vector2Int position, int range, Tilemap tilemap, bool ignoreElevation) {
        List<Vector2Int> outerRing = FindRing(position,range);
        HashSet<Vector2Int> visionTiles = new HashSet<Vector2Int>();
        for(int i=0; i<outerRing.Count; i++) {
            List<Vector2Int> line = FindVisionLine(position,outerRing[i],tilemap);
            for(int k=0; k<line.Count; k++) {
                visionTiles.Add(line[k]);
            }
        }

        return visionTiles.ToList();
    }

    public static List<Vector2Int> FindVisionLine(Vector2Int startPos, Vector2Int endPos, Tilemap tilemap) {
        float distance = HexDistance( startPos, endPos );
        List<Vector2Int> visionLine = new List<Vector2Int>();
        Vector3 cubeStart = OddrToCube( startPos );
        Vector3 cubeEnd = OddrToCube( endPos );
        if(cubeStart == cubeEnd) {
            visionLine.Add(CubeToOddr(CubeRound(cubeStart)));

        } else {
            Vector3 epsilonHex = new Vector3(1e-6f, 2e-6f, -3e-6f);
            if(cubeEnd.z-cubeStart.z>=0) {
                cubeEnd = cubeEnd + epsilonHex;
            } else {
                cubeEnd = cubeEnd - epsilonHex;
            }

            HexTile startTile = tilemap.GetTile((Vector3Int)startPos) as HexTile;
            if(startTile == null) {
                return visionLine;
            }
            for( int i = 0; i <= distance; i++) {
                Vector3Int cubePosition = CubeRound( CubeLerp( cubeStart, cubeEnd, (1.0f / distance)*i));
                Vector2Int tilePosition = CubeToOddr(cubePosition);
                HexTile tile = tilemap.GetTile((Vector3Int)tilePosition) as HexTile;
                
                if(tile == null) {
                    continue;
                }
                visionLine.Add(tilePosition);
                if(tile.elevation > startTile.elevation) {
                    break;
                }
            }
        }
        
        return visionLine;
    }

    // Finds a path with direction between starting and ending position
    // only paths to non-null tiles on the argument tilemap
    // if ignoreElevation is false, it won't path to an elevation difference >= 2
    public static List<Tuple<Vector2Int,int>> PathfindingWithDirection(Vector2Int start, Vector2Int end, Tilemap tilemap, bool ignoreElevation){
        PriorityQueue<PriorityTuple> frontier = new PriorityQueue<PriorityTuple>();
        frontier.Enqueue(new PriorityTuple(0,start));
        Dictionary<Vector2Int,Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();
        
        cameFrom.Add(start, start);
        costSoFar.Add(start, 0);
        while(frontier.Count()>0){
            Vector2Int current = frontier.Dequeue().GetHex();
            if (current == end){
                break;
            }
            List<Vector2Int> neighbors = GetNeighbors(current, tilemap, ignoreElevation);
            foreach(Vector2Int next in neighbors){
                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]){
                    costSoFar[next] = newCost;
                    int heuristicFunction = (int)HexDistance(next, end);
                    int evalFunction = newCost + heuristicFunction;
                    frontier.Enqueue(new PriorityTuple(evalFunction, next));
                    cameFrom[next] = current;
                }
            }
        }
        List<Tuple<Vector2Int,int>> path = new List<Tuple<Vector2Int,int>> ();
        Vector2Int stepBack = end;
        while(cameFrom.ContainsKey(stepBack) && cameFrom[stepBack] != stepBack) {
            int direction = FindDirection(cameFrom[stepBack],stepBack);
            path.Insert(0, new Tuple<Vector2Int, int>(stepBack,direction));
            stepBack = cameFrom[stepBack];
        }

        return path;
    }

    // Returns true if elevated movement between two tiles is possible, false otherwise
    public static bool IsElevationReachable(Vector2Int startPos, Vector2Int endPos, Tilemap tilemap) {
        if(tilemap == null) {
            return false;
        }

        HexTile startTile = tilemap.GetTile((Vector3Int)startPos) as HexTile;
        HexTile endTile = tilemap.GetTile((Vector3Int)endPos) as HexTile;

        if(startTile == null || endTile == null) {
            return false;
        }

        return Math.Abs(endTile.elevation - startTile.elevation) < ELEVATION_REACHABLE_DISTANCE;
    }

    public static List<Vector2Int> FindRing(Vector2Int center, int distance) {
        // TODO: get it working with cache
        List<Vector2Int> results = new List<Vector2Int>();
        Vector3Int cubeCenter = OddrToCube(center);
        Vector3Int cube = cubeAdd(cubeCenter, cubeScale(cubeDirections[4], distance));
        for(int i = 0; i < 6; i++) {
            for(int j = 0; j < distance; j++) {
                Vector2Int position = CubeToOddr(cube);
                results.Add(position);
                cube = OddrToCube(NeighborTile(position,i));
            }
        }
        return results;
    }

    public static bool IsEdgeTile(Vector2Int position, Tilemap tilemap) {
        List<Vector2Int> neighbors = GetNeighbors(position,tilemap,true);
        return neighbors.Count < 6;
    }

    public static int DirectionToAngle(int direction) {
        switch(direction) {
            case 1:
                return 60;
            case 2:
                return 120;
            case 3:
                return 180;
            case 4:
                return 240;
            case 5:
                return 300;
            default:
                return 0;
        }
    }

    //Helper class to say that direction(0) = curPos + cubeDirections[0]
    //DO NOT CHANGE THE ORDER OF THE LIST
    private static readonly List<Vector2Int> oddYOffsetDirections = new List<Vector2Int> {
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1),
    };
    private static readonly List<Vector2Int> evenYOffsetDirections = new List<Vector2Int> {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
    };

    private static List<Vector3Int> cubeDirections = new List<Vector3Int> {
        new Vector3Int(1, -1, 0),
        new Vector3Int(0, -1, 1),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, 1, -1),
        new Vector3Int(1, 0, -1),
    };

    private static Vector3Int cubeAdd(Vector3Int a, Vector3Int b) {
        return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    private static Vector3Int cubeScale(Vector3Int a, int k) {
        return new Vector3Int(a.x * k, a.y * k, a.z * k);
    }

    // This dictionary is in the format of tile coordinates to a list of lists of tile coordinates
    // the list will be of size n, where n is the highest range we calculated
    // List(1) will return List<Vector3Int>, where each Vector3Int in that list is a tile within 1 range of the key
    private static Dictionary<Vector2Int, List<List<Vector2Int>>> rangeCalculationCache = new Dictionary<Vector2Int, List<List<Vector2Int>>>();

    //Note: this function will add tiles that do not exist to the cache (which is required for some uses that need padding around the tiles)
    private static List<Vector2Int> BuildCacheAndGetTiles( Vector2Int startingPos, int range) {
        List<Vector2Int> returnList = new List<Vector2Int>();
        
        //taking absolute to ensure no negatives
        int absMovementDistance = Math.Abs( range );

        //This is a variable to keep track of which ring(s) we need to calculate
        //    if the list already contains 2 rings and we need a range of 3,
        //    that means we just need to calculate range of distance 2
        //Add one to the range because if range is 2, we really need 3 rings
        int offsetRange = absMovementDistance + 1;
        List<List<Vector2Int>> tileCache;
        if (!rangeCalculationCache.TryGetValue( startingPos, out tileCache )) {
            tileCache = new List<List<Vector2Int>>();
            List<Vector2Int> initialPosition = new List<Vector2Int> {
                startingPos
            };
            tileCache.Add( initialPosition );
        }
        //We don't need to do any calculations, we have enough information cached
        if(tileCache.Count > absMovementDistance) {
            for(int i = 0; i < offsetRange; i++) {
                returnList.AddRange( tileCache[i] );
            }
            return returnList;
        }
        //If the above condition wasn't true, we have to calulate any additional rings that aren't cached
        Vector2Int curPos;
        int startingRing = tileCache.Count;
        //This loop effectively does a "walk" around the ring and adds the tile to the map if the tile is on the board
        for (int ring = startingRing; ring < offsetRange; ring++) {
            curPos = startingPos;
            //This will walk us from the starting positing to the position we need to start the calculation from
            for(int counter = 0; counter < ring; counter++) {
                curPos = NeighborTile( curPos, 4 );
            }
            List<Vector2Int> targetRing = new List<Vector2Int>();
            //Since a hexagon has side length == radius, we have to calculate edges equal to the current ring
            for(int dir = 0; dir < 6; dir++) {
                for (int i = 0; i < ring; i++) {
                    targetRing.Add( curPos );
                    curPos = NeighborTile( curPos, dir );
                }
            }
            targetRing.Remove( startingPos );
            tileCache.Add( targetRing );
        }
        for (int i = 0; i < offsetRange; i++) {
            returnList.AddRange( tileCache[i] );
        }

        rangeCalculationCache.Remove( startingPos );
        rangeCalculationCache.Add( startingPos, tileCache );
        return returnList;
    }

}
