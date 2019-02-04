using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class has to extend from monobehaviour so it can be created before a scene is loaded.
public class GameManager : MonoBehaviour {

    private GameBuilder builder;
    private PlayerController player;

    private List<Action> turnActions;

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);
        builder = new GameBuilder();
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
}
