using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//This class has to extend from monobehaviour so it can be created before a scene is loaded.
public class GameManager : MonoBehaviour {

    private INetwork client;

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
    private Dictionary<Vector2Int, UnitStats> unitPositions = new Dictionary<Vector2Int, UnitStats>();

    //Logic to handle the case where we are placing units;
    private List<UnitStats> placedUnits;
    private bool isPlacing = false;

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);

        client = GameObject.Find("Networking").GetComponent<Client>();
    }

    //The method called when the load game button is pressed
    //Since we are loading the correct scene, we have to setup the onsceneloaded function
    public void LoadGame(GameState state) {
        this.state = state;
        this.user = client.GetUserInformation();
        this.isPlacing = false;

        SceneManager.sceneLoaded -= OnMenuLoaded;
        SceneManager.sceneLoaded += OnGameLoaded;

        SceneManager.LoadScene(BoardMetadata.BoardNames[state.boardId]);
    }

    //This method is called when we need to place units
    public void PlaceUnits(GameState state, ArmyPreset selectedPreset) {
        this.state = client.GetGamestate(state.id).Second; //get the updated gamestate
        this.user = client.GetUserInformation();
        this.selectedPreset = selectedPreset;
        this.isPlacing = true;
        this.placedUnits = new List<UnitStats>();

        SceneManager.sceneLoaded -= OnMenuLoaded;
        SceneManager.sceneLoaded += OnPlaceUnits;

        SceneManager.LoadScene(BoardMetadata.BoardNames[state.boardId]);
    }
	
	public void StartSandbox(GameState state){
		this.state = state;
		this.isPlacing = false;
		this.client = new Sandbox();
		this.user = client.GetUserInformation();
		
		SceneManager.sceneLoaded -= OnMenuLoaded;
        SceneManager.sceneLoaded += OnGameLoaded;
		SceneManager.sceneLoaded += OnSandboxLoaded;
	
		SceneManager.LoadScene(BoardMetadata.BoardNames[state.boardId]);
	}

    private void OnGameLoaded(Scene scene, LoadSceneMode mode) {
        inGameMenu = GameObject.Find("GameHUDCanvas").GetComponent<InGameMenu>();
        GameObject.Find("EndTurnButton").GetComponent<Button>().onClick.AddListener(this.EndTurn);

        boardController = new BoardController();
        boardController.Initialize();

        gameBuilderObject = Instantiate(gameBuilderPrefab);
        gameBuilder = gameBuilderObject.GetComponent<GameBuilder>();
        gameBuilder.Build(ref state, user.Username, ref boardController, false);

        unitPositions = gameBuilder.unitPositions;
        turnActions = new List<Action>();

        playerControllerObject = Instantiate(playerControllerPrefab);
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.Initialize(this, state, null, gameBuilder, boardController, isPlacing);

        inGameMenu.SetupPanels(isPlacing: false);

        SceneManager.sceneLoaded -= OnGameLoaded;
        SceneManager.sceneLoaded += OnMenuLoaded;
    }
	
	private void OnSandboxLoaded(Scene scene, LoadSceneMode mode) {
		SceneManager.sceneLoaded += OnMenuSandbox;
	}

    private void OnPlaceUnits(Scene scene, LoadSceneMode mode) {
        inGameMenu = GameObject.Find("GameHUDCanvas").GetComponent<InGameMenu>();

        inGameMenu.SetupPanels(isPlacing: true);

        boardController = new BoardController();
        boardController.Initialize();

        gameBuilderObject = Instantiate(gameBuilderPrefab);
        gameBuilder = gameBuilderObject.GetComponent<GameBuilder>();
        gameBuilder.Build(ref state, user.Username, ref boardController, isPlacing, selectedPreset);

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
        playerController.Initialize(this, state, null, gameBuilder, boardController, true, selectedPreset, gameBuilder.UnitDisplayTexts, spawnPoint);

        SceneManager.sceneLoaded -= OnPlaceUnits;
        SceneManager.sceneLoaded += OnMenuLoaded;
    }

    private void OnMenuLoaded(Scene scene, LoadSceneMode mode) {
        state = null; //Verify that the state is destroyed;
        unitPositions.Clear();
        turnActions.Clear();
        //Anything else that the game manager has to reset needs to be done here
    }
	
	private void OnMenuSandbox(Scene scene, LoadSceneMode mode){
		client = GameObject.Find("Networking").GetComponent<Client>();
		this.user = client.GetUserInformation();
	}

    public void EndTurn() {
        //This function will need to figure out how to send the updated gamestate to the server
        client.EndTurn(new EndTurnState(state, user.Username, turnActions, new List<UnitStats>(unitPositions.Values)));
        SceneManager.LoadScene("MainMenu");
    }

    public void Forfeit() {
        client.ForfeitGame(state.id);
        SceneManager.LoadScene("MainMenu");
    }

    //For now just load the main menu and don't do anything else
    public void ExitGame() {
        SceneManager.LoadScene("MainMenu");
    }

    public void EndUnitPlacement() {
        //This function will have to figure out how to send the unit data to the server, and confirm that we are going
        //to be playing in this game
        UnitStats general = placedUnits[0];
        CardController cards = CardFactory.GetCardControllerFromUnits(placedUnits, user.Username);
        placedUnits.RemoveAt(0);
        ReadyUnitsGameState readyState = new ReadyUnitsGameState(state.id, placedUnits, general, cards);
        client.ReadyUnits(readyState);
        SceneManager.LoadScene("MainMenu");
    }

    //Used for unit placement
    public void CreateUnitAtPos(Vector2Int position, int unit) {
        UnitStats createdUnit = gameBuilder.InstantiateUnit(position, unit, user.Username);
        placedUnits.Add(createdUnit);
        unitPositions.Add(position, createdUnit);
    }

    //===================== Functions used to handle units ===================
    //Null is used in the event there isn't a unit on the tile
    public bool GetUnitOnTile(Vector2Int tile, out UnitStats unit) {
        bool containsUnit = unitPositions.ContainsKey(tile);
        unit = containsUnit ? unitPositions[tile] : null;
        return containsUnit;
    }

    public bool GetUnitOnTileUserOwns(Vector2Int tile, out UnitStats unit) {
        bool myUnit = false;
        unit = null;
        if (unitPositions.ContainsKey(tile)) {
            unit = unitPositions[tile];
            myUnit = unit.Owner == user.Username;
        }
        return myUnit;
    }

    public bool TileContainsUnit(Vector2Int tile) {
        return unitPositions.ContainsKey(tile);
    }

    //If the following conditions are true:
    //   the dictionary contains a unit at the "targetUnit" key, and does not contain a unit at the endpoint key
    public void MoveUnit(Vector2Int targetUnit, Vector2Int endpoint) {
        if (!unitPositions.ContainsKey(endpoint)) {
            if (GetUnitOnTile(targetUnit, out UnitStats unit)) {
                if(unit.MovementActions > 0 && unit.Owner == user.Username) {
                    unitPositions.Remove(targetUnit);
                    unitPositions[endpoint] = unit;
                    unit.Move(endpoint, ref boardController);
                    turnActions.Add(new Action(user.Username, ActionType.Movement, targetUnit, endpoint));
                }
            }
        }
    }

    public void AttackUnit(Vector2Int source, Vector2Int target) {
        turnActions.Add(new Action(user.Username, ActionType.Attack, source, target));
        if (GetUnitOnTile(source, out UnitStats sourceUnit)) {
            if(sourceUnit.AttackActions > 0 && sourceUnit.Owner == user.Username) {
                List<Tuple<Vector2Int, int>> damages = sourceUnit.Attack(target);
                foreach (var damage in damages) {
                    if (GetUnitOnTile(damage.First, out UnitStats targetUnit)) {
                        int modifiedDamage = System.Convert.ToInt32(damage.Second * UnitMetadata.GetMultiplier(sourceUnit.UnitType, targetUnit.UnitType));
                        if (targetUnit.TakeDamage(modifiedDamage, sourceUnit.Pierce)) {
                            unitPositions.Remove(damage.First);
                            targetUnit.Kill();
                        }
                    }
                }
            }
        }
    }
}
