using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaveStrategy : IAttackStrategy {
    public List<Tuple<Vector2Int, int>> Attack(UnitStats source, Vector2Int target) {
        return new List<Tuple<Vector2Int, int>>() {
            new Tuple<Vector2Int, int>(target, source.Damage)
        };
    }
}
