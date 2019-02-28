using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using cakeslice; //for Outline effect package
public class PlayerController : MonoBehaviour {

   
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

    //private List<Vector3Int> highlightedMovementTiles; //test

    private GameObject tileObject; //test

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

        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int tempPos = boardController.MousePosToCell(Input.mousePosition);
            
        }
        
        if (initialized) {
            InputController();
        }
    }

    private void InputController() {
        Vector2Int tilePos = boardController.MousePosToCell();
        Vector3Int tempPos = boardController.MousePosToCell(Input.mousePosition);  //using Vector3Int because boardController.GetHexTile takes Vector3Int
        if (Input.GetMouseButtonDown(0)) {
            HighlightTile(tempPos);

            if (isMoving) {
                manager.MoveUnit(selectedUnit.Position,tilePos);
                isMoving = false;
            }
            else if(isAttacking) {
                manager.AttackUnit(selectedUnit.Position,tilePos);
                isAttacking = false;
            }
            else if (manager.GetUnitOnTile(tilePos, out UnitStats unit)) {
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
        isMoving = !isMoving;
        isAttacking = false;
    }

    private void AttackButton() {
        isAttacking = !isAttacking;
        isMoving = false;
    }


    //Feel free to make changes as necessary -jp
    //This function highlights a single tile. And disables the previous highlighted when another tile is selected
    private void HighlightTile(Vector3Int pos){

        if (boardController.GetTilemap().HasTile(pos)){
            //if tileObject is not null that means a previous tile is highlighted
            if (tileObject != null){
               
                    DisableHighlight(tileObject); //so, disable that object
                
            }
               HexTile tile = boardController.GetHexTile(pos); //get the Hex tile using Vector3Int position
               tileObject = tile.GetTileObject(); //get the tile game object 
            
            //check if tileObject has the Outline component attached -- using cakeslice.Outline because it was giving this error "'Outline' is an ambiguous reference between 'cakeslice.Outline' and 'UnityEngine.UI.Outline'"
            if (tileObject.GetComponent<cakeslice.Outline>()){
                EnableHighlight(tileObject);
            }
            else {
                tileObject = null;  //this is for tiles that do not have the Outline component from the prefab but are stored in tileObject. Might remove in future 
            }
        }
    }


    //enables the outline script component on the tile game object 
    private void EnableHighlight(GameObject go){
        if (go.GetComponent<cakeslice.Outline>())
        {
            go.GetComponent<cakeslice.Outline>().enabled = true;
        }
    }
    //disables the outline script component on the tile game object 
    private void DisableHighlight(GameObject go){
        if (go.GetComponent<cakeslice.Outline>())
        {
            go.GetComponent<cakeslice.Outline>().enabled = false;
        }
    }




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
