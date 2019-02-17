using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy {
    List<Tuple<Vector2Int, int>> Attack(UnitStats source, Vector2Int target);
}
