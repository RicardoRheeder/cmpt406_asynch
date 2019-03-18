﻿using System;
using UnityEngine;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public GameObject go;

    public GameObject PlayCardPanel;
    public GameObject NotEnoughCardPointsText;

    private PlayerController controller;
    private CardSystemManager manager;

    private GameObject cardOnTableTop;

    void Start() {
        PlayCardPanel.SetActive(false);
        cardOnTableTop = null;
    }

    public void SetPlayerController(PlayerController controller) {
        this.controller = controller;
    }

    public void SetCardSystemManager(CardSystemManager manager) {
        this.manager = manager;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();

        if(d != null) {
            d.placeholderParent = this.transform;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        if(eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if(d != null && d.placeholderParent==this.transform) {
            d.placeholderParent = d.parentToReturnTo;
        }
    }

    
    public void OnDrop(PointerEventData eventData) {
        GameObject droppedCard = eventData.pointerDrag.gameObject;
        if (manager == null) {
            manager = GameObject.FindObjectOfType<CardSystemManager>();
        }

        if (droppedCard.GetComponent<Draggable>().allowDrag == true) {
            Draggable d = droppedCard.GetComponent<Draggable>();
            if (d != null){
                d.parentToReturnTo = this.transform;
            }

            if (this.gameObject.name == "Tabletop") {
                cardOnTableTop = droppedCard;
                PlayCardPanel.SetActive(true);
                
            }

            if (this.gameObject.name == "DiscardPanel") {
                Card card = eventData.pointerDrag.gameObject.GetComponent<CardDisplay>().card;
                print("Card: " + card);

                if (manager != null) {
                    manager.PlayCard(card);

                    //TODO: Get better way of hiding tabletop after discarding a card
                    GameObject TableTopPanel = GameObject.Find("Tabletop");
                    TableTopPanel.SetActive(false);
                    if (droppedCard.GetComponent<Draggable>().tempCardDisplaying != null)
                    {
                        Destroy(droppedCard.GetComponent<Draggable>().tempCardDisplaying);
                    }

                    manager.IncrementCardPoints(manager.discardRegainPointsAmount); //Regain points based on discarded card
                    StartCoroutine(DestroyCardAfterDelay(droppedCard));
                }
            }
        }
    }

    IEnumerator DestroyCardAfterDelay(GameObject cardToDestroy) {
        yield return new WaitForSeconds(0.5f);
        Destroy(cardToDestroy);
    }

    // ConfirmPlayCardButton calls this
    public void ConfirmPlayCard() {
        if (this.gameObject.name == "Tabletop" && cardOnTableTop != null){
            Card card = cardOnTableTop.GetComponent<CardDisplay>().card;

            if (card.cardCost <= manager.GetRemainingCardPoints()) {
                PlayCardPanel.SetActive(false);

                // Deals with dissolving
                Transform cardTemp = cardOnTableTop.transform.Find("CardTemplate");
                cardTemp.gameObject.GetComponent<Image>().material = cardOnTableTop.GetComponent<CardDisplay>().dissolveMaterial;

                cardOnTableTop.BroadcastMessage("Dissolve");
                Destroy(cardOnTableTop, 3f);
                StartCoroutine(HideTableTop(2.9f));

                controller.PlayCard(card);
                manager.PlayCard(card);

                manager.DeductCardPoints(card.cardCost);
            }
            else {
                DisplayNotEnoughCardPoints();
                CancelPlayCard();
            }

            
        }
    }

    void DisplayNotEnoughCardPoints()  {
        GameObject NoCardPointsText = Instantiate(NotEnoughCardPointsText, cardOnTableTop.transform.position, Quaternion.identity);
        GameObject gameHUD = GameObject.Find("GameHUDCanvas");
        NoCardPointsText.transform.SetParent(gameHUD.transform);
        NoCardPointsText.GetComponent<DissolveAttributes>().Dissolve();
        Destroy(NoCardPointsText, 2);
    }

    IEnumerator HideTableTop(float seconds){
        yield return new WaitForSeconds(seconds);
        this.gameObject.SetActive(false);
    }

    // CancelPlayCardButton calls this
    public void CancelPlayCard(){
        if (this.gameObject.name == "Tabletop" && cardOnTableTop != null) {
            GameObject HandPanel = GameObject.Find("Hand");
            this.transform.GetChild(0).transform.SetParent(HandPanel.transform);
            // Return card to player's hand
            this.gameObject.SetActive(false);
            PlayCardPanel.SetActive(false);
            cardOnTableTop = null;

            StartCoroutine(HideTableTop(0.3f));
        }
    }

    public void SubtractCost(int costToSubtract)  {
        print(costToSubtract + " subtracted from available spending.");
    }
}
