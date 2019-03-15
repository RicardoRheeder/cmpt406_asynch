using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour {
    public Card card;

    public TextMeshProUGUI nameText;

    public TextMeshProUGUI effectText;
    public TextMeshProUGUI descriptionText;

    public Image artworkImage;

    public TextMeshProUGUI cardCostText;

    private Vector3 scale;
    private int layer;

    private float OffsetX;
    private float OffsetY;

    public Material standardMaterial;
    public Material dissolveMaterial;


    // Use this for initialization
    public void SetCard(Card aCard) {
        card = aCard;

        nameText.text = card.displayName;
        descriptionText.text = card.description;

        artworkImage.sprite = card.artwork;
        effectText.text = "";
        foreach (String effect in card.effects)
        {
            effectText.text += effect + '\n';
        }
        cardCostText.text = card.cardCost.ToString();
    }

    void Start() {
        this.transform.Find("CardTemplate").GetComponent<Image>().material = standardMaterial;
    }

    void OnMouseUp() {
        this.transform.rotation = this.transform.rotation;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
    }


}
