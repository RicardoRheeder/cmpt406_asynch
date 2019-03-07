using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
	public Transform parentToReturnTo = null;
	public Transform placeholderParent = null;

	GameObject placeholder = null;

    public bool allowDrag = false;
	


	public void OnBeginDrag(PointerEventData eventData) {

        if (eventData.pointerDrag.transform.parent.name == "Hand" ||
            eventData.pointerDrag.transform.parent.name == "Tabletop")
        {
            allowDrag = true;
        }

        if (allowDrag == true)
        {
            Debug.Log("OnBeginDrag");

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
	
	public void OnDrag(PointerEventData eventData) {
        if (allowDrag == true)
        {
            //Debug.Log ("OnDrag");

            this.transform.position = eventData.position;

//            this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
////            this.transform.rotation.Set(-30.84f, this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w); 
//            this.transform.position = new Vector3(this.transform.position.x+12, this.transform.position.y, this.transform.position.z );

             
            if (this.transform.parent.GetComponent<RectTransform>() != null)
            {
                this.GetComponent<RectTransform>().rotation = placeholder.transform.parent.GetComponent<RectTransform>().rotation;

            }

            if (placeholder.transform.parent != placeholderParent)
            {
                placeholder.transform.SetParent(placeholderParent);
            }

            int newSiblingIndex = placeholderParent.childCount;

            for (int i = 0; i < placeholderParent.childCount; i++)
            {
                if (this.transform.position.x < placeholderParent.GetChild(i).position.x)
                {

                    newSiblingIndex = i;

                    if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                        newSiblingIndex--;

                    break;
                }
            }

            placeholder.transform.SetSiblingIndex(newSiblingIndex);
        }
    }
	
	public void OnEndDrag(PointerEventData eventData) {
        if (allowDrag == true)
        {
            //		Debug.Log ("OnEndDrag");
            this.transform.SetParent(parentToReturnTo);
            this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            GetComponent<CanvasGroup>().blocksRaycasts = true;

            Destroy(placeholder);
        }

	}
	
	
	
}
