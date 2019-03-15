using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;

using cakeslice; //for Outline effect package
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour {

    //Constants
    private Color BUTTON_ACTIVE = Color.yellow;
    private Color BUTTON_INACTIVE = Color.gray;
   
    //Controllers
    private CardController deck;
    private GameManager manager;
    private BoardController boardController;
    private FogOfWarController fogOfWarController;
    private GameBuilder builder;
    private GameState gamestate;
    private AudioManager audioManager;

    //Stuff for unit stat display pane
    private TMP_Text unitDisplayHealth;
    private TMP_Text unitDisplayArmour;
    private TMP_Text unitDisplayRange;
    private TMP_Text unitDisplayDamage;
    private TMP_Text unitDisplayAOE;
    private TMP_Text unitDisplayPierce;
    private TMP_Text unitDisplayMovementSpeed;
    private TMP_Text unitDisplayName;

    //The action buttons
    private GameObject actionsName;
    private Button attackButton;
    private GameObject attackButtonObject;
    private Button movementButton;
    private GameObject movementButtonObject;

    //General info panel
    private GameObject generalName;
    private TMP_Text generalNameText;
    private GameObject Ability1Object;
    private Button Ability1Button;
    private GameObject Ability2Object;
    private Button Ability2Button;

    //Stored ability functions
    private UnityAction ability1Function;
    private UnityAction ability2Function;

    //Stuff for top right information on hud
    private TMP_Text userTurnText;
    private TMP_Text turnText;

    //menu buttons
    private Button concedeButton;
    private Button closeGameButton;
    
    private UnitStats selectedUnit;

    private bool initialized = false;

    private Color tempColor;
    private enum PlayerState {
        playing,
        placing
    };
    private PlayerState controllerState;
    private enum InteractionState {
        none,
        ability1,
        ability2,
        moving,
        attacking,
        playingCard
    };
    private InteractionState interactionState;
    private List<int> armyPreset;
    private List<GameObject> presetTexts;
    private List<UnitStats> placedUnits;
    private int armyPositionCount = 0;
    private SpawnPoint spawnPoint;
    private bool donePlacing = false;
    private bool spawnZoneHighlighted = false;
    private bool isPlacing;
    private string username;
    private Card cardBeingPlayed;

    private List<Vector2Int> highlightedTiles;

    public void Initialize(GameManager manager, AudioManager audioManager, string username, GameState gamestate, CardController deck, GameBuilder builder, BoardController board, FogOfWarController fogOfWarController, bool isPlacing, ArmyPreset armyPreset = null, List<GameObject> presetTexts = null, SpawnPoint spawnPoint = SpawnPoint.none) {
        this.deck = deck;
        this.manager = manager;
        this.boardController = board;
        this.fogOfWarController = fogOfWarController;
        this.builder = builder;
        this.spawnPoint = spawnPoint;
        this.isPlacing = isPlacing;
        this.username = username;
        this.audioManager = audioManager;

        if (isPlacing) {
            controllerState = PlayerState.placing;
            this.armyPreset = new List<int>() {
                armyPreset.General,
            };
            this.armyPreset.AddRange(armyPreset.Units);
            this.presetTexts = presetTexts;
        }
        else {
            controllerState = PlayerState.playing;
            interactionState = InteractionState.none;
            unitDisplayHealth = GameObject.Find("unitDisplayHealth").GetComponent<TMP_Text>();
            unitDisplayArmour = GameObject.Find("unitDisplayArmour").GetComponent<TMP_Text>();
            unitDisplayRange = GameObject.Find("unitDisplayRange").GetComponent<TMP_Text>();
            unitDisplayAOE = GameObject.Find("unitDisplayAOE").GetComponent<TMP_Text>();
            unitDisplayDamage = GameObject.Find("unitDisplayDamage").GetComponent<TMP_Text>();
            unitDisplayMovementSpeed = GameObject.Find("unitDisplayMove").GetComponent<TMP_Text>();
            unitDisplayPierce = GameObject.Find("unitDisplayPierce").GetComponent<TMP_Text>();
            unitDisplayName = GameObject.Find("unitName").GetComponent<TMP_Text>();
            userTurnText = GameObject.Find("GameUserTurnText").GetComponent<TMP_Text>();
            turnText = GameObject.Find("GameTurnsText").GetComponent<TMP_Text>();

            actionsName = GameObject.Find("ActionName");
            attackButtonObject = GameObject.Find("AttackButton");
            attackButton = attackButtonObject.GetComponent<Button>();
            attackButton.onClick.AddListener(AttackButton);
            movementButtonObject = GameObject.Find("MovementButton");
            movementButton = movementButtonObject.GetComponent<Button>();
            movementButton.onClick.AddListener(MovementButton);
            
            turnText.text = "Turn " + gamestate.TurnNumber;
            userTurnText.text = gamestate.UsersTurn + "'s Turn";

            generalName = GameObject.Find("GeneralName");
            generalNameText = generalName.GetComponent<TMP_Text>();
            Ability1Object = GameObject.Find("AbilityOneButton");
            Ability1Button = Ability1Object.GetComponent<Button>();
            Ability2Object = GameObject.Find("AbilityTwoButton");
            Ability2Button = Ability2Object.GetComponent<Button>();

            concedeButton = GameObject.Find("ConcedeButton").GetComponent<Button>();
            concedeButton.onClick.AddListener(Forfeit);
            closeGameButton = GameObject.Find("CloseGameButton").GetComponent<Button>();
            closeGameButton.onClick.AddListener(ExitGame);
        }
        
        initialized = true;
    }

    void Update() {
        if (initialized && !donePlacing) {
            if(!spawnZoneHighlighted && isPlacing) {
                boardController.HighlightSpawnZone(spawnPoint);
                spawnZoneHighlighted = true;
            }
            InputController();
        }
        if (donePlacing) {
            manager.EndUnitPlacement();
        }
        if(selectedUnit != null) {
            if (selectedUnit.CurrentHP <= 0) {
                selectedUnit = null;
            }
            else if (selectedUnit.MyUnit.rend.material.color != Color.white){
                tempColor = selectedUnit.MyUnit.rend.material.color;
                selectedUnit.MyUnit.rend.material.color = Color.white;
            }
        }
    }

    private void InputController() {
        Vector2Int tilePos = boardController.MousePosToCell();
        boardController.HoverHighlight(tilePos);

        switch(controllerState) {
            case (PlayerState.playing):
                if (Input.GetMouseButtonDown(0)) {
                    if (!EventSystem.current.IsPointerOverGameObject()) {
                        switch (interactionState) {
                            case (InteractionState.moving):
                                if (highlightedTiles.Any(tile => tile.Equals(tilePos))) {
                                    manager.MoveUnit(selectedUnit.Position, tilePos);
                                    boardController.ClearHighlighting();
                                    interactionState = InteractionState.none;
                                }
                                break;
                            case (InteractionState.attacking):
                                if (highlightedTiles.Any(tile => tile.Equals(tilePos))) {
                                    manager.AttackUnit(selectedUnit.Position, tilePos);
                                    boardController.ClearHighlighting();
                                    interactionState = InteractionState.none;
                                }
                                break;
                            case (InteractionState.ability1):
                                interactionState = InteractionState.none;
                                if(manager.UseAbility(selectedUnit.Position, tilePos, selectedUnit.Ability1)) {
                                    Ability1Button.onClick.RemoveAllListeners();
                                    Ability1Button.GetComponent<Image>().color = BUTTON_INACTIVE;
                                }
                                boardController.ClearHighlighting();
                                break;
                            case (InteractionState.ability2):
                                interactionState = InteractionState.none;
                                if (manager.UseAbility(selectedUnit.Position, tilePos, selectedUnit.Ability2)) {
                                    Ability2Button.onClick.RemoveAllListeners();
                                    Ability2Button.GetComponent<Image>().color = BUTTON_INACTIVE;
                                }
                                boardController.ClearHighlighting();
                                break;
                            case (InteractionState.playingCard):
                                if(manager.UseCard(tilePos, cardBeingPlayed))
                                    interactionState = InteractionState.none;
                                break;
                            case (InteractionState.none):
                                if(!fogOfWarController.CheckIfTileHasFog(tilePos)) {
                                    if (manager.GetUnitOnTile(tilePos, out UnitStats unit)) {
                                        if (selectedUnit != null) {
                                            selectedUnit.MyUnit.rend.material.color = tempColor;
                                        }
                                        selectedUnit = unit;
                                    }
                                }
                                break;
                            default:
                                Debug.Log("Interaction state is in a weird place");
                                break;
                        }
                        if(selectedUnit != null) {
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
            if(interactionState == InteractionState.moving) {
                interactionState = InteractionState.none;
                boardController.ClearHighlighting();
                
            }
            else {
                interactionState = InteractionState.moving;
                this.highlightedTiles = boardController.GetTilesWithinMovementRange(selectedUnit.Position, selectedUnit.MovementSpeed);
                boardController.HighlightTiles(this.highlightedTiles);
                audioManager.Play("ButtonPress");
            }
        }
    }

    private void AttackButton() {
        if(selectedUnit != null) {
            if(interactionState == InteractionState.attacking) {
                interactionState = InteractionState.none;
                boardController.ClearHighlighting();
            }
            else {
                interactionState = InteractionState.attacking;
                this.highlightedTiles = boardController.GetTilesWithinAttackRange(selectedUnit.Position, selectedUnit.Range);
                boardController.HighlightTiles(this.highlightedTiles);
                audioManager.Play("ButtonPress");
            }
        }
    }

    public void Ability1ButtonClicked() {
        if(interactionState == InteractionState.ability1) {
            interactionState = InteractionState.none;
            boardController.ClearHighlighting();
        }
        else {
            interactionState = InteractionState.ability1;
            this.highlightedTiles = boardController.GetTilesWithinAttackRange(selectedUnit.Position, GeneralMetadata.AbilityRangeDictionary[selectedUnit.Ability1]);
            boardController.HighlightTiles(this.highlightedTiles);
            audioManager.Play("ButtonPress");
        }
    }

    public void Ability2ButtonClicked() {
        if (interactionState == InteractionState.ability2) {
            interactionState = InteractionState.none;
            boardController.ClearHighlighting();
        }
        else {
            interactionState = InteractionState.ability2;
            this.highlightedTiles = boardController.GetTilesWithinAttackRange(selectedUnit.Position, GeneralMetadata.AbilityRangeDictionary[selectedUnit.Ability2]);
            boardController.HighlightTiles(this.highlightedTiles);
            audioManager.Play("ButtonPress");
        }
    }

    public void PlayCard(Card card) {
        this.cardBeingPlayed = card;
        this.interactionState = InteractionState.playingCard;

        //Note: here we have to highlight the appropriate tiles if the card is unique to a unit
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

        if (unit.Owner == username) {
            actionsName.SetActive(true);
            attackButtonObject.SetActive(true);
            movementButtonObject.SetActive(true);
            if (unit.AttackActions != 0 && unit.Damage != 0) {
                attackButton.GetComponent<Image>().color = BUTTON_ACTIVE;
                attackButton.onClick.RemoveAllListeners();
                attackButton.onClick.AddListener(AttackButton);
            }
            else {
                attackButton.GetComponent<Image>().color = BUTTON_INACTIVE;
                attackButton.onClick.RemoveAllListeners();
            }
            if (unit.MovementActions != 0 && unit.MovementSpeed != 0) {
                movementButton.GetComponent<Image>().color = BUTTON_ACTIVE;
                movementButton.onClick.RemoveAllListeners();
                movementButton.onClick.AddListener(MovementButton);
            }
            else {
                movementButton.GetComponent<Image>().color = BUTTON_INACTIVE;
                movementButton.onClick.RemoveAllListeners();
            }
            if (unit.UnitClass == UnitClass.general) {
                generalNameText.SetText(UnitMetadata.ReadableNames[unit.UnitType]);
                Ability1Button.GetComponentInChildren<TMP_Text>().SetText(GeneralMetadata.ReadableAbilityNameDict[unit.Ability1]);
                if (unit.Ability1Cooldown == 0) {
                    Ability1Button.onClick.RemoveAllListeners();
                    Ability1Button.onClick.AddListener(Ability1ButtonClicked);
                    Ability1Button.GetComponent<Image>().color = BUTTON_ACTIVE;
                }
                else {
                    Ability1Button.GetComponent<Image>().color = BUTTON_INACTIVE;
                    Ability1Button.onClick.RemoveAllListeners();
                }
                Ability2Button.GetComponentInChildren<TMP_Text>().SetText(GeneralMetadata.ReadableAbilityNameDict[unit.Ability2]);
                if (unit.Ability2Cooldown == 0) {
                    Ability2Button.onClick.RemoveAllListeners();
                    Ability2Button.onClick.AddListener(Ability2ButtonClicked);
                    Ability2Button.GetComponent<Image>().color = BUTTON_ACTIVE;
                }
                else {
                    Ability2Button.GetComponent<Image>().color = BUTTON_INACTIVE;
                    Ability2Button.onClick.RemoveAllListeners();
                }
                generalName.SetActive(true);
                Ability1Object.SetActive(true);
                Ability2Object.SetActive(true);
            }
            else {
                generalName.SetActive(false);
                Ability1Object.SetActive(false);
                Ability2Object.SetActive(false);
            }
        }
        else {
            actionsName.SetActive(false);
            attackButtonObject.SetActive(false);
            movementButtonObject.SetActive(false);
            generalName.SetActive(false);
            Ability1Object.SetActive(false);
            Ability2Object.SetActive(false);
        }
    }

    public void Forfeit() {
        manager.Forfeit();
    }

    public void ExitGame() {
        manager.ExitGame();
    }
}
