using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;

using cakeslice; //for Outline effect package
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

   
    private CardController deck;
    private GameManager manager;
    private BoardController boardController;
    private GameBuilder builder;

    private TMP_Text unitDisplayHealth;
    private TMP_Text unitDisplayArmour;
    private TMP_Text unitDisplayRange;
    private TMP_Text unitDisplayDamage;
    private TMP_Text unitDisplayAOE;
    private TMP_Text unitDisplayPierce;
    private TMP_Text unitDisplayMovementSpeed;
    private TMP_Text unitDisplayName;

    private Button attackButton;
    private Button movementButton;

    private UnitStats selectedUnit;

    private bool initialized = false;

    private bool isMoving = false;
    private bool isAttacking = false;

    private enum PlayerState {
        playing,
        placing
    };
    private PlayerState controllerState;
    private List<int> armyPreset;
    private List<GameObject> presetTexts;
    private List<UnitStats> placedUnits;
    private int armyPositionCount = 0;
    private SpawnPoint spawnPoint;
    private bool donePlacing = false;
    private bool spawnZoneHighlighted = false;

    private List<Vector2Int> highlightedTiles;

    public void Initialize(GameManager manager, CardController deck, GameBuilder builder, BoardController board, bool isPlacing, ArmyPreset armyPreset = null, List<GameObject> presetTexts = null, SpawnPoint spawnPoint = SpawnPoint.none) {
        this.deck = deck;
        this.manager = manager;
        this.boardController = board;
        this.builder = builder;
        this.spawnPoint = spawnPoint;

        if(isPlacing) {
            controllerState = PlayerState.placing;
            this.armyPreset = new List<int>() {
                armyPreset.General,
            };
            this.armyPreset.AddRange(armyPreset.Units);
            this.presetTexts = presetTexts;
        }
        else {
            controllerState = PlayerState.playing;
            unitDisplayHealth = GameObject.Find("unitDisplayHealth").GetComponent<TMP_Text>();
            unitDisplayArmour = GameObject.Find("unitDisplayArmour").GetComponent<TMP_Text>();
            unitDisplayRange = GameObject.Find("unitDisplayRange").GetComponent<TMP_Text>();
            unitDisplayAOE = GameObject.Find("unitDisplayAOE").GetComponent<TMP_Text>();
            unitDisplayDamage = GameObject.Find("unitDisplayDamage").GetComponent<TMP_Text>();
            unitDisplayMovementSpeed = GameObject.Find("unitDisplayMove").GetComponent<TMP_Text>();
            unitDisplayPierce = GameObject.Find("unitDisplayPierce").GetComponent<TMP_Text>();
            unitDisplayName = GameObject.Find("unitName").GetComponent<TMP_Text>();

            attackButton = GameObject.Find("AttackButton").GetComponent<Button>();
            attackButton.onClick.AddListener(AttackButton);

            movementButton = GameObject.Find("MovementButton").GetComponent<Button>();
            movementButton.onClick.AddListener(MovementButton);
        }

        initialized = true;
    }

    void Update() {
        if (initialized && !donePlacing) {
            if(!spawnZoneHighlighted) {
                boardController.HighlightSpawnZone(spawnPoint);
                spawnZoneHighlighted = true;
            }
            InputController();
        }
        if (donePlacing) {
            manager.EndUnitPlacement();
        }
    }

    private void InputController() {
        Vector2Int tilePos = boardController.MousePosToCell();
        switch(controllerState) {
            case (PlayerState.playing):
                if (Input.GetMouseButtonDown(0)) {
                    if (!EventSystem.current.IsPointerOverGameObject()) {
                        if (isMoving) {
                            if (highlightedTiles.Any(tile => tile.Equals(tilePos))) {
                                Debug.Log("here");
                                manager.MoveUnit(selectedUnit.Position, tilePos);
                                isMoving = false;
                                boardController.ClearHighlighting();
                            }
                        }
                        else if (isAttacking) {
                            if (highlightedTiles.Any(tile => tile.Equals(tilePos))) {
                                Debug.Log("here");
                                manager.AttackUnit(selectedUnit.Position, tilePos);
                                isAttacking = false;
                                boardController.ClearHighlighting();
                            }
                        }
                        else if (manager.GetUnitOnTileUserOwns(tilePos, out UnitStats unit)) {
                            selectedUnit = unit;
                            UpdateUnitDisplay(selectedUnit);
                        }
                    }
                }
                break;
            case (PlayerState.placing):
                if (presetTexts.Count == 0) {
                    donePlacing = true;
                }
                if (Input.GetMouseButtonDown(0)) {
                    if (!EventSystem.current.IsPointerOverGameObject()) {
                        if (boardController.CellIsSpawnTile(spawnPoint, tilePos) && !manager.TileContainsUnit(tilePos)) {
                            int unit = armyPreset[armyPositionCount];
                            armyPositionCount++;
                            manager.CreateUnitAtPos(tilePos, unit);
                            GameObject unitText = presetTexts[0];
                            Destroy(unitText);
                            presetTexts.RemoveAt(0);
                        }
                    }
                }
                break;
            default:
                Debug.Log("Player controller is in an invalid state");
                break;
        }
    }

    private void MovementButton() {
        if(selectedUnit != null) {
            if(isMoving) {
                boardController.ClearHighlighting();
            }
            else {
                this.highlightedTiles = boardController.GetTilesWithinMovementRange(selectedUnit.Position, selectedUnit.MovementSpeed);
                boardController.HighlightTiles(this.highlightedTiles);
            }
            isMoving = !isMoving;
            isAttacking = false;
        }
    }

    private void AttackButton() {
        if(selectedUnit != null) {
            if(isAttacking) {
                boardController.ClearHighlighting();
            }
            else {
                this.highlightedTiles = boardController.GetTilesWithinAttackRange(selectedUnit.Position, selectedUnit.Range);
                boardController.HighlightTiles(this.highlightedTiles);
            }
            isAttacking = !isAttacking;
            isMoving = false;
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
        unitDisplayName.text = unit.GetDisplayName();
    }
}
