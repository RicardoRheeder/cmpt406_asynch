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


    //For testing the units
    Dictionary<Vector3Int, UnitStats> units = new Dictionary<Vector3Int, UnitStats>(); 

    UnitStats testUnit_1 = new UnitStats(60, 100, 25, 1, 10, 3, 4, 5); // test unit
    UnitStats testUnit_2 = new UnitStats(85, 200, 30, 3, 50, 7, 8, 2); // test unit
    UnitStats testUnit_3 = new UnitStats(30, 100, 80, 6, 30, 9, 2, 4); // test unit
    UnitStats temp = new UnitStats(0, 0, 0, 0, 0, 0, 0, 0); // test unit



    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);
        builder = new GameBuilder();

        playerControllerObject = GameObject.Find("PlayerController");
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.Initialize(this, null); //Somewhere deck will be created, now you can just pass in null

        // for testing
        units.Add(new Vector3Int(0,0,0),testUnit_1);
        units.Add(new Vector3Int(0, -2, 0), testUnit_2);
        units.Add(new Vector3Int(1, 3, 0), testUnit_3);
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

    //public UnitStats GetUnitOnTile(Vector3Int tile)
    //{
        
    //    //print("this unit");
    //    return new UnitStats(50, 100, 10, 2, 5, 3, 4, 5); //update this for later
    //}


    //Update this for later
    public UnitStats GetUnitOnTile(Vector3Int tile)
    {

        //if key is present in the dictonary
        if (units.ContainsKey(tile))
        {
            return units[tile]; //return unit, using the key
        }
        else {
            return temp; //otherwise return temp, which is value of zero in unit stats
        }
    }

    
}
