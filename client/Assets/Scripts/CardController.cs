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

    public CardController(List<Card> hand, List<Card> drawPile, List<Card> discardPile) {
        this.hand = hand;
        this.drawPile = drawPile;
        this.discardPile = discardPile;
    }
}
