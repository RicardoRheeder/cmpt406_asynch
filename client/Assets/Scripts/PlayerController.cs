using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController {

    private CardController deck;

    private GameManager manager;

    public PlayerController(GameManager manager, CardController deck) {
        this.deck = deck;
        this.manager = manager;
    }
}
