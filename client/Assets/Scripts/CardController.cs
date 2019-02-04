using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController {
    private List<Card> hand;
    private List<Card> drawPile;
    private List<Card> discardPile;

    public CardController(List<Card> hand, List<Card> drawPile, List<Card> discardPile) {
        this.hand = hand;
        this.drawPile = drawPile;
        this.discardPile = discardPile;
    }
}
