using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 649
public class CardDisplay : MonoBehaviour {
    public Card card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI unitNameText;

    public TextMeshProUGUI effectText;
    public TextMeshProUGUI descriptionText;

    public Image artworkImage;

    public TextMeshProUGUI cardCostText;

    private Vector3 scale;
    private int layer;

    private float OffsetX;
    private float OffsetY;

    [SerializeField]private Material standardMaterial_Grey;
    [SerializeField] private Material dissolveMaterial_Grey;

    [SerializeField] private Material standardMaterial_Blue;
    [SerializeField] private Material dissolveMaterial_Blue;

    [SerializeField] private Material standardMaterial_Red;
    [SerializeField] private Material dissolveMaterial_Red;

    [SerializeField] private Material standardMaterial_Yellow;
    [SerializeField] private Material dissolveMaterial_Yellow;

    [SerializeField] private Material standardMaterial_Green;
    [SerializeField] private Material dissolveMaterial_Green;

    public Material standardMaterial;
    public Material dissolveMaterial;

    private Transform cardsArtObject;

    // Use this for initialization
    public void SetCard(Card aCard) {
        card = aCard;
        if (card.type != UnitType.none && card.type != UnitType.light_adren && card.type != UnitType.support_sandman && card.type != UnitType.heavy_albarn && card.type != UnitType.piercing_tungsten){
            unitNameText.text = (UnitMetadata.ReadableNames[card.type]);
        }
        else if (card.type == UnitType.none){
            unitNameText.text = "Any Unit";
        }
        else {
            unitNameText.text = "Invalid Card Unit Name";
        }
        nameText.text = card.displayName;
        descriptionText.text = card.description;

        artworkImage.sprite = card.artwork;
        effectText.text = "";
        foreach (String effect in card.effects) {
            effectText.text += effect + '\n';
        }
        cardCostText.text = card.cardCost.ToString();

        if (card.type.Equals(UnitType.trooper) || card.type.Equals(UnitType.recon) || card.type.Equals(UnitType.light_adren)) {
            standardMaterial = standardMaterial_Yellow;
            dissolveMaterial = dissolveMaterial_Yellow;
        }
        else if (card.type.Equals(UnitType.steamer) || card.type.Equals(UnitType.pewpew) || card.type.Equals(UnitType.heavy_albarn)) {
            standardMaterial = standardMaterial_Blue;
            dissolveMaterial = dissolveMaterial_Blue;
        }
        else if (card.type.Equals(UnitType.compensator) || card.type.Equals(UnitType.foundation) || card.type.Equals(UnitType.piercing_tungsten)) {
            standardMaterial = standardMaterial_Red;
            dissolveMaterial = dissolveMaterial_Red;
        }
        else if (card.type.Equals(UnitType.powerSurge) || card.type.Equals(UnitType.claymore) || card.type.Equals(UnitType.midas) || card.type.Equals(UnitType.support_sandman)) {
            standardMaterial = standardMaterial_Green;
            dissolveMaterial = dissolveMaterial_Green;
        }
        else {
            standardMaterial = standardMaterial_Grey;
            dissolveMaterial = dissolveMaterial_Grey;
        }

        if (cardsArtObject == null) {
            cardsArtObject = this.transform.Find("CardTemplate");
        }
        cardsArtObject.GetComponent<Image>().material = standardMaterial;
    }

    void Start() {
        cardsArtObject = this.transform.Find("CardTemplate");
    }

    void OnMouseUp() {
        this.transform.rotation = this.transform.rotation;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
    }
}
