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
    private bool isPlacing;
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
    public void Build(ref GameState state, string username, ref BoardController board, bool isPlacing, ArmyPreset armyPreset = null) {
        this.state = state;
        this.username = username;
        this.board = board;
        this.isPlacing = isPlacing;
        this.armyPreset = armyPreset;

        SetupScene();

        if(!isPlacing) {
            InstantiateUnits();
        }
    }

    //Method responsible for instantiate the canvas, the camera rig, the light(s),
    //  and making sure the camera rig script has the appropriate parameters
    private void SetupScene() {
        GameObject.Find("CameraRig").GetComponent<CameraMovement>().UpdateLimits();
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
        //this check shouldn't be required
        List<UnitStats> units = state.UserUnitsMap.ContainsKey(username) ? state.UserUnitsMap[username] : new List<UnitStats>();
        for (int i = 0; i < units.Count; i++) {
            UnitStats unitStats = units[i];
            unitPositions.Add(unitStats.Position, InstantiateUnit(unitStats.Position, (int)unitStats.UnitType));
        }
    }

    public UnitStats InstantiateUnit(Vector2Int position, int type) {
        UnitStats unit = UnitFactory.GetBaseUnit((UnitType)type);
        GameObject unitObject = Instantiate(typePrefabStorage[unit.UnitType]);
        unit.SetUnit(unitObject.GetComponent<Unit>());
        unit.Move(position, ref board);
        return unit;
    }
}
