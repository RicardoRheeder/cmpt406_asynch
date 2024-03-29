﻿using System.Collections.Generic;
using UnityEngine;

//An attack strategy that given a target and a source, will return damage on all of the tiles in between
public class LineStrategy : IAttackStrategy {
    public List<Tuple<Vector2Int, int>> Attack(UnitStats source, Vector2Int target) {
        List<Tuple<Vector2Int, int>> damageList = new List<Tuple<Vector2Int, int>>();
        List<Vector2Int> tiles = HexUtility.FindLineWithoutOrigin(source.Position,target);
        for (int i = 0; i < tiles.Count; i++) {
            damageList.Add(new Tuple<Vector2Int, int>(tiles[i], source.Damage));
        }
        return damageList;
    }
}
