using System;
using UnityEngine;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject go;

    public void OnPointerEnter(PointerEventData eventData) {
        //Debug.Log("OnPointerEnter");
        if(eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();

        if(d != null) {
            d.placeholderParent = this.transform;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        //Debug.Log("OnPointerExit");
        if(eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if(d != null && d.placeholderParent==this.transform) {
            d.placeholderParent = d.parentToReturnTo;
        }
    }

    
    public void OnDrop(PointerEventData eventData) {
        GameObject droppedCard = eventData.pointerDrag.gameObject;

        if (droppedCard.GetComponent<Draggable>().allowDrag == true)
        {
            Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);
            Draggable d = droppedCard.GetComponent<Draggable>();
            if (d != null)
            {
                d.parentToReturnTo = this.transform;
            }

            if (this.gameObject.name == "Tabletop")
            {
                Transform cardTemp = droppedCard.transform.Find("CardTemplate");
                cardTemp.gameObject.GetComponent<Image>().material = droppedCard.GetComponent<CardDisplay_v2>().dissolveMaterial;
                //            eventData.pointerDrag.transform.GetChild(0).GetComponent<Image>().material = eventData.pointerDrag.GetComponent<CardDisplay_v2>().dissolveMaterial;


                droppedCard.BroadcastMessage("dissolve");
                Destroy(droppedCard, 3f);


                sendToGameManager(droppedCard.GetComponent<CardDisplay_v2>().card.name);
                //            subtractCost(droppedCard.GetComponent<CardDisplay_v2>().cost);
            }
        }
       

    }

    public void sendToGameManager(String cardName)
    {
        print(cardName + " sent to GameManager!");
    }

    public void subtractCost(int costToSubtract)
    {
        print(costToSubtract + " subtracted from available spending.");
    }
}
