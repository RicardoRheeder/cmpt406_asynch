using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deckDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject DeckDisplayTemplate;

    public GameObject DeckDisplay;

    void Start()
    {
        DeckDisplay = Instantiate(DeckDisplayTemplate);
        DeckDisplay.transform.SetParent(this.transform, false);
        DeckDisplay.transform.localPosition = new Vector3(0, 0, 0);
//        DeckDisplay.transform. = DeckDisplayTemplate.transform;
        // Temporary line until I have the back of the card for deck top
        DeckDisplay.GetComponent<CardDisplay_v2>().card = null;
        DisplayDeck();
    }

    private void HideDeck()
    {
        DeckDisplay.SetActive(false);
    }

    private void DisplayDeck()
    {
        DeckDisplay.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
