using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class has to extend from monobehaviour so it can be created before a scene is loaded.
public class GameManager : MonoBehaviour {

    private GameBuilder builder;

    [SerializeField]
    private GameObject playerControllerPrefab;
    private GameObject playerControllerObject;
    private PlayerController playerController;

    private List<Action> turnActions;


    //For testing the units
    Dictionary<Vector3Int, UnitStats> unitPositions = new Dictionary<Vector3Int, UnitStats>();

    UnitStats testUnit_1 = UnitFactory.CreateClaymore();
    UnitStats testUnit_2 = UnitFactory.CreateCompensator();
    UnitStats testUnit_3 = UnitFactory.CreatePewPew();
    UnitStats temp = UnitFactory.CreateTrooper();



    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);
        builder = new GameBuilder();

        playerControllerObject = GameObject.Find("PlayerController");
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.Initialize(this, null); //Somewhere deck will be created, now you can just pass in null

        // for testing
        unitPositions.Add(new Vector3Int(0,0,0),testUnit_1);
        unitPositions.Add(new Vector3Int(0, -2, 0), testUnit_2);
        unitPositions.Add(new Vector3Int(1, 3, 0), testUnit_3);
    }

    void LoadGame(GameState state, string username) {
        CardController deck = new CardController(
            new List<Card>(state.hand[username]),
            new List<Card>(state.drawPile[username]),
            new List<Card>(state.discardPile[username])
        );
        playerController = new PlayerController(this, deck); 

        builder.Build(state);

        turnActions = new List<Action>();
    }

    public void AddAction(Action action) {
        turnActions.Add(action);
    }

    //end the users turn
    public void EndTurn() {

    }

    //Update this for later
    public UnitStats GetUnitOnTile(Vector3Int tile) {

        //if key is present in the dictonary
        if (unitPositions.ContainsKey(tile)) {
            return unitPositions[tile]; //return unit, using the key
        }
        else {
            return null; //otherwise return temp, which is value of zero in unit stats
        }
    }
}
