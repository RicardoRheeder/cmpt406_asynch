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

    private List<Action> turnActions;

    //Two variables that should be set by the "Load game" method
    //They are only used for persistant information between scenes
    private GameState state;
    private string username;

    //For testing the units
    Dictionary<Vector3Int, UnitStats> unitPositions = new Dictionary<Vector3Int, UnitStats>();

    UnitStats testUnit_1 = UnitFactory.GetBaseUnit(UnitType.claymore);
    UnitStats testUnit_2 = UnitFactory.GetBaseUnit(UnitType.compensator);
    UnitStats testUnit_3 = UnitFactory.GetBaseUnit(UnitType.steamer);
    UnitStats temp = UnitFactory.GetBaseUnit(UnitType.trooper);

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);

        // for testing
        unitPositions.Add(new Vector3Int(0,0,0),testUnit_1);
        unitPositions.Add(new Vector3Int(0, -2, 0), testUnit_2);
        unitPositions.Add(new Vector3Int(1, 3, 0), testUnit_3);
    }

    //The method called when the load game button is pressed
    //Since we are loading the correct scene, we have to setup the onsceneloaded function
    void LoadGame(GameState state, string username) {
        this.state = state;
        this.username = username;

        SceneManager.sceneLoaded += OnGameLoaded;

        SceneManager.LoadScene(SceneMetadata.BoardNames[state.boardId]);
    }

    //This method has to be called immediately after we've loaded a scene
    void OnGameLoaded(Scene scene, LoadSceneMode mode) {
        CardController deck = new CardController(
            new List<Card>(state.hand[username]),
            new List<Card>(state.drawPile[username]),
            new List<Card>(state.discardPile[username])
        );
        gameBuilderObject = Instantiate(gameBuilderPrefab);
        gameBuilder = gameBuilderObject.GetComponent<GameBuilder>();
        gameBuilder.Build(state);

        turnActions = new List<Action>();

        playerControllerObject = Instantiate(playerControllerPrefab);
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.Initialize(this, deck);

        //Since the only scene we can load from this point is the main menu, we can prep 
        SceneManager.sceneLoaded -= OnGameLoaded;
        SceneManager.sceneLoaded += OnMenuLoaded;
    }

    void OnMenuLoaded(Scene scene, LoadSceneMode mode) {
        state = null; //Verify that the state is destroyed;
        //Anything else that the game manager has to reset needs to be done here
    }

    //end the users turn
    public void EndTurn() {
        //This function will need to figure out how to send the updated gamestate to the server
    }

    public void AddAction(Action action) {
        turnActions.Add(action);
    }

    public UnitStats GetUnitOnTile(Vector3Int tile) {
        return unitPositions.ContainsKey(tile) ? unitPositions[tile] : null;
    }
}
