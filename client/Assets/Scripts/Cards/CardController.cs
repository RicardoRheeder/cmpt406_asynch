using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class CardController {

    [DataMember]
    public string owner;

    [DataMember]
    private List<int> hand;
    public List<CardFunction> Hand { get; set; }

    public CardController(string username, List<CardFunction> hand) {
        this.owner = username;
        this.Hand = hand;
    }

    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context) {
        if (hand == null) hand = new List<int>();

        Hand = new List<CardFunction>();
        for(int i = 0; i < hand.Count; i++) {
            Hand.Add((CardFunction)hand[i]);
        }
    }

    //Note: the unit type will never change so we don't have to update the int value
    [OnSerializing()]
    internal void OnSerializingMethod(StreamingContext context) {
        hand = new List<int>();
        for (int i = 0; i < hand.Count; i++) {
            hand.Add((int)Hand[i]);
        }
    }
}
