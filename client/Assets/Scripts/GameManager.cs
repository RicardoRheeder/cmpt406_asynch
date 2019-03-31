using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

using CardsAndCarnage;

#pragma warning disable 649
//This class has to extend from monobehaviour so it can be created before a scene is loaded.
public class GameManager : MonoBehaviour {

    //References to in scene objects
    private INetwork client;

    [SerializeField]
    private GameObject gameBuilderPrefab;
    private GameObject gameBuilderObject;
    private GameBuilder gameBuilder;

    [SerializeField]
    private GameObject playerControllerPrefab;
    private GameObject playerControllerObject;
    private PlayerController playerController;

    private CardSystemManager cardSystem;
    private BoardController boardController;
    private FogOfWarController fogOfWarController;

    private CameraMovement cameraRig;
    private InGameMenu inGameMenu;
    private AudioManager audioManager;

    //Stores the list of actions made by the user so that they can be serialized and sent to the server
    private List<Action> turnActions;

    //Two variables that should be set by the "Load game" method
    //They are only used for persistant information between scenes
    private GameState state;
    private PlayerMetadata user;
    private ArmyPreset selectedPreset;

    //Dictionary used to the game information
    private Dictionary<Vector2Int, UnitStats> unitPositions = new Dictionary<Vector2Int, UnitStats>();
    private Dictionary<Vector2Int, Effect> effectPositions = new Dictionary<Vector2Int, Effect>();
    private List<Action> actionsSinceLastTurn;

    //Logic to handle the case where we are placing units;
    private List<UnitStats> placedUnits;
    private bool isPlacing = false;

