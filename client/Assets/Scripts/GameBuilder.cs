using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameBuilder : MonoBehaviour {

    //Variables for the game manager to access
    public Dictionary<Vector2Int, UnitStats> unitPositions = new Dictionary<Vector2Int, UnitStats>();
    public Dictionary<Vector2Int, Effect> effectPositions = new Dictionary<Vector2Int, Effect>();

    //Reference to the menus
    public GameObject unitPlacementDisplayPrefab;
    public GameObject unitSelectionDisplayPrefab;
    private GameObject unitPlacementViewport;
    public List<GameObject> UnitDisplayTexts { get; private set; }

    //Prefabs for each unit for instantiation purposes
    public GameObject claymorePrefab;
    public GameObject compensatorPrefab;
    public GameObject trooperPrefab;
    public GameObject reconPrefab;
    public GameObject steamerPrefab;
    public GameObject pewpewPrefab;
    public GameObject foundationPrefab;
    public GameObject powerSurgePrefab;
    public GameObject midasPrefab;
    public GameObject heavyAlbarnPrefab;
    public GameObject piercingTungstenPrefab;
    public GameObject lightAdrenPrefab;
    public GameObject supportSandmanPrefab;
    public Dictionary<UnitType, GameObject> typePrefabStorage;
    
    private GameState state;
    private string username;
    private BoardController board;
    private FogOfWarController fogController;
    private bool isPlacing;
    private List<int> unitNumbers;
    private List<GameObject> unitTexts;
    public Dictionary<UnitStats, GameObject> UnitButtons;

    public void Awake() {
        //Populate the dictionary whenever we build the map
        typePrefabStorage = new Dictionary<UnitType, GameObject>() {
            {UnitType.claymore, claymorePrefab },
            {UnitType.compensator, compensatorPrefab },
            {UnitType.trooper, trooperPrefab },
            {UnitType.recon, reconPrefab },
            {UnitType.steamer, steamerPrefab },
            {UnitType.pewpew, pewpewPrefab },
            {UnitType.foundation, foundationPrefab },
            {UnitType.powerSurge, powerSurgePrefab },
            {UnitType.midas, midasPrefab },
            {UnitType.heavy_albarn, heavyAlbarnPrefab },
            {UnitType.piercing_tungsten, piercingTungstenPrefab },
            {UnitType.light_adren, lightAdrenPrefab },
            {UnitType.support_sandman, supportSandmanPrefab }
        };

    }

    //Method that takes in a game state, instantiates all of the objects and makes sure the scene is setup how it should be.
    //Note: the game manager is responsible for creating the other managers, the game builder is just responsible for creating the playable objects.
    public void Build(ref GameState state, string username, ref BoardController board, ref FogOfWarController fogController, bool isPlacing, ArmyPreset armyPreset = null) {
        this.state = state;
        this.username = username;
        this.board = board;
        this.fogController = fogController;
        this.isPlacing = isPlacing;
        unitPositions = new Dictionary<Vector2Int, UnitStats>();
        effectPositions = new Dictionary<Vector2Int, Effect>();

        if (armyPreset != null) {
            this.unitNumbers = new List<int>(){
                armyPreset.General
            };
            this.unitNumbers.AddRange(armyPreset.Units);
        }
        else {
            this.unitNumbers = new List<int>();
            this.unitTexts = new List<GameObject>();
            this.UnitButtons = new Dictionary<UnitStats, GameObject>();
            List<UnitStats> userGeneral = state.UserGeneralsMap[username];
            for (int i = 0; i < userGeneral.Count; i++) {
                this.unitNumbers.Add((int)userGeneral[i].UnitType);
            }
            List<UnitStats> userUnits = state.UserUnitsMap[username];
            for (int i = 0; i < userUnits.Count; i++) {
                this.unitNumbers.Add((int)userUnits[i].UnitType);
            }
        }

        SetupScene();
        if(!isPlacing) {
            InstantiateUnits();
        }
    }

    //Method responsible for instantiate the canvas, the camera rig, the light(s),
    private void SetupScene() {
        GameObject prefabToUse;
        UnitDisplayTexts = new List<GameObject>();

        if (isPlacing) {
            prefabToUse = unitPlacementDisplayPrefab;
            unitPlacementViewport = GameObject.Find("PlaceUnitsContent");
        }
        else {
            prefabToUse = unitSelectionDisplayPrefab;
            unitPlacementViewport = GameObject.Find("UnitSnapContent");
        }

        for(int i = 0; i < unitNumbers.Count; i++) {
            int unit = unitNumbers[i];
            GameObject unitText = Instantiate(prefabToUse);
            UnitDisplayTexts.Add(unitText);
            unitText.transform.SetParent(unitPlacementViewport.transform, false);
            unitText.GetComponentInChildren<TMP_Text>().text = UnitMetadata.ReadableNames[(UnitType)unit];
            if(!isPlacing) {
                this.unitTexts.Add(unitText);
            }
        }
    }

    //Method responsible for making sure all of the units are created with the appropriate gameobjects
    private void InstantiateUnits() {
        foreach (var userGeneralList in state.UserGeneralsMap) {
            SpawnPoint spawnPoint = SpawnPoint.none;
            for(int i = 0; i < state.ReadyUsers.Count; i++) {
                if (state.ReadyUsers[i] == userGeneralList.Key) {
                    spawnPoint = (SpawnPoint)i;
                    break;
                }
            }
            foreach (var general in userGeneralList.Value) {
                UnitStats newUnit = InstantiateUnit(general.Position, (int)general.UnitType, general.Owner, general);
                newUnit.MyUnit.rend.material.SetColor("_EmissionColor", ColourConstants.SpawnColours[spawnPoint]);
                unitPositions.Add(general.Position, newUnit);
                if(newUnit.Owner == username) {
                    if (unitNumbers.Contains((int)newUnit.UnitType)) {
                        unitNumbers.Remove((int)newUnit.UnitType);
                        for(int i = 0; i < unitTexts.Count; i++) {
                            if (unitTexts[i].GetComponentInChildren<TMP_Text>().text == UnitMetadata.ReadableNames[(UnitType)newUnit.UnitType]) {
                                UnitButtons[newUnit] = unitTexts[i];
                                unitTexts.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
        }
        foreach (var userUnitList in state.UserUnitsMap) {
            SpawnPoint spawnPoint = SpawnPoint.none;
            for (int i = 0; i < state.ReadyUsers.Count; i++) {
                if (state.ReadyUsers[i] == userUnitList.Key) {
                    spawnPoint = (SpawnPoint)i;
                    break;
                }
            }
            foreach (var unit in userUnitList.Value) {
                UnitStats newUnit = InstantiateUnit(unit.Position, (int)unit.UnitType, unit.Owner, unit);
                newUnit.MyUnit.rend.material.SetColor("_EmissionColor", ColourConstants.SpawnColours[spawnPoint]);
                unitPositions.Add(unit.Position, newUnit);
                if (newUnit.Owner == username) {
                    if (unitNumbers.Contains((int)newUnit.UnitType)) {
                        unitNumbers.Remove((int)newUnit.UnitType);
                        for (int i = 0; i < unitTexts.Count; i++) {
                            if (unitTexts[i].GetComponentInChildren<TMP_Text>().text == UnitMetadata.ReadableNames[(UnitType)newUnit.UnitType]) {
                                UnitButtons[newUnit] = unitTexts[i];
                                unitTexts.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public UnitStats InstantiateUnit(Vector2Int pos, int unitType, string username, UnitStats serverUnit) {
        UnitStats unit = UnitFactory.GetBaseUnit((UnitType)unitType);
        unit.CurrentHP = serverUnit.CurrentHP;
        GameObject unitObject = Instantiate(typePrefabStorage[unit.UnitType]);
        Unit unitComponent = unitObject.GetComponent<Unit>();
        unitComponent.PlaceAt(pos, ref board);
        unit.Direction = serverUnit.Direction;
        unitComponent.SnapToDirection(unit.Direction);
        unit.SetUnit(unitComponent);
        unit.Place(pos, ref board);
        unit.Owner = username;
        if(this.username == username && !isPlacing) {
            FogViewer unitFogViewer = unitComponent.GetFogViewer();
            unitFogViewer.SetRadius(unit.Vision + 1);
            fogController.AddFogViewer(unitFogViewer);
            unit.unitHUD = UnitHUDController.CreateUnitHUD(unit.CurrentHP.ToString(), unit.Armour.ToString(), unit.Damage.ToString(), unit.Pierce.ToString(), unit.MyUnit);
        }
        if (unitType > UnitMetadata.GENERAL_THRESHOLD) {
            unit.SetAbilities(GeneralMetadata.GeneralAbilityDictionary[unit.UnitType], serverUnit, username);
            unit.SetPassive(GeneralMetadata.GeneralPassiveDictionary[unit.UnitType]);
        }
        return unit;
    }

    public UnitStats InstantiateUnit(Vector2Int pos, int unitType, string username) {
        UnitStats unit = UnitFactory.GetBaseUnit((UnitType)unitType);
        GameObject unitObject = Instantiate(typePrefabStorage[unit.UnitType]);
        Unit unitComponent = unitObject.GetComponent<Unit>();
        unitComponent.PlaceAt(pos, ref board);
        unitComponent.SnapToDirection(unit.Direction);
        unit.SetUnit(unitObject.GetComponent<Unit>());
        unit.Place(pos, ref board);
        unit.Owner = username;
        if(username == this.username && !isPlacing) {
            FogViewer unitFogViewer = unitObject.GetComponent<Unit>().GetFogViewer();
            unitFogViewer.SetRadius(unit.Vision + 1);
            fogController.AddFogViewer(unitFogViewer);
            unit.unitHUD = UnitHUDController.CreateUnitHUD(unit.CurrentHP.ToString(), unit.Armour.ToString(), unit.Damage.ToString(), unit.Pierce.ToString(), unit.MyUnit);
        }
        if(unitType > UnitMetadata.GENERAL_THRESHOLD) {
            unit.SetAbilities(GeneralMetadata.GeneralAbilityDictionary[unit.UnitType]);
            unit.SetPassive(GeneralMetadata.GeneralPassiveDictionary[unit.UnitType]);
        }
        return unit;
    }
}
