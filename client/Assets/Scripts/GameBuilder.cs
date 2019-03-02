using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class GameBuilder : MonoBehaviour {

    public Dictionary<Vector2Int, UnitStats> unitPositions = new Dictionary<Vector2Int, UnitStats>();

    //Prefabs used to set up the scene
    public GameObject inGameUiPrefab;
    private GameObject inGameUi;
    public GameObject placementUiPrefab;
    private GameObject placementUi;
    public GameObject inGameCameraPrefab;
    private GameObject inGameCamera;
    public GameObject inGameLightPrefab;
    private GameObject inGameLight;

    //Reference to the menus
    public GameObject unitDisplayPrefab;
    private GameObject unitPlacementViewport;
    public List<GameObject> UnitDisplayTexts { get; private set; }

    //Prefabs for each unit for instantiation purposes
    public GameObject compensatorPrefab;
    public Dictionary<UnitType, GameObject> typePrefabStorage;

    private GameState state;
    private string username;
    private BoardController board;
    private bool isPlacing;
    private ArmyPreset armyPreset;

    public void Awake() {
        //Populate the dictionary whenever we build the map
        typePrefabStorage = new Dictionary<UnitType, GameObject>() {
            {UnitType.claymore, compensatorPrefab },
            {UnitType.compensator, compensatorPrefab },
            {UnitType.trooper, compensatorPrefab },
            {UnitType.reacon, compensatorPrefab },
            {UnitType.steamer, compensatorPrefab },
            {UnitType.pewpew, compensatorPrefab },
            {UnitType.foundation, compensatorPrefab },
            {UnitType.powerSurge, compensatorPrefab },
            {UnitType.midas, compensatorPrefab },
            {UnitType.general1, compensatorPrefab }
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
        inGameCamera = Instantiate(inGameCameraPrefab);
        inGameCamera.GetComponent<CameraMovement>().UpdateLimits();
        inGameLight = Instantiate(inGameLightPrefab);
        if (isPlacing) {
            placementUi = Instantiate(placementUiPrefab);
            unitPlacementViewport = GameObject.Find("UnitPlacementViewport");
            UnitDisplayTexts = new List<GameObject>();

            GameObject generalText = Instantiate(unitDisplayPrefab);
            UnitDisplayTexts.Add(generalText);
            generalText.transform.SetParent(unitPlacementViewport.transform, false);
            generalText.GetComponent<TMP_Text>().text = ((UnitType)armyPreset.General).ToString();
            foreach (int unit in armyPreset.Units) {
                GameObject unitText = Instantiate(unitDisplayPrefab);
                UnitDisplayTexts.Add(unitText);
                unitText.transform.SetParent(unitPlacementViewport.transform, false);
                unitText.GetComponent<TMP_Text>().SetText(((UnitType)unit).ToString());
            }
        }
        else {
            inGameUi = Instantiate(inGameUiPrefab);
        }
    }

    //Method responsible for making sure all of the units are created with the appropriate gameobjects
    private void InstantiateUnits() {
        //this check shouldn't be required
        List<UnitStats> units = state.UserUnitsMap.ContainsKey(username) ? state.UserUnitsMap[username] : new List<UnitStats>();
        for (int i = 0; i < units.Count; i++) {
            UnitStats unitStats = units[i];
            unitPositions.Add(unitStats.Position, unitStats);
            GameObject unit = Instantiate(typePrefabStorage[unitStats.UnitType]);
            unit.transform.position = board.CellToWorld((Vector3Int)unitStats.Position);
            unitStats.SetUnit(unit.GetComponent<Unit>());
        }
    }

    public void InstantiateUnit(int unit) {

    }
}
