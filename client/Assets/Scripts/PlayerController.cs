using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {

    private CardController deck;
    private GameManager manager;
    public Tilemap tilemap;

    public TMP_Text unitDisplayHealth;
    public TMP_Text unitDisplayArmour;
    public TMP_Text unitDisplayRange;
    public TMP_Text unitDisplayDamage;
    public TMP_Text unitDisplayAOE;
    public TMP_Text unitDisplayPierce;
    public TMP_Text unitDisplayMovementSpeed;

    public void Initialize(GameManager manager, CardController deck) {
        this.deck = deck;
        this.manager = manager;
        unitDisplayHealth = GameObject.Find("unitDisplayHealth").GetComponent<TMP_Text>();
        unitDisplayArmour = GameObject.Find("unitDisplayArmour").GetComponent<TMP_Text>();
        unitDisplayRange = GameObject.Find("unitDisplayRange").GetComponent<TMP_Text>();
        unitDisplayAOE = GameObject.Find("unitDisplayAOE").GetComponent<TMP_Text>();
        unitDisplayDamage = GameObject.Find("unitDisplayDamage").GetComponent<TMP_Text>();
        unitDisplayMovementSpeed = GameObject.Find("unitDisplayPierce").GetComponent<TMP_Text>();
        unitDisplayPierce = GameObject.Find("unitDisplayPierce").GetComponent<TMP_Text>();
    }

    void Update() {
        InputController();
    }

    private void InputController() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        
        //test
        if (Input.GetMouseButtonDown(0)) { //on mouse left click
            print(tilePos);
           
            UnitStats unit = manager.GetUnitOnTile(tilePos);
            if(unit != null)
                UpdateUnitDisplay(unit);
        }
    }

    private void UpdateUnitDisplay(UnitStats unit) {
        //Strings to display the information
        string hp = "Health: " + unit.currentHP + " / " + unit.maxHP;
        string armour = "Armour: " + unit.armour;
        string range = "Range: " + unit.range;
        string damage = "Damage: " + unit.damage;
        string aoe = "AOE: " + unit.aoe;
        string pierce = "Pierce: " + unit.pierce;
        string movementSpeed = "Movement Speed: " + unit.movementSpeed;

        //Finders to find which text to change for what attribute
        unitDisplayHealth.text = hp;
        unitDisplayArmour.text = armour;
        unitDisplayRange.text = range;
        unitDisplayAOE.text = aoe;
        unitDisplayDamage.text = damage;
        unitDisplayMovementSpeed.text = movementSpeed;
        unitDisplayPierce.text = pierce;
    }
}
