using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //Prefabs for each unit for instantiation purposes
    public GameObject compensatorPrefab;
    public Dictionary<UnitType, GameObject> typePrefabStorage;

    private GameState state;
    private string username;
    private BoardController board;
    private bool isPlacing;

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
    public void Build(ref GameState state, ref string username, ref BoardController board, bool isPlacing) {
        this.state = state;
        this.username = username;
        this.board = board;
        this.isPlacing = isPlacing;

        SetupScene();

        if(!isPlacing) {
            InstantiateUnits();
        }
    }

    //Method responsible for instantiate the canvas, the camera rig, the light(s),
    //  and making sure the camera rig script has the appropriate parameters
    private void SetupScene() {
        if (isPlacing) {
            placementUi = Instantiate(placementUiPrefab);
        }
        else {
            inGameUi = Instantiate(inGameUiPrefab);
        }
        inGameCamera = Instantiate(inGameCameraPrefab);
        inGameCamera.GetComponent<CameraMovement>().UpdateLimits();
        inGameLight = Instantiate(inGameLightPrefab);
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
}
