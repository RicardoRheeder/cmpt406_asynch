using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CardsAndCarnage;

public enum CardFunction {
    //Generic Cards
    card1 = 1

    //Trooper Cards

    //Recon Cards

    //Steamer Cards

    //Pew Pew Cards

    //Compensator Cards

    //Foundation Cards

    //Power Surge Cards

    //Midas Cards

    //Claymore Cards

};

public static class CardMetadata {

    public static readonly int GENERIC_CARD_LIMIT = 4;
    public static readonly int UNIQUE_CARD_LIMIT = 3;

    public static readonly Dictionary<CardFunction, CardEffect<UnitStats>> CardEffectDictionary = new Dictionary<CardFunction, CardEffect<UnitStats>>() {
    };

    private static void IncreaseMovement(ref UnitStats unit) {

    }
}
