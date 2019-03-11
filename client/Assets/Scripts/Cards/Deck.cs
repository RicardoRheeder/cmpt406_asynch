using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;


[CreateAssetMenu(fileName = "New Deck", menuName = "Deck")]
public class Deck : ScriptableObject {
    [SerializeField]
    public List<Card> deck;

    public Card DrawCard() {
        return deck[Random.Range(0, deck.Count)];
    }
}