    //flag for the sandbox;
    private bool isSandboxMode = false;

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        client = GameObject.Find("Networking").GetComponent<Client>();
    }

    //===================== Setup functions ===================
    //The method called when the load game button is pressed
    //Since we are loading the correct scene, we have to setup the onsceneloaded function
    public void LoadGame(GameState state) {
        Tuple<bool, GameState> result = client.GetGamestate(state.id);
        if (result.First) {
            this.state = client.GetGamestate(state.id).Second; //get the updated gamestate
            this.user = client.GetUserInformation();
            this.isPlacing = false;

            SceneManager.sceneLoaded -= OnMenuLoaded;
            SceneManager.sceneLoaded += OnGameLoaded;

            SceneManager.LoadScene(BoardMetadata.BoardSceneNames[state.boardId]);
        }
    }

    //This method is called when we need to place units
    public void PlaceUnits(GameState state, ArmyPreset selectedPreset) {
        Tuple<bool, GameState> result = client.GetGamestate(state.id);
        if(result.First) {
            this.state = result.Second; //get the updated gamestate
            this.user = client.GetUserInformation();
            this.selectedPreset = selectedPreset;
            this.isPlacing = true;
            this.placedUnits = new List<UnitStats>();

            SceneManager.sceneLoaded -= OnMenuLoaded;
            SceneManager.sceneLoaded += OnPlaceUnits;

            SceneManager.LoadScene(BoardMetadata.BoardSceneNames[state.boardId]);
        }
        else {
            //something bad happened
        }
    }
    
    public void StartSandbox(GameState state){
        this.state = state;
        this.isPlacing = false;
        this.client = new Sandbox();
        this.user = client.GetUserInformation();

        audioManager.Play(SoundName.ButtonPress);
        
        SceneManager.sceneLoaded -= OnMenuLoaded;
        SceneManager.sceneLoaded += OnGameLoaded;
        SceneManager.sceneLoaded += OnSandboxLoaded;
    
        SceneManager.LoadScene(BoardMetadata.BoardSceneNames[state.boardId]);
    }

    private void OnGameLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log("Loading state: " + state);
		FloatingTextController.Initialize();
		UnitHUDController.Initialize();

        inGameMenu = GameObject.Find("GameHUDCanvas").GetComponent<InGameMenu>();
        GameObject.Find("EndTurnButton").GetComponent<Button>().onClick.AddListener(this.EndTurn);
        
        boardController = new BoardController();
        boardController.Initialize();

        fogOfWarController = new FogOfWarController();
        fogOfWarController.InitializeFogOfWar(boardController.GetTilemap());

        gameBuilderObject = Instantiate(gameBuilderPrefab);
        gameBuilder = gameBuilderObject.GetComponent<GameBuilder>();
        gameBuilder.Build(ref state, user.Username, ref boardController, ref fogOfWarController, false);

        unitPositions = gameBuilder.unitPositions;
        turnActions = new List<Action>();

        playerControllerObject = Instantiate(playerControllerPrefab);
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.Initialize(this, audioManager, user.Username, state, null, gameBuilder, boardController, fogOfWarController, isPlacing, presetTexts:gameBuilder.UnitDisplayTexts, unitButtonReferences: gameBuilder.UnitButtons);

        cameraRig = GameObject.Find("CameraRig").GetComponent<CameraMovement>();
        cameraRig.SnapToPosition(boardController.CellToWorld(GetGeneralPosition(user.Username)));

        cardSystem = GameObject.Find("CardSystem").GetComponent<CardSystemManager>();
        List<CardFunction> hand = new List<CardFunction>();
        if (state.UserCardsMap.ContainsKey(user.Username)) {
            hand = state.UserCardsMap[user.Username].Hand;
        }

        DropZone dropZone = GameObject.Find("Tabletop").GetComponent<DropZone>();
        dropZone.SetPlayerController(playerController);
        dropZone.SetCardSystemManager(cardSystem);

        bool wasActions = false;
        for(int i = 0; i < state.Actions.Count; i++) {
            if (state.Actions[i].Username == user.Username) {
                wasActions = true;
                break;
            }
        }

        if(wasActions) {
            cardSystem.Initialize(hand, state.UserUnitsMap[user.Username], state.id);
        }
        else {
            cardSystem.Initialize(hand, state.UserUnitsMap[user.Username], state.id, drawLimit:CardMetadata.GENERIC_CARD_LIMIT + CardMetadata.UNIQUE_CARD_LIMIT);
        }

        inGameMenu.SetupPanels(isPlacing: false);

        actionsSinceLastTurn = new List<Action>();
        PreprocessGenerals();
        PreprocessCards();
        PreprocessTraps();

        fogOfWarController.UpdateAllFog();

        SceneManager.sceneLoaded -= OnGameLoaded;
        SceneManager.sceneLoaded += OnMenuLoaded;
    }
    
    private void OnSandboxLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.sceneLoaded -= OnSandboxLoaded;
        SceneManager.sceneLoaded += OnMenuSandbox;

        isSandboxMode = true;
    }

    private void OnPlaceUnits(Scene scene, LoadSceneMode mode) {
        inGameMenu = GameObject.Find("GameHUDCanvas").GetComponent<InGameMenu>();

        boardController = new BoardController();
        boardController.Initialize();

        fogOfWarController = new FogOfWarController();
        fogOfWarController.InitializeFogOfWar(boardController.GetTilemap());

        gameBuilderObject = Instantiate(gameBuilderPrefab);
        gameBuilder = gameBuilderObject.GetComponent<GameBuilder>();
        gameBuilder.Build(ref state, user.Username, ref boardController, ref fogOfWarController, isPlacing, selectedPreset);

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
        playerController.Initialize(this, audioManager, user.Username, state, null, gameBuilder, boardController, fogOfWarController, true, selectedPreset, gameBuilder.UnitDisplayTexts, spawnPoint);

        cameraRig = GameObject.Find("CameraRig").GetComponent<CameraMovement>();
        cameraRig.SnapToPosition(boardController.CellToWorld(boardController.GetCenterSpawnTile(spawnPoint)));

        inGameMenu.SetupPanels(isPlacing: true);

        fogOfWarController.DeleteFogAtSpawnPoint(spawnPoint, ref boardController);

        SceneManager.sceneLoaded -= OnPlaceUnits;
        SceneManager.sceneLoaded += OnMenuLoaded;
    }

    private void OnMenuLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.sceneLoaded -= OnMenuLoaded;
        state = null; //Verify that the state is destroyed;
        gameBuilder = null;
        playerController = null;
        boardController = null;
        fogOfWarController = null;
        Destroy(gameBuilderObject);
        Destroy(playerControllerObject);
        unitPositions.Clear();
        turnActions.Clear();
        effectPositions.Clear();
    }
    
    private void OnMenuSandbox(Scene scene, LoadSceneMode mode) {
        SceneManager.sceneLoaded -= OnMenuSandbox;
        client = GameObject.Find("Networking").GetComponent<Client>();
        this.user = client.GetUserInformation();
        isSandboxMode = false;
    }

    //===================== Preprocessing functions ===================
    private void PreprocessGenerals() {
        foreach(var position in unitPositions.Keys) {
            UnitStats general = unitPositions[position];
            if((int)general.UnitType > UnitMetadata.GENERAL_THRESHOLD) {
                GeneralMetadata.PassiveEffectsDictionary[general.Passive](unitPositions, general.Owner);
                if (general.Ability1Duration > 0) {
                    if (general.Owner == user.Username) {
                        general.Ability1Duration--;
                    }
                    GeneralMetadata.ActiveAbilityFunctionDictionary[general.Ability1](
                        ref general,
                        unitPositions,
                        general.Owner,
                        user.Username == general.Owner
                    );
                }
                if (general.Ability1Cooldown > 0 && general.Owner == user.Username) {
                    general.Ability1Cooldown--;
                }
                if (general.Ability2Duration > 0) {
                    if (general.Owner == user.Username) {
                        general.Ability2Duration--;
                    }
                    GeneralMetadata.ActiveAbilityFunctionDictionary[general.Ability2](
                        ref general,
                        unitPositions,
                        general.Owner,
                        user.Username == general.Owner
                    );
                }
                if (general.Ability2Cooldown > 0 && general.Owner == user.Username) {
                    general.Ability2Cooldown--;
                }
            }
        }
    }

    //Deal with persistant cards
    private void PreprocessCards() {
        List<Action> reverseActions = new List<Action>(state.Actions);
        reverseActions.Reverse();
        List<Action> cardsSinceLastTurn = new List<Action>();
        for(int i = 0; i < reverseActions.Count; i++) {
            Action action = reverseActions[i];
            if(action.Username == user.Username) {
                break;
            }
            else {
                actionsSinceLastTurn.Add(action);
                if (action.Type == ActionType.Card) {
                    cardsSinceLastTurn.Add(action);
                }
            }
        }

        actionsSinceLastTurn.Reverse();
        cardsSinceLastTurn.Reverse();
        for(int i = 0; i < cardsSinceLastTurn.Count; i++) {
            Action action = cardsSinceLastTurn[i];
            CardMetadata.CardEffectDictionary[action.CardId](new Vector2Int(action.TargetXPos, action.TargetYPos), unitPositions, action.Username, true);
        }
    }

    private void PreprocessTraps() {

    }

    //===================== In game button functionality ===================
    private bool exiting = false;
    public void EndTurn() {
        if (exiting) {
            return;
        }
        GameObject.Find("MenuButton").GetComponent<Button>().onClick.RemoveAllListeners();
        exiting = true;
        EndTurnState endTurnState = new EndTurnState(state, user.Username, turnActions, new List<UnitStats>(unitPositions.Values), cardSystem.EndTurn());

        audioManager.Play(SoundName.ButtonPress);

        if (endTurnState.IsVictory && !isSandboxMode) {
            client.EndTurn(endTurnState); //done here to prevent the user from exiting the game
            inGameMenu.victoryButton.onClick.RemoveAllListeners();
            inGameMenu.victoryButton.onClick.AddListener(() => {
                audioManager.Play(SoundName.ButtonPress);
                string path = CardMetadata.FILE_PATH_BASE + "/." + state.id + CardMetadata.FILE_EXTENSION;
                if (System.IO.File.Exists(path)) {
                    System.IO.File.Delete(path);
                }
                exiting = false;

                SceneManager.sceneLoaded += OnMenuLoaded;
                SceneManager.LoadScene("MainMenu");
            });
            inGameMenu.victoryPanel.SetActive(true);
        }
        else if(endTurnState.IsDefeat && !isSandboxMode) {
            client.EndTurn(endTurnState); //done here to prevent the user from exiting the game
            inGameMenu.defeatButton.onClick.RemoveAllListeners();
            inGameMenu.defeatButton.onClick.AddListener(() => {
                audioManager.Play(SoundName.ButtonPress);
                string path = CardMetadata.FILE_PATH_BASE + "/." + state.id + CardMetadata.FILE_EXTENSION;
                if (System.IO.File.Exists(path)) {
                    System.IO.File.Delete(path);
                }
                exiting = false;

                SceneManager.sceneLoaded += OnMenuLoaded;
                SceneManager.LoadScene("MainMenu");
            });
            inGameMenu.defeatPanel.SetActive(true);
        }
        else {
            StartCoroutine("MainMenuNavigationCountDown");

            client.EndTurn(endTurnState);
            string path = CardMetadata.FILE_PATH_BASE + "/." + state.id + CardMetadata.FILE_EXTENSION;
            if (System.IO.File.Exists(path)) {
                System.IO.File.Delete(path);
            }
        }
    }

    IEnumerator MainMenuNavigationCountDown() {
        TextMeshProUGUI countDownText = this.inGameMenu.returningToMainMenuPanel.transform.Find("CountDownText").gameObject.GetComponent<TextMeshProUGUI>();
        this.inGameMenu.returningToMainMenuPanel.SetActive(true);
        float startTime = Time.time;

        int displayTime = 3;
        while (displayTime > 0) {
            countDownText.text = displayTime + "...";
            displayTime = (int)(3 - (Time.time - startTime) + 1);
            yield return null;
        }
        exiting = false;

        SceneManager.sceneLoaded += OnMenuLoaded;
        SceneManager.LoadScene("MainMenu");
    }

    public void Forfeit() {
        if (exiting) {
            return;
        }
        audioManager.Play(SoundName.ButtonPress);
        client.ForfeitGame(state.id);
        SceneManager.LoadScene("MainMenu");
    }

    //For now just load the main menu and don't do anything else
    public void ExitGame() {
        if (exiting) {
            return;
        }
        audioManager.Play(SoundName.ButtonPress);
        SceneManager.LoadScene("MainMenu");
    }

    public void EndUnitPlacement() {
        if (exiting) {
            return;
        }
        exiting = true;
        StartCoroutine("MainMenuNavigationCountDown");
        UnitStats general = placedUnits[0];
        placedUnits.RemoveAt(0);
        ReadyUnitsGameState readyState = new ReadyUnitsGameState(state.id, placedUnits, general);
        client.ReadyUnits(readyState);
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

    public bool TileContainsUnit(Vector2Int tile) {
        return unitPositions.ContainsKey(tile);
    }

    // Returns first general position, or position of last unit
    private Vector2Int GetGeneralPosition(string username) {
        Vector2Int lastPosition = Vector2Int.zero;
        foreach (Vector2Int position in unitPositions.Keys) {
            UnitStats general = unitPositions[position];
            if(general.Owner == username) {
                if ((int)general.UnitType > UnitMetadata.GENERAL_THRESHOLD) {
                    lastPosition = position;
                    return position;
                }
                else if (lastPosition.Equals(Vector2Int.zero)) {
                    lastPosition = position;
                }
            }
        }
        return lastPosition;
    }

    //If the following conditions are true:
    //   the dictionary contains a unit at the "targetUnit" key, and does not contain a unit at the endpoint key
    public void MoveUnit(Vector2Int targetUnit, Vector2Int endpoint) {
        if (!unitPositions.ContainsKey(endpoint)) {
            if (GetUnitOnTile(targetUnit, out UnitStats unit)) {
                if(unit.MovementSpeed > 0 && unit.Owner == user.Username) {
                    unitPositions.Remove(targetUnit);
                    if(state.boardId == BoardType.Sandbox){
                        unit.SandboxMove(endpoint, ref boardController);
                    }
                    else{
                        unit.Move(endpoint, ref boardController);
                    }
                    unitPositions[endpoint] = unit;
                    turnActions.Add(new Action(user.Username, ActionType.Movement, targetUnit, endpoint, GeneralAbility.NONE, CardFunction.NONE));
                }
            }
        }
    }

    public void AttackUnit(Vector2Int source, Vector2Int target) {
        turnActions.Add(new Action(user.Username, ActionType.Attack, source, target, GeneralAbility.NONE, CardFunction.NONE));
        if (GetUnitOnTile(source, out UnitStats sourceUnit)) {
            if(sourceUnit.AttackActions > 0 && sourceUnit.Owner == user.Username) {
                List<Tuple<Vector2Int, int>> damages = sourceUnit.Attack(target, boardController.CellToWorld(source), boardController.CellToWorld(target));
                foreach (var damage in damages) {
                    if (GetUnitOnTile(damage.First, out UnitStats targetUnit)) {
                        int modifiedDamage = System.Convert.ToInt32(damage.Second * UnitMetadata.GetMultiplier(sourceUnit.UnitType, targetUnit.UnitType));
                        if (modifiedDamage > 0) {
                            if (targetUnit.TakeDamage(modifiedDamage, sourceUnit.Pierce)) {
                                unitPositions.Remove(damage.First);
                                targetUnit.Kill();
                            }
                        }
                        else {
                            targetUnit.Heal(-1 * modifiedDamage);
                        }
                    }
                }
            }
        }
    }

    //Takes in a position and an ability and does the ability
    public bool UseAbility(Vector2Int source, Vector2Int target, GeneralAbility ability) {
        UnitStats general = unitPositions[source];
        if (general.Ability1 == ability) {
            if (general.Ability1Cooldown == 0) {
                AbilityAction<UnitStats, Dictionary<Vector2Int, UnitStats>, string, bool> abilityFunction = GeneralMetadata.ActiveAbilityFunctionDictionary[ability];
                if (target != source) {
                    if(GetUnitOnTile(target, out UnitStats targetUnit)) {
                        general.Ability1Cooldown = GeneralMetadata.AbilityCooldownDictionary[ability];
                        abilityFunction(ref targetUnit, unitPositions, user.Username, true);
                    }
                    else {
                        return false; //don't put the ability on cooldown
                    }
                }
                else {
                    general.Ability1Cooldown = GeneralMetadata.AbilityCooldownDictionary[ability];
                    abilityFunction(ref general, unitPositions, user.Username, true);
                }
            }
        }
        else if(general.Ability2 == ability) {
            if (general.Ability2Cooldown == 0) {
                AbilityAction<UnitStats, Dictionary<Vector2Int, UnitStats>, string, bool> abilityFunction = GeneralMetadata.ActiveAbilityFunctionDictionary[ability];
                if (target != source) {
                    if (GetUnitOnTile(target, out UnitStats targetUnit)) {
                        general.Ability2Cooldown = GeneralMetadata.AbilityCooldownDictionary[ability];
                        abilityFunction(ref targetUnit, unitPositions, user.Username, true);
                    }
                    else {
                        return false; //don't put the ability on cooldown
                    }
                }
                else {
                    general.Ability2Cooldown = GeneralMetadata.AbilityCooldownDictionary[ability];
                    abilityFunction(ref general, unitPositions, user.Username, true);
                }
            }
        }
        turnActions.Add(new Action(user.Username, ActionType.Ability, source, target, ability, CardFunction.NONE));
        return true;
    }

    public bool UseCard(Vector2Int target, Card card) {
        CardFunction cardId = card.func;
        if (CardMetadata.CardEffectDictionary[cardId](target, unitPositions, user.Username, false)) {
            turnActions.Add(new Action(user.Username, ActionType.Card, target, target, GeneralAbility.NONE, cardId));
            return true;
        }
        else {
            return false;
        }
    }

    //===================== Functions used to get unit positions ===================
    public List<Vector2Int> GetUnitPositions(UnitType type = UnitType.none) {
        List<Vector2Int> retList = new List<Vector2Int>();
        foreach (Vector2Int pos in unitPositions.Keys) {
            UnitStats unit = unitPositions[pos];
            if((int)unit.UnitType < UnitMetadata.GENERAL_THRESHOLD) {
                if (unit.Owner == user.Username) {
                    if (type == UnitType.none || unit.UnitType == type) {
                        retList.Add(pos);
                    }
                }
            }
        }
        return retList;
    }

    //===================== Functions used to interact with the camera ===================
    public void SnapToPosition(Vector2Int pos) {
        cameraRig.SnapToPosition(boardController.CellToWorld(pos));
    }
}
