using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {

    public string displayName;
    public string[] effects;
    public string description;
    public Sprite artwork;

    public CardFunction func;
    public UnitType type;

    public int cardCost;
//    public int attack;
//    public int health;

    public void Print ()  {
        Debug.Log(displayName + ": " + description + " The card costs: " + cardCost);
    }

}
