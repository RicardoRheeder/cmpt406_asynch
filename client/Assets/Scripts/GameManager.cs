using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This class has to extend from monobehaviour so it can be created before a scene is loaded.
public class GameManager : MonoBehaviour {
    [SerializeField]
    private GameObject gameBuilderPrefab;
    private GameObject gameBuilderObject;
    private GameBuilder gameBuilder;

    [SerializeField]
    private GameObject playerControllerPrefab;
    private GameObject playerControllerObject;
    private PlayerController playerController;

    private BoardController boardController;

    //Stores the list of actions made by the user so that they can be serialized and sent to the server
    private List<Action> turnActions;

    //Two variables that should be set by the "Load game" method
    //They are only used for persistant information between scenes
    private GameState state;
    private string username;

    //Dictionary used to store the units.
    Dictionary<Vector2Int, UnitStats> unitPositions = new Dictionary<Vector2Int, UnitStats>();

    UnitStats testUnit_1 = UnitFactory.GetBaseUnit(UnitType.claymore);
    UnitStats testUnit_2 = UnitFactory.GetBaseUnit(UnitType.compensator);
    UnitStats testUnit_3 = UnitFactory.GetBaseUnit(UnitType.steamer);
    UnitStats temp = UnitFactory.GetBaseUnit(UnitType.trooper);

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);

        // for testing
        unitPositions.Add(new Vector2Int(0, 0),testUnit_1);
        unitPositions.Add(new Vector2Int(0, -2), testUnit_2);
        unitPositions.Add(new Vector2Int(1, 3), testUnit_3);
    }

    //The method called when the load game button is pressed
    //Since we are loading the correct scene, we have to setup the onsceneloaded function
    public void LoadGame(GameState state, string username) {
        this.state = state;
        this.username = username;

        SceneManager.sceneLoaded += OnGameLoaded;

        SceneManager.LoadScene(SceneMetadata.BoardNames[state.boardId]);
    }

    //This method has to be called immediately after we've loaded a scene
    private void OnGameLoaded(Scene scene, LoadSceneMode mode) {
        boardController = new BoardController();
        boardController.Initialize();

        gameBuilderObject = Instantiate(gameBuilderPrefab);
        gameBuilder = gameBuilderObject.GetComponent<GameBuilder>();
        gameBuilder.Build(ref state, ref username, ref boardController);

        unitPositions = gameBuilder.unitPositions;
        turnActions = new List<Action>();

        playerControllerObject = Instantiate(playerControllerPrefab);
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.Initialize(this, null, boardController);
  
        //Since the only scene we can load from this point is the main menu, we can prep 
        SceneManager.sceneLoaded -= OnGameLoaded;
        SceneManager.sceneLoaded += OnMenuLoaded;
    }

    private void OnMenuLoaded(Scene scene, LoadSceneMode mode) {
        state = null; //Verify that the state is destroyed;
        unitPositions.Clear();
        turnActions.Clear();
        //Anything else that the game manager has to reset needs to be done here
    }

    //end the users turn
    public void EndTurn() {
        //This function will need to figure out how to send the updated gamestate to the server
    }

    //===================== Functions used to handle units ===================
    //In this case, we are using null as a way to say 
    public bool GetUnitOnTile(Vector2Int tile, out UnitStats unit) {
        bool containsUnit = unitPositions.ContainsKey(tile);
        unit = containsUnit ? unitPositions[tile] : null;
        return containsUnit;
    }

    //If the following conditions are true:
    //   the dictionary contains a unit at the "targetUnit" key, and does not contain a unit at the endpoint key
    public void MoveUnit(Vector2Int targetUnit, Vector2Int endpoint) {
        turnActions.Add(new Action(username, ActionType.Movement, targetUnit, endpoint));
        if (!unitPositions.ContainsKey(endpoint)) {
            if (GetUnitOnTile(targetUnit, out UnitStats unit)) {
                unitPositions.Remove(targetUnit);
                unitPositions[endpoint] = unit;
                unit.Move(endpoint);
            }
        }
    }

    public void AttackUnit(Vector2Int source, Vector2Int target) {
        turnActions.Add(new Action(username, ActionType.Attack, source, target));
        if (GetUnitOnTile(source, out UnitStats sourceUnit)) {
            List<Tuple<Vector2Int, int>> damages = sourceUnit.Attack(target);
            foreach (var damage in damages) {
                if (GetUnitOnTile(damage.First, out UnitStats targetUnit)) {
                    int modifiedDamage = System.Convert.ToInt32(damage.Second * UnitMetadata.GetMultiplier(sourceUnit.UnitType, targetUnit.UnitType));
                    if (targetUnit.TakeDamage(modifiedDamage, sourceUnit.Pierce)) {
                        unitPositions.Remove(damage.First);
                        //Destroy the related gameobject
                        Destroy(targetUnit.MyUnit);
                    }
                }
            }
        }
    }
}
