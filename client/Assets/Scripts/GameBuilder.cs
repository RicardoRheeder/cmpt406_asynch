using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBuilder : MonoBehaviour {

    public Dictionary<Vector2Int, UnitStats> unitPositions = new Dictionary<Vector2Int, UnitStats>();

    //Prefabs for each unit for instantiation purposes
    public GameObject compensatorPrefab;
    public Dictionary<UnitType, GameObject> typePrefabStorage;

    private GameState state;
    private string username;

    private BoardController board;

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
    public void Build(ref GameState state, ref string username, ref BoardController board) {
        this.state = state;
        this.username = username;
        this.board = board;

        InstantiateUnits();
    }

    private void InstantiateUnits() {
        List<UnitStats> units = state.UserUnitsMap[username];
        for (int i = 0; i < units.Count; i++) {
            UnitStats unitStats = units[i];
            unitPositions.Add(unitStats.Position, unitStats);
            GameObject unit = Instantiate(typePrefabStorage[unitStats.UnitType]);
            unit.transform.position = board.CellToWorld((Vector3Int)unitStats.Position);
            unitStats.SetUnit(unit.GetComponent<Unit>());
        }
    }
}
