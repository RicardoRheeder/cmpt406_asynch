using System.Collections.Generic;
using UnityEngine;

//Attack all units in an area around the target tile
public class AreaStrategy : IAttackStrategy {
    public List<Tuple<Vector2Int, int>> Attack(UnitStats source, Vector2Int target) {
        List<Tuple<Vector2Int, int>> damageList = new List<Tuple<Vector2Int, int>>();
        List<Vector2Int> tiles = HexUtility.GetTilePositionsInRangeWithoutMap(target, source.Aoe);
        for(int i = 0; i < tiles.Count; i++) {
            damageList.Add(new Tuple<Vector2Int, int>((Vector2Int)tiles[i], source.Damage));
        }
        return damageList;
    }
}
