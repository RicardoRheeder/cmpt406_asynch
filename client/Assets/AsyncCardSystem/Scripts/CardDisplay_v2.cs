using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDisplay_v2 : MonoBehaviour
{
    public Card card;

    public Text nameText;

    public Text effectText;
    public Text descriptionText;

    public Image artworkImage;

    public Text cardCostText;
    public Text attackText;
    public Text healthText;

    private Vector3 scale;
    private int layer;

    private float OffsetX;
    private float OffsetY;

    public Material standardMaterial;
    public Material dissolveMaterial;


    // Use this for initialization
    public void setCard(Card aCard)
    {
        card = aCard;

        nameText.text = card.name;
        descriptionText.text = card.description;

        artworkImage.sprite = card.artwork;
        effectText.text = "";
        foreach (String effect in card.effects)
        {
            effectText.text += effect + '\n';
        }
        cardCostText.text = card.cardCost.ToString();
//        attackText.text = card.attack.ToString();
//        healthText.text = card.health.ToString();



    }

    void Start()
    {
        this.transform.Find("CardTemplate").GetComponent<Image>().material = standardMaterial;
    }

    void OnMouseUp()
    {
        this.transform.rotation = this.transform.rotation;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
    }



}
