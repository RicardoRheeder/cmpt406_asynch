using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using cakeslice; //for Outline effect package
public class PlayerController : MonoBehaviour {

   //public Tilemap tileMap;
    private CardController deck;
    private GameManager manager;
    private BoardController boardController;

    private TMP_Text unitDisplayHealth;
    private TMP_Text unitDisplayArmour;
    private TMP_Text unitDisplayRange;
    private TMP_Text unitDisplayDamage;
    private TMP_Text unitDisplayAOE;
    private TMP_Text unitDisplayPierce;
    private TMP_Text unitDisplayMovementSpeed;

    private Button attackButton;
    private Button movementButton;

    private UnitStats selectedUnit;

    private bool initialized = false;

    private bool isMoving = false;
    private bool isAttacking = false;

    public void Initialize(GameManager manager, CardController deck, BoardController board) {
        this.deck = deck;
        this.manager = manager;
        this.boardController = board;
        unitDisplayHealth = GameObject.Find("unitDisplayHealth").GetComponent<TMP_Text>();
        unitDisplayArmour = GameObject.Find("unitDisplayArmour").GetComponent<TMP_Text>();
        unitDisplayRange = GameObject.Find("unitDisplayRange").GetComponent<TMP_Text>();
        unitDisplayAOE = GameObject.Find("unitDisplayAOE").GetComponent<TMP_Text>();
        unitDisplayDamage = GameObject.Find("unitDisplayDamage").GetComponent<TMP_Text>();
        unitDisplayMovementSpeed = GameObject.Find("unitDisplaySpeed").GetComponent<TMP_Text>();
        unitDisplayPierce = GameObject.Find("unitDisplayPierce").GetComponent<TMP_Text>();

        attackButton = GameObject.Find("AttackButton").GetComponent<Button>();
        attackButton.onClick.AddListener(AttackButton);

        movementButton = GameObject.Find("MovementButton").GetComponent<Button>();
        movementButton.onClick.AddListener(MovementButton);

        initialized = true;
    }

    void Update() {
        if(initialized) {
            InputController();
        }
    }

    private void InputController() {
        Vector2Int tilePos = boardController.MousePosToCell();
        if (Input.GetMouseButtonDown(0)) {
            if(isMoving) {
                manager.MoveUnit(selectedUnit.Position,tilePos);
                isMoving = false;
            }
            else if(isAttacking) {
                manager.AttackUnit(selectedUnit.Position,tilePos);
                isAttacking = false;
            }
            if (manager.GetUnitOnTile(tilePos, out UnitStats unit)) {
                selectedUnit = unit;
                UpdateUnitDisplay(selectedUnit);
            }
            else { //Remove this in the future
                selectedUnit = UnitFactory.GetBaseUnit(UnitType.claymore);
                UpdateUnitDisplay(selectedUnit);
            }
        }
    }

    private void MovementButton() {
        isMoving = true;
    }

    private void AttackButton() {
        isAttacking = true;
    }


    //private void HighlightTile(Vector3Int pos, int range)
    //{
    //    print("In highlight");
    //    List<Vector3Int> temp = HexUtility.HexReachable(pos, range, tileMap, true);
    //    foreach (var t in temp)
    //    {
    //        HexTile tile = boardController.GetHexTile(t);
    //        GameObject go = tile.GetTileObject();
    //        go.GetComponent<Outline>().enabled = true;
    //    }
    //}

    private void UpdateUnitDisplay(UnitStats unit) {
        //Strings to display the information
        string hp = "" + unit.CurrentHP + " / " + unit.MaxHP;
        string armour = "" + unit.Armour;
        string range = "" + unit.Range;
        string damage = "" + unit.Damage;
        string aoe = "" + unit.Aoe;
        string pierce = ""+unit.Pierce;
        string movementSpeed = ""+unit.MovementSpeed;

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
