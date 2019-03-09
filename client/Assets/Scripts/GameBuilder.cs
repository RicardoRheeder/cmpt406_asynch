using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class GameBuilder : MonoBehaviour {

    public Dictionary<Vector2Int, UnitStats> unitPositions = new Dictionary<Vector2Int, UnitStats>();

    //Reference to the menus
    public GameObject unitDisplayPrefab;
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
    private int colorPick = 0;
    private ArmyPreset armyPreset;

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
        this.armyPreset = armyPreset;
        SetupScene();

        if(!isPlacing) {
            InstantiateUnits();
        }
    }

    //Method responsible for instantiate the canvas, the camera rig, the light(s),
    private void SetupScene() {
        if (isPlacing) {
            unitPlacementViewport = GameObject.Find("PlaceUnitsViewport");
            UnitDisplayTexts = new List<GameObject>();

            GameObject generalText = Instantiate(unitDisplayPrefab);
            UnitDisplayTexts.Add(generalText);
            generalText.transform.SetParent(unitPlacementViewport.transform, false);
            generalText.GetComponent<TMP_Text>().text = UnitMetadata.ReadableNames[(UnitType)armyPreset.General];
            foreach (int unit in armyPreset.Units) {
                GameObject unitText = Instantiate(unitDisplayPrefab);
                UnitDisplayTexts.Add(unitText);
                unitText.transform.SetParent(unitPlacementViewport.transform, false);
                unitText.GetComponent<TMP_Text>().text = UnitMetadata.ReadableNames[(UnitType)unit];
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
                newUnit.MyUnit.rend.material.color = SpawnMetadata.SpawnColours[spawnPoint];
                unitPositions.Add(general.Position, newUnit);
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
                newUnit.MyUnit.rend.material.color = SpawnMetadata.SpawnColours[spawnPoint];
                unitPositions.Add(unit.Position, newUnit);
            }
        }
    }

    public UnitStats InstantiateUnit(Vector2Int pos, int unitType, string username, UnitStats serverUnit) {
        UnitStats unit = UnitFactory.GetBaseUnit((UnitType)unitType);
        unit.CurrentHP = serverUnit.CurrentHP;
        GameObject unitObject = Instantiate(typePrefabStorage[unit.UnitType]);
        unitObject.GetComponent<Unit>().PlaceAt(pos, ref board);
        unit.SetUnit(unitObject.GetComponent<Unit>());
        unit.Move(pos, ref board, true);
        unit.Owner = username;
        if(this.username == username) {
            FogViewer unitFogViewer = unitObject.GetComponent<Unit>().GetFogViewer();
            unitFogViewer.SetRadius(unit.Vision + 1);
            fogController.AddFogViewer(unitFogViewer);
        }
        if (unitType > UnitMetadata.GENERAL_THRESHOLD) {
            unit.SetAbilities(GeneralMetadata.GeneralAbilityDictionary[unit.UnitType], serverUnit);
            unit.SetPassive(GeneralMetadata.GeneralPassiveDictionary[unit.UnitType]);
        }
        return unit;
    }

    public UnitStats InstantiateUnit(Vector2Int pos, int unitType, string username) {
        UnitStats unit = UnitFactory.GetBaseUnit((UnitType)unitType);
        GameObject unitObject = Instantiate(typePrefabStorage[unit.UnitType]);
        unitObject.GetComponent<Unit>().PlaceAt(pos, ref board);
        unit.SetUnit(unitObject.GetComponent<Unit>());
        unit.Move(pos, ref board, true);
        unit.Owner = username;
        if(username == this.username) {
            FogViewer unitFogViewer = unitObject.GetComponent<Unit>().GetFogViewer();
            unitFogViewer.SetRadius(unit.Vision + 1);
            fogController.AddFogViewer(unitFogViewer);
        }
        if(unitType > UnitMetadata.GENERAL_THRESHOLD) {
            unit.SetAbilities(GeneralMetadata.GeneralAbilityDictionary[unit.UnitType]);
            unit.SetPassive(GeneralMetadata.GeneralPassiveDictionary[unit.UnitType]);
        }
        return unit;
    }
}
