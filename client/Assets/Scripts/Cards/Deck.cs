using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;


[CreateAssetMenu(fileName = "New Deck", menuName = "Deck")]
public class Deck : ScriptableObject {
    [SerializeField]
    public List<Card> deck;

    public void PrintDeck() {
        Debug.Log("Deck Size: " + deck.Count);
        int count = 1;
        foreach (Card card in deck) {
            Debug.Log(count + ": " + card.displayName);
            count++;
        }
    }

    public Card DrawCard() {
        return deck[Random.Range(0, deck.Count)];
    }
}
