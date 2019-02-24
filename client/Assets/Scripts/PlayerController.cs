using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Tilemaps;
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

    private bool initialized = false;

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

        initialized = true;
    }

    void Update() {
        if(initialized) {
            print("initileized is true");
            InputController();
        }
    }

    private void InputController() {
        Vector3Int tilePos = boardController.MousePosToCell(Input.mousePosition);
        
        //test
        if (Input.GetMouseButtonDown(0)) { //on mouse left click
            //print("should call highlight");
            //HighlightTile(tilePos, 5);
            if (manager.GetUnitOnTile((Vector2Int)tilePos, out UnitStats unit))
                UpdateUnitDisplay(unit);
            else
                UpdateUnitDisplay(UnitFactory.GetBaseUnit(UnitType.claymore));
        }
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
