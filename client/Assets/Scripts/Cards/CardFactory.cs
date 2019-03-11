using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardFactory {

    public static CardController GetCardControllerFromUnits(List<UnitStats> units, string username) {
        return new CardController(username, new List<CardFunction>());
    }
}
