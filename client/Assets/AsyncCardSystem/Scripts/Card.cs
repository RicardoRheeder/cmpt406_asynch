using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {


    //    public Tuple<Effects, int> effectValue = new Tuple<Effects, int>(Effects.DamageAmp, 0);

   

//        public Tuple<Effects, int> cardEffect;

//    public List<effectTypes> cardEffects;

    public new string name;

    public string[] effects;
	public string description;

	public Sprite artwork;

	public int cardCost;
//	public int attack;
//	public int health;

	public void Print ()
	{
		Debug.Log(name + ": " + description + " The card costs: " + cardCost);
	}

}

public class Order
{

    public int ID { get; set; }
    public string Username { get; set; }
    public int Amount { get; set; }
    public float Price { get; set; }

    public Order(int ID, string Username, int Amount, float Price)
    {
        this.ID = ID;
        this.Username = Username;
        this.Amount = Amount;
        this.Price = Price;
    }
}
[System.Serializable]
public class effectTypes{
    public enum Effects
    {
        DamageAmp = 1,
        MovementAmp = 2,
    }

    public Effects CardEffects;
    public int amount;
}
