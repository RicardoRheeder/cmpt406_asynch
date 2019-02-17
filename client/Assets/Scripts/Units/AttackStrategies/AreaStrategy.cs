using System.Collections.Generic;
using UnityEngine;

//Attack all units in an area around the target tile
public class AreaStrategy : IAttackStrategy {
    public List<Tuple<Vector2Int, int>> Attack(UnitStats source, Vector2Int target) {
        return new List<Tuple<Vector2Int, int>>() {
            new Tuple<Vector2Int, int>(target, source.Damage)
        };
    }
}
