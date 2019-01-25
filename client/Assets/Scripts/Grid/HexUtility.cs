using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class HexUtility {
    public static Vector3Int OddrToCube(Vector3Int hex) {
		int x = hex.x - (hex.y - (hex.y&1)) / 2;
		int z = hex.y;
		int y = -x-z;
		return new Vector3Int(x, y, z);
	}

    public static Vector3Int CubeToOddr(Vector3Int hex) {
        int x = hex.x + (hex.z - (hex.z&1))/2;
        int y = hex.z;
        return new Vector3Int(x, y, 0);
	}

    public static float HexDistance(Vector3Int a, Vector3Int b) {
		Vector3Int dA = OddrToCube( a);
		Vector3Int dB = OddrToCube( b);
		return (Math.Abs(dA.x - dB.x) 
          + Math.Abs(dA.x + dA.y - dB.x - dB.y)
          + Math.Abs(dA.y - dB.y)) / 2;
	}


    public static List<Vector3Int> FindLine(Vector3Int starting, Vector3Int ending) {
        List<Vector3Int > tileCache;
        
        float distance = HexDistance( starting, ending );
        List<Vector3Int> resultsCube = new List<Vector3Int>();
        Vector3 dA = OddrToCube( starting );
        Vector3 dB = OddrToCube( ending );
        if(dA == dB)
        {
            resultsCube.Add(CubeRound(dA));
        }
        else{
            Vector3 epsilonHex = new Vector3(1e-6f, 2e-6f, -3e-6f);
            if(dB.z-dA.z>=0){
                dB = dB + epsilonHex;
            }
            else{
                dB = dB - epsilonHex;
            }
            for( int i = 0; i <= distance; i++){
                resultsCube.Add( CubeRound( CubeLerp( dA, dB, (1.0f / distance)*i)));
            }
        }
        tileCache = new List<Vector3Int>();
        foreach(Vector3Int CubeHex in resultsCube){
            Vector3Int OffsetHex = CubeToOddr(CubeHex);

            tileCache.Insert(0, OffsetHex);
        }
        if(tileCache == null || tileCache.Count == 0){
            Debug.Log("tilecache is 0");
        }
        return tileCache;

    }

    //Finds the direction based on a starting and end position
    public static int FindDirection(Vector3Int starting, Vector3Int ending ) {
        float distance = HexDistance( starting, ending );
        Vector3Int dA = OddrToCube( starting );
        Vector3Int dB = OddrToCube( ending );
        Vector3Int neighborCubeHex = CubeRound( CubeLerp(dA, dB, 1.0f / (distance == 0f ? 1.0f : distance)));
        int direction = 0;
        for(int i = 0; i < cubeDirections.Count; i++) {
            if(neighborCubeHex == dA + cubeDirections[i]) {
                direction = i;
            }
        }
        return direction;
    }

    //Finds a line, but excludes the original hex.
    public static List<Vector3Int> FindLineWithoutOrigin(Vector3Int starting, Vector3Int ending) {
        List<Vector3Int> fullLine = FindLine( starting, ending );
        List<Vector3Int> result = new List<Vector3Int>(fullLine);
        result.Remove( starting );
        return result;
    }

    //Linear extrapolation
    public static float Lerp(float a, float b, float t) {
        return a+(b-a)*t;
    }

    //Linear extrapolates a value for each cubic direction, and places them in a Vector3.
    public static Vector3 CubeLerp(Vector3 a, Vector3 b, float t) {
        return new Vector3( Lerp( a.x, b.x, t), Lerp( a.y, b.y, t), Lerp( a.z, b.z, t));
    }

    //Round a Vector3 with floating values in order to find corresponding Vector3Int.
    //This is usually used to find which Hex a Vector3 value can be found in.
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


    //Helper class to say that direction(0) = curPoos + cubeDirections[0]
    //DO NOT CHANGE THE ORDER OF THE LIST
    private static readonly List<Vector3Int> oddYOffsetDirections = new List<Vector3Int> {
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, -1, 0),
    };
    private static readonly List<Vector3Int> evenYOffsetDirections = new List<Vector3Int> {
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(0, -1, 0),
    };

    private static List<Vector3Int> cubeDirections = new List<Vector3Int> {
        new Vector3Int(1, -1, 0),
        new Vector3Int(0, -1, 1),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, 1, -1),
        new Vector3Int(1, 0, -1),
    };

    //Given a tile and a direction integer, return the tile found in that direction.
    public static Vector3Int NeighborTile(Vector3Int starting, int direction ) {
        direction = direction % 6;
        Vector3Int directionVector;
        if ( starting.y % 2 == 0 ) {
            directionVector = evenYOffsetDirections[direction];
        }
        else {
            directionVector = oddYOffsetDirections[direction];
        }
        return starting + directionVector;
    }

    //Given two tiles, determine whether they are neighbors.
    public static bool IsNeighbor(Vector3Int startingTile, Vector3Int possibleNeighbor){
        
        Vector3Int cubeStart = OddrToCube(startingTile);
        Vector3Int cubePosNeighbor = OddrToCube(possibleNeighbor);
        Vector3Int difference = cubePosNeighbor-cubeStart;
        foreach(Vector3Int direction in cubeDirections){
            if(difference==direction){
                return true;
            }
        }
        return false;
    }

    public static List<Vector3Int> GetNeighbors(Vector3Int hex, Tilemap tilemap){
        List<Vector3Int> neighbors = new List<Vector3Int>();
        for(int i = 0; i<=5; i++){
            Vector3Int neighbor = NeighborTile(hex, i);
            if(tilemap.HasTile(neighbor)){
                neighbors.Add(neighbor);
            }
            
        }
        return neighbors;
    }

    // This dictionary is in the format of tile coordinates to a list of lists of tile coordinates
    // the list will be of size n, where n is the highest range we calculated
    // List(1) will return List<Vector3Int>, where each Vector3Int in that list is a tile within 1 range of the key
    private static Dictionary<Vector3Int, List<List<Vector3Int>>> rangeCalculationCache = new Dictionary<Vector3Int, List<List<Vector3Int>>>();
    private static Dictionary<Tuple<Vector3Int, int>, List<Vector3Int>> testRangeCalculationCache = new Dictionary<Tuple<Vector3Int, int>, List<Vector3Int>>();

    //Note: this function will add tiles that do not exist to the cache (which is required for some uses that need padding around the tiles)
    private static List<Vector3Int> BuildCacheAndGetTiles( Vector3Int startingPos, int range) {
        List<Vector3Int> returnList = new List<Vector3Int>();
        
        //taking absolute to ensure no negatives
        int absMovementDistance = Math.Abs( range );

        //This is a variable to keep track of which ring(s) we need to calculate
        //    if the list already contains 2 rings and we need a range of 3,
        //    that means we just need to calculate range of distance 2
        //Add one to the range because if range is 2, we really need 3 rings
        int offsetRange = absMovementDistance + 1;
        List<List<Vector3Int >> tileCache;
        if (!rangeCalculationCache.TryGetValue( startingPos, out tileCache )) {
            tileCache = new List<List<Vector3Int>>();
            List<Vector3Int> initialPosition = new List<Vector3Int> {
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
        Vector3Int curPos;
        int startingRing = tileCache.Count;
        //This loop effectively does a "walk" around the ring and adds the tile to the map if the tile is on the board
        for (int ring = startingRing; ring < offsetRange; ring++) {
			curPos = startingPos;
            //This will walk us from the starting positing to the position we need to start the calculation from
			for(int counter = 0; counter < ring; counter++) {
				curPos = NeighborTile( curPos, 4 );
			}
            List<Vector3Int> targetRing = new List<Vector3Int>();
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

    //This function will will remove any tiles from the list of tiles that the board does not actually contain
    public static List<Vector3Int> GetTilePositionsInRange( Tilemap tilemap, Vector3Int startingPos, int range ) {
        List<Vector3Int> tileList = BuildCacheAndGetTiles( startingPos, range );
        List<Vector3Int> paddedList = new List<Vector3Int>();
        foreach (Vector3Int tile in tileList) {
            if (!tilemap.HasTile( tile )) {
                paddedList.Add( tile );
            }
        }
        foreach (Vector3Int tile in paddedList) {
            tileList.Remove( tile );
        }
        return new List<Vector3Int>( tileList );
    }

    public static List<Vector3Int> TestGetTilePositionsInRange(Tilemap tilemap, Vector3Int startingPos, int range){
        Tuple<Vector3Int,int> posAndRange = new Tuple<Vector3Int,int>(startingPos, range);
        if(!testRangeCalculationCache.ContainsKey(posAndRange)){
            List<Vector3Int> results = new List<Vector3Int>();
            for (int x = -range; x<=range; x++){
                for( int y = Math.Max(-range, -x-range); y <= Math.Min(+range, -x+range); y++){
                    int z = -x-y;
                    Vector3Int startingPosCube = OddrToCube(startingPos);
                    Vector3Int tileInRange = startingPosCube + new Vector3Int(x,y,z);
                    tileInRange = CubeToOddr(tileInRange);
                    if(tilemap.HasTile(tileInRange)){
                        results.Add(tileInRange);
                    }
                    
                }
                
            }
            testRangeCalculationCache.Add(posAndRange, results);
            return results;
            }
        else{
            return testRangeCalculationCache[posAndRange];
        }
        
            

    }

    //This function will will remove any tiles from the list of tiles that the board does not actually contain
    public static List<Vector3Int> GetTilePositionsInRangeWithoutStarting( Tilemap tilemap, Vector3Int startingPos, int range ) {
        List<Vector3Int> tileList = GetTilePositionsInRange(tilemap, startingPos, range);
        tileList.Remove( startingPos );
        return new List<Vector3Int>( tileList );
    }

    public static List<Vector3Int> TestGetTilePositionsInRangeWithoutStarting( Tilemap tilemap, Vector3Int startingPos, int range ) {
        List<Vector3Int> tileList = TestGetTilePositionsInRange(tilemap, startingPos, range);
        tileList.Remove( startingPos );
        return new List<Vector3Int>( tileList );
    }

    //This function will return all positions in the range of the target and leave in the starting tile and the padded tiles.
    public static List<Vector3Int> GetTilePositionsInRangeWithStartingWithPaddingTiles( Vector3Int startingPos, int range ) {
        List<Vector3Int> tileList = BuildCacheAndGetTiles( startingPos, range );
        return new List<Vector3Int>( tileList );
    }

    //This function will return all positions in the range of the target and leave in the padded tiles, removing the starting tile
    public static List<Vector3Int> GetTilePositionsInRangeWithoutStartingWithPaddingTiles( Vector3Int startingPos, int range ) {
        List<Vector3Int> tileList = BuildCacheAndGetTiles( startingPos, range );
        tileList.Remove( startingPos );
        return new List<Vector3Int>( tileList );
    }

    //This function returns a list of hexes that can be reached, given a a certain movement range.
    //Obstacles and the edge of the map are taken into account, and will prevent certain hexes from being reached.
    public static HashSet<Vector3Int> HexReachable(Vector3Int starting, int movementRange, Tilemap tilemap){
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        visited.Add(starting);
        List<List<Vector3Int>> fringes = new List<List<Vector3Int>>();
        List<Vector3Int> startList = new List<Vector3Int>();
        startList.Add(starting);
        fringes.Add(startList);
        for(int movement = 1; movement <= movementRange; movement++){
            fringes.Add(new List<Vector3Int>());
            foreach(Vector3Int hex in fringes[movement-1]){
                for(int dir = 0; dir < 6; dir++){
                    Vector3Int neighbor = NeighborTile(hex, dir);
                    if(!visited.Contains(neighbor) && tilemap.HasTile(neighbor)){
                        visited.Add(neighbor);
                        fringes[movement].Add(neighbor);
                    }
                }
            }
        }
        return visited;
    }
	
	//Takes in a list of tiles, returns the list of tiles with all tiles outside of the tilemap removed
	public static List<Vector3Int> RemoveInvalidTiles(Tilemap tilemap, List<Vector3Int> tileList ){
		List<Vector3Int> targetListToRemove = new List<Vector3Int>();
		
		foreach(Vector3Int tileTarget in tileList){
			if (!tilemap.GetTile(tileTarget)){
				targetListToRemove.Add(tileTarget);
			}
		}
		foreach(Vector3Int tileTarget in targetListToRemove){
			tileList.Remove(tileTarget);
		}
		return tileList;
	}

    public static List<Vector3Int> Pathfinding(Vector3Int start, Vector3Int end, Tilemap tilemap){
        PriorityQueue<PriorityTuple> frontier = new PriorityQueue<PriorityTuple>();
        frontier.Enqueue(new PriorityTuple(0,start));
        Dictionary<Vector3Int,Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        
        cameFrom.Add(start, start);
        costSoFar.Add(start, 0);
        while(frontier.Count()>0){
            Vector3Int current = frontier.Dequeue().GetHex();
            if (current == end){
                break;
            }
            List<Vector3Int> neighbors = GetNeighbors(current, tilemap);
            foreach(Vector3Int next in neighbors){
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
        List<Vector3Int> path = new List<Vector3Int> ();
        Vector3Int stepBack = end;
        while(cameFrom[stepBack] != stepBack){
            path.Insert(0, stepBack);
            stepBack = cameFrom[stepBack];
        }

        return path;
    }
}
