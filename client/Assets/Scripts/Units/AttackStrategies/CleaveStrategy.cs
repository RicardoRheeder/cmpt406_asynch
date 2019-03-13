using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaveStrategy : IAttackStrategy {
    public List<Tuple<Vector2Int, int>> Attack(UnitStats source, Vector2Int target) {
        List<Vector2Int> sourceSurroundingTiles = HexUtility.GetTilePositionsInRangeWithoutMapWithoutStarting(source.Position, 1);
        List<Vector2Int> targetSurroundingTiles = HexUtility.GetTilePositionsInRangeWithoutMap(target, 1);
        List<Tuple<Vector2Int, int>> targets = new List<Tuple<Vector2Int, int>>();
        for (int i = 0; i < targetSurroundingTiles.Count; i++) {
            Vector2Int pos = targetSurroundingTiles[i];
            if(sourceSurroundingTiles.Contains(pos)) {
                targets.Add(new Tuple<Vector2Int, int>(pos, source.Damage));
            }
        }
        return targets;
    }
}
