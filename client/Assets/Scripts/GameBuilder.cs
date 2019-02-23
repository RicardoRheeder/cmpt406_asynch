using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBuilder : MonoBehaviour {

    public Dictionary<Vector2Int, UnitStats> unitPositions = new Dictionary<Vector2Int, UnitStats>();

    private GameState state;
    private string username;

    //Method that takes in a game state, instantiates all of the objects and makes sure the scene is setup how it should be.
    //Note: the game manager is responsible for creating the other managers, the game builder is just responsible for creating the playable objects.
    public void Build(ref GameState state, ref string username) {
        this.state = state;
        this.username = username;
    }

    private void InstantiateUnits() {
        List<UnitStats> units = state.UserUnitsMap[username];
        for (int i = 0; i < units.Count; i++) {
            UnitStats unit = units[i];
            unitPositions.Add(unit.Position, unit);
            //Instantiate the at the appropriate location here
        }
    }
}
