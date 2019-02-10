using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    private CardController deck;

    //public Tilemap tilemap;

    private GameManager manager;

    public TMP_Text unitDisplayHealth;
    public TMP_Text unitDisplayArmour;
    public TMP_Text unitDisplayRange;
    public TMP_Text unitDisplayDamage;
    public TMP_Text unitDisplayAOE;
    public TMP_Text unitDisplayPierce;
    public TMP_Text unitDisplayMovementSpeed;


    void Awake()
    {

    }

    public PlayerController(GameManager manager, CardController deck) {
        this.deck = deck;
        this.manager = manager;
    }

    public void Initialize(GameManager manager, CardController deck)
    {
        this.deck = deck;
        this.manager = manager;
    }


    void Update()
    {
      
        InputController();
    }

    private void InputController()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3Int tilePos = tilemap.WorldToCell(mousePos);

        //test
        if (Input.GetMouseButtonDown(0))
        {
            //manager.GetUnitOnTile(tilepos);
            UnitStats unit = manager.GetUnitOnTile(mousePos);
            UpdateUnitDisplay(unit);
            
            // print(tilePos);
          
        }

    }

    private void UpdateUnitDisplay(UnitStats unit)
    {
        //Strings to display the information
        string hp = "Health: " + unit.currentHP + " / " + unit.maxHP;
        string armour = "Armour: " + unit.armour;
        string range = "Range: " + unit.range;
        string damage = "Damage: " + unit.damage;
        string aoe = "AOE: " + unit.aoe;
        string pierce = "Pierce: " + unit.pierce;
        string movementSpeed = "Movement Speed: " + unit.movementSpeed;

        //Finders to find which text to change for what attribute
        unitDisplayHealth = GameObject.Find("unitDisplayHealth").GetComponent<TMP_Text>();
        unitDisplayHealth.text = hp;

        unitDisplayArmour = GameObject.Find("unitDisplayArmour").GetComponent<TMP_Text>();
        unitDisplayArmour.text = armour;

        unitDisplayRange = GameObject.Find("unitDisplayRange").GetComponent<TMP_Text>();
        unitDisplayRange.text = range;

        unitDisplayAOE = GameObject.Find("unitDisplayAOE").GetComponent<TMP_Text>();
        unitDisplayAOE.text = aoe;

        unitDisplayDamage = GameObject.Find("unitDisplayDamage").GetComponent<TMP_Text>();
        unitDisplayDamage.text = damage;

        unitDisplayMovementSpeed = GameObject.Find("unitDisplayPierce").GetComponent<TMP_Text>();
        unitDisplayMovementSpeed.text = movementSpeed;

        unitDisplayPierce = GameObject.Find("unitDisplayPierce").GetComponent<TMP_Text>();
        unitDisplayPierce.text = pierce;
    }


}
