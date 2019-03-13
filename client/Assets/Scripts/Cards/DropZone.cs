using System;
using UnityEngine;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public GameObject go;

    private PlayerController controller;
    private CardSystemManager manager;

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

        if (droppedCard.GetComponent<Draggable>().allowDrag == true) {
            Draggable d = droppedCard.GetComponent<Draggable>();
            if (d != null) {
                d.parentToReturnTo = this.transform;
            }

            if (this.gameObject.name == "Tabletop") {
                Transform cardTemp = droppedCard.transform.Find("CardTemplate");
                cardTemp.gameObject.GetComponent<Image>().material = droppedCard.GetComponent<CardDisplay>().dissolveMaterial;

                droppedCard.BroadcastMessage("Dissolve");
                Destroy(droppedCard, 3f);

                Card card = droppedCard.GetComponent<CardDisplay>().card;
                controller.PlayCard(card);
                manager.PlayCard(card);
            }
        }
    }

    public void SubtractCost(int costToSubtract)  {
        print(costToSubtract + " subtracted from available spending.");
    }
}
