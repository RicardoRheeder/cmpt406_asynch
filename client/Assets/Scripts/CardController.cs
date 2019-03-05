using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class CardController {

    [DataMember]
    public string owner;

    [DataMember]
    private List<Card> hand;

    [DataMember]
    private List<Card> drawPile;

    [DataMember]
    private List<Card> discardPile;

    public CardController(string username, List<Card> hand, List<Card> drawPile, List<Card> discardPile) {
        this.owner = username;
        this.hand = hand;
        this.drawPile = drawPile;
        this.discardPile = discardPile;
    }
}
