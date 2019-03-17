using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CardSystemManager cardSystem;

    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;

    GameObject placeholder = null;

    public bool allowDrag = false;

    private GameObject tableTop;

    private Vector3 defaultScale;

//    private GameObject cardDisplayPanel;
    private GameObject tempCardDisplaying;

    void Start()
    {
        cardSystem = FindObjectOfType<CardSystemManager>();
    }

    public void SetTableTop(GameObject tableTop) {
        this.tableTop = tableTop;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        tableTop.SetActive(true);

        // The following makes sure that you are only able to drag cards that belong to your hand
        if (eventData.pointerDrag.transform.parent.name == "Hand" ) {
            allowDrag = true;
        }

        if (allowDrag == true) {
            placeholder = new GameObject();
            placeholder.transform.SetParent(this.transform.parent);
            LayoutElement le = placeholder.AddComponent<LayoutElement>();
            le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
            le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;

            placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

            parentToReturnTo = this.transform.parent;
            placeholderParent = parentToReturnTo;
            this.transform.SetParent(this.transform.parent.parent);

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

    }
    
    public void OnDrag(PointerEventData eventData){
        cardSystem.cardBeingDragged = true;
        if (allowDrag == true) {

            this.transform.position = eventData.position;
             
            if (this.transform.parent.GetComponent<RectTransform>() != null) {
                this.GetComponent<RectTransform>().rotation = placeholder.transform.parent.GetComponent<RectTransform>().rotation;
            }

            if (placeholder.transform.parent != placeholderParent) {
                placeholder.transform.SetParent(placeholderParent);
            }

            int newSiblingIndex = placeholderParent.childCount;

            for (int i = 0; i < placeholderParent.childCount; i++) {
                if (this.transform.position.x < placeholderParent.GetChild(i).position.x) {

                    newSiblingIndex = i;

                    if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                        newSiblingIndex--;

                    break;
                }
            }

            placeholder.transform.SetSiblingIndex(newSiblingIndex);
        }
    }
    
    public void OnEndDrag(PointerEventData eventData){
        cardSystem.cardBeingDragged = false;
        if (allowDrag == true) {
            this.transform.SetParent(parentToReturnTo);
            this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            GetComponent<CanvasGroup>().blocksRaycasts = true;

            Destroy(placeholder);
        }

        // Deals with the enlarged dummy display of the card
        if (tempCardDisplaying != null){
            this.transform.localScale = defaultScale;
            Destroy(tempCardDisplaying);
            cardSystem.cardDisplayPanel.SetActive(false);
        }
    }



    public void OnPointerEnter(PointerEventData eventData){
        if (!cardSystem.cardBeingDragged){
            defaultScale = this.transform.localScale;
            this.transform.localScale *= 1.2f;

            tempCardDisplaying = GameObject.Instantiate(this.gameObject) as GameObject;
            cardSystem.cardDisplayPanel.SetActive(true);
            tempCardDisplaying.transform.SetParent(cardSystem.cardDisplayPanel.transform);
            tempCardDisplaying.transform.localScale *= 4f;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
//        print("Exiting");
        if (tempCardDisplaying != null && placeholder == null){
            this.transform.localScale = defaultScale;
            Destroy(tempCardDisplaying);
            cardSystem.cardDisplayPanel.SetActive(false);
        }
    }

}
