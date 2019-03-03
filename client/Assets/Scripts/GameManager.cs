using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This class has to extend from monobehaviour so it can be created before a scene is loaded.
public class GameManager : MonoBehaviour {

    private Client client;

    [SerializeField]
    private GameObject gameBuilderPrefab;
    private GameObject gameBuilderObject;
    private GameBuilder gameBuilder;

    [SerializeField]
    private GameObject playerControllerPrefab;
    private GameObject playerControllerObject;
    private PlayerController playerController;

    private BoardController boardController;

    private InGameMenu inGameMenu;

    //Stores the list of actions made by the user so that they can be serialized and sent to the server
    private List<Action> turnActions;

    //Two variables that should be set by the "Load game" method
    //They are only used for persistant information between scenes
    private GameState state;
    private PlayerMetadata user;
    private ArmyPreset selectedPreset;

    //Dictionary used to store the units.
    Dictionary<Vector2Int, UnitStats> unitPositions = new Dictionary<Vector2Int, UnitStats>();

    UnitStats testUnit_1 = UnitFactory.GetBaseUnit(UnitType.claymore);
    UnitStats testUnit_2 = UnitFactory.GetBaseUnit(UnitType.compensator);
    UnitStats testUnit_3 = UnitFactory.GetBaseUnit(UnitType.steamer);
    UnitStats temp = UnitFactory.GetBaseUnit(UnitType.trooper);

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);

        client = GameObject.Find("Networking").GetComponent<Client>();

        // for testing
        unitPositions.Add(new Vector2Int(0, 0),testUnit_1);
        unitPositions.Add(new Vector2Int(0, -2), testUnit_2);
        unitPositions.Add(new Vector2Int(1, 3), testUnit_3);
    }

    //The method called when the load game button is pressed
    //Since we are loading the correct scene, we have to setup the onsceneloaded function
    public void LoadGame(GameState state) {
        this.state = state;
        this.user = client.UserInformation;

        SceneManager.sceneLoaded += OnGameLoaded;

        SceneManager.LoadScene(BoardMetadata.BoardNames[state.boardId]);
    }

    //This method is called when we need to place units
    public void PlaceUnits(GameState state, ArmyPreset selectedPreset) {
        this.state = state;
        this.user = client.UserInformation;
        this.selectedPreset = selectedPreset;

        SceneManager.sceneLoaded += OnPlaceUnits;

        SceneManager.LoadScene(BoardMetadata.BoardNames[state.boardId]);
    }

    private void OnGameLoaded(Scene scene, LoadSceneMode mode) {
        inGameMenu = GameObject.Find("GameHUDCanvas").GetComponent<InGameMenu>();

        inGameMenu.SetupPanels(isPlacing: false);

        boardController = new BoardController();
        boardController.Initialize();

        gameBuilderObject = Instantiate(gameBuilderPrefab);
        gameBuilder = gameBuilderObject.GetComponent<GameBuilder>();
        gameBuilder.Build(ref state, user.Username, ref boardController, false);

        unitPositions = gameBuilder.unitPositions;
        turnActions = new List<Action>();

        playerControllerObject = Instantiate(playerControllerPrefab);
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.Initialize(this, null, gameBuilder, boardController, false);
  
        SceneManager.sceneLoaded -= OnGameLoaded;
        SceneManager.sceneLoaded += OnMenuLoaded;
    }

    private void OnPlaceUnits(Scene scene, LoadSceneMode mode) {
        inGameMenu = GameObject.Find("GameHUDCanvas").GetComponent<InGameMenu>();

        inGameMenu.SetupPanels(isPlacing: true);

        boardController = new BoardController();
        boardController.Initialize();

        gameBuilderObject = Instantiate(gameBuilderPrefab);
        gameBuilder = gameBuilderObject.GetComponent<GameBuilder>();
        gameBuilder.Build(ref state, user.Username, ref boardController, true, selectedPreset);

        unitPositions = gameBuilder.unitPositions;
        turnActions = new List<Action>();

        SpawnPoint spawnPoint = SpawnPoint.none;
        for (int i = 0; i < state.AcceptedUsers.Count; i++) {
            if( state.AcceptedUsers[i] == user.Username) {
                spawnPoint = (SpawnPoint)i;
                break;
            }
        }

        playerControllerObject = Instantiate(playerControllerPrefab);
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.Initialize(this, null, gameBuilder, boardController, true, selectedPreset, gameBuilder.UnitDisplayTexts);

        SceneManager.sceneLoaded -= OnPlaceUnits;
        SceneManager.sceneLoaded += OnMenuLoaded;
    }

    private void OnMenuLoaded(Scene scene, LoadSceneMode mode) {
        state = null; //Verify that the state is destroyed;
        unitPositions.Clear();
        turnActions.Clear();
        //Anything else that the game manager has to reset needs to be done here
    }

    public void EndTurn() {
        //This function will need to figure out how to send the updated gamestate to the server
        SceneManager.LoadScene("MainMenu");
    }

    public void EndUnitPlacement(List<UnitStats> placedUnits) {
        //This function will have to figure out how to send the unit data to the server, and confirm that we are going
        //to be playing in this game
        SceneManager.LoadScene("MainMenu");
    }

    public void CreateUnitAtPos(Vector2Int position, int unit) {
        unitPositions.Add(position, gameBuilder.InstantiateUnit(position, unit));
    }

    //===================== Functions used to handle units ===================
    //Null is used in the event there isn't a unit on the tile
    public bool GetUnitOnTile(Vector2Int tile, out UnitStats unit) {
        bool containsUnit = unitPositions.ContainsKey(tile);
        unit = containsUnit ? unitPositions[tile] : null;
        return containsUnit;
    }

    public bool TileContainsUnit(Vector2Int tile) {
        return unitPositions.ContainsKey(tile);
    }

    //If the following conditions are true:
    //   the dictionary contains a unit at the "targetUnit" key, and does not contain a unit at the endpoint key
    public void MoveUnit(Vector2Int targetUnit, Vector2Int endpoint) {
        turnActions.Add(new Action(user.Username, ActionType.Movement, targetUnit, endpoint));
        if (!unitPositions.ContainsKey(endpoint)) {
            if (GetUnitOnTile(targetUnit, out UnitStats unit)) {
                unitPositions.Remove(targetUnit);
                unitPositions[endpoint] = unit;
                unit.Move(endpoint, ref boardController);
            }
        }
    }

    public void AttackUnit(Vector2Int source, Vector2Int target) {
        turnActions.Add(new Action(user.Username, ActionType.Attack, source, target));
        if (GetUnitOnTile(source, out UnitStats sourceUnit)) {
            List<Tuple<Vector2Int, int>> damages = sourceUnit.Attack(target);
            foreach (var damage in damages) {
                if (GetUnitOnTile(damage.First, out UnitStats targetUnit)) {
                    int modifiedDamage = System.Convert.ToInt32(damage.Second * UnitMetadata.GetMultiplier(sourceUnit.UnitType, targetUnit.UnitType));
                    if (targetUnit.TakeDamage(modifiedDamage, sourceUnit.Pierce)) {
                        unitPositions.Remove(damage.First);
                        Destroy(targetUnit.MyUnit);
                    }
                }
            }
        }
    }
}
