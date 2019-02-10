using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class has to extend from monobehaviour so it can be created before a scene is loaded.
public class GameManager : MonoBehaviour {

    private GameBuilder builder;

    [SerializeField]
    private GameObject playerControllerPrefab;

   private PlayerController player;
    private GameObject playerControllerObject;
    private PlayerController playerController;

    private List<Action> turnActions;

    //Dictionary<Vector2Int, UnitStats> units;

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);
        builder = new GameBuilder();

        playerControllerObject = GameObject.Find("PlayerController");
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.Initialize(this, null); //Somewhere deck will be created, now you can just pass in null
    }

    void LoadGame(GameState state, string username) {
        CardController deck = new CardController(
            new List<Card>(state.hand[username]),
            new List<Card>(state.drawPile[username]),
            new List<Card>(state.discardPile[username]));
        player = new PlayerController(this, deck); 

        builder.Build(state);

        turnActions = new List<Action>();
    }

    public void AddAction(Action action) {
        turnActions.Add(action);
    }

    //end the users turn
    public void EndTurn() {

    }

    public UnitStats GetUnitOnTile(Vector3 tile)
    {

        print("this unit");
        return new UnitStats(50, 100, 10, 2, 5, 3, 4, 5); //update this for later
    }

    
}
