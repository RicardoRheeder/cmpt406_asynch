using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class CardSystemManager : MonoBehaviour
{

    public int maxHP = 20;

    public int currentHP;

    public Deck full_deck;

    [SerializeField]
    private GameObject CardTemplate; //Card_V2


    public List<Card> currentDeck;

    public List<Card> currentHand;

    public Card currentCard;

    public List<Card> graveyard;

    [SerializeField]
    private Button aButton;


    public GameObject TableTop;
    public GameObject TableHand;

    void Awake()
    {
        TableTop = GameObject.Find("Tabletop");
        TableHand = GameObject.Find("Hand");

        currentHP = maxHP;
        if (full_deck.deck != null)
        {
            foreach (Card currentCard in full_deck.deck)
            {
                currentDeck.Add(currentCard);
            }
        }
    }

    void Start()
    {
//        aButton.onClick.AddListener(delegate { click(); });
    }

    void Update()
    {
        if (currentHP == 0)
        {
            Debug.Log("HP is zero");

        }
        if (currentDeck.Count == 0)
        {
            Debug.Log("'s deck is empty");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print(currentDeck);
        }

        if (currentDeck.Count == 0)
        {
            this.SendMessage("HideDeck");
        }

        if (Input.GetMouseButtonDown(1))
        {
            shuffle(currentDeck);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            DrawCard();
        }
    }

    public void print(List<Card> a_deck)
    {
        Debug.Log("Deck Size: " + a_deck.Count);
        int count = 1;
        foreach (Card card in a_deck)
        {
            Debug.Log(count + ": " + card.name);
            count++;
        }
    }

    public Card pop(List<Card> a_deck)
    {
        Assert.AreNotEqual(a_deck.Count, 0);
        Card cardDrawn = a_deck.Last();
        a_deck.RemoveAt(a_deck.Count - 1);
        return cardDrawn;
    }

    private void DrawCard()
    {
        currentCard = pop(currentDeck);
        currentHand.Add(currentCard);

        //        GameObject drawnCard = (GameObject)PrefabUtility.InstantiatePrefab(CardTemplate);
        GameObject drawnCard = (GameObject)Instantiate(CardTemplate, transform.position, Quaternion.identity);

        //        drawnCard.transform.SetParent(Player1DeckUI.transform, false);
        //        drawnCard.transform.position = Player1DeckUI.transform.position;
        drawnCard.GetComponent<CardDisplay_v2>().setCard(currentCard); 

//        drawnCard.transform.position = new Vector3(0,0,0);
        drawnCard.transform.SetParent(TableHand.transform, false);
        
        

    }

    public void shuffle(List<Card> a_deck)
    {
        for (int i = 0; i < a_deck.Count; i++)
        {
            Card temp = a_deck[i];
            int randomIndex = Random.Range(i, a_deck.Count);
            a_deck[i] = a_deck[randomIndex];
            a_deck[randomIndex] = temp;
        }
    }
}
