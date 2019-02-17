using System.Collections.Generic;
using UnityEngine;

//An attack strategy that given a target and a source, will return damage on all of the tiles in between
public class LineStrategy : IAttackStrategy {
    public List<Tuple<Vector2Int, int>> Attack(UnitStats source, Vector2Int target) {
        throw new System.NotImplementedException();
    }
}
