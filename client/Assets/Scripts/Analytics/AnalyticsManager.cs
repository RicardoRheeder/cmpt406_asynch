using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public Client client;

    /* We're gonna sum up the total damage each UnitType did */
    /* As well as the total damage it did to each individual other UnitType */
    private Dictionary<UnitType, UnitAnalyticsValue> unitStats;
    private struct UnitAnalyticsValue {
        public int TotalDamage;
        public Dictionary<UnitType, int> DamagePerUnitType;
    }

    private Dictionary<Vector2Int, UnitType> localBoard;

    void Start()
    {
        if (!client.LoginUser("ParkerReese1", "password")) {
            Debug.LogError("Failed to Login");
            return;
        }

        Tuple<bool, GameStateCollection> response = client.GetAllCompletedGames();

        if (!response.First) {
            Debug.LogError("Failed to get completed games");
            return;
        }
        if (response.Second.states.Count <= 0) {
            Debug.Log("There are no completed games");
            return;
        }

        unitStats = new Dictionary<UnitType, UnitAnalyticsValue>();
        
        /* We now have a list of GameStates to loop over and build data with */
        foreach(GameState gameState in response.Second.states) {
            if (gameState.InitUnits.Count <= 0) {
                Debug.LogError("There are no units for this game??");
                continue;
            }
            if (gameState.Actions.Count <= 0) {
                Debug.LogError("There are no actions for this game??");
                continue;
            }
            
            /* Initialize the local board given the initUnits */
            localBoard = new Dictionary<Vector2Int, UnitType>();
            foreach(UnitStats unit in gameState.InitUnits) {
                localBoard[unit.Position] = unit.UnitType;
            }

            /* Loop over the actions and pool the data */
            foreach(Action action in gameState.Actions) {
                 switch (action.Type)
                {
                    case ActionType.Movement:
                        movementAction(action.OriginXPos, action.OriginXPos, action.TargetXPos, action.TargetYPos);
                        break;
                    case ActionType.Attack:
                        attackAction(action.OriginXPos, action.OriginXPos, action.TargetXPos, action.TargetYPos);
                        break;
                    default:
                        Debug.Log("Unhandled Action");
                        break;
                }
            }
        }

        /* TODO: Now show this data somehow */
        foreach(KeyValuePair<UnitType, UnitAnalyticsValue> pair in unitStats) {
            Debug.Log("UnitType: " + pair.Key + " Has a total damage of: " + pair.Value.TotalDamage);
        }
    }

    private void movementAction(int sourceXPos, int sourceYPos, int targetXPos, int targetYPos) {
        UnitType unitTypeThatMoved;
        Vector2Int source = new Vector2Int(sourceXPos, sourceYPos);
        Vector2Int target = new Vector2Int(targetXPos, targetYPos);
        /* Error checks */
        if (!localBoard.TryGetValue(source, out unitTypeThatMoved)){
            Debug.LogError("No unit with this source");
            return;
        }
        if (localBoard.ContainsKey(target)) {
            Debug.LogError("Unit can't move to occupied target");
            return;
        }

        /* Move the unit */
        localBoard.Remove(source);
        localBoard.Add(target, unitTypeThatMoved);

        return;
    }

    private void attackAction(int sourceXPos, int sourceYPos, int targetXPos, int targetYPos) {
        UnitType sourceUnitType;
        UnitType targetUnitType;
        
        Vector2Int source = new Vector2Int(sourceXPos, sourceYPos);
        Vector2Int target = new Vector2Int(targetXPos, targetYPos);

        /* Get the units that are involved */
        if (!localBoard.TryGetValue(source, out sourceUnitType)){
            Debug.LogError("No unit with this source");
            return;
        }
        if (!localBoard.TryGetValue(target, out targetUnitType)){
            Debug.LogError("No unit with this target");
            return;
        }

        /* TODO: Find the damage that the source unit does */
        int damage = 5;

        /* Update the Dict to increase total damages */

        UnitAnalyticsValue curUav;
        if (!unitStats.TryGetValue(sourceUnitType, out curUav)) {
            // UnitType is not yet in Dict
            curUav.TotalDamage = damage;
            curUav.DamagePerUnitType = new Dictionary<UnitType, int>();
            curUav.DamagePerUnitType[targetUnitType] = damage;
        } else {
            /* UnitType is in the Dict already, don't erase values, increment them */
            curUav.TotalDamage = curUav.TotalDamage + damage;
            
            /* Also increment the specific damage done to that UnitType */
            int totalDamagePerUnitType;
            if (!curUav.DamagePerUnitType.TryGetValue(targetUnitType, out totalDamagePerUnitType)) {
                curUav.DamagePerUnitType[targetUnitType] = damage;
            } else {
                curUav.DamagePerUnitType[targetUnitType] = totalDamagePerUnitType + damage;
            }
        }
        /* Put all changes */
        unitStats[sourceUnitType] = curUav; // Note, using dict[key] = val will overwrite values. dict.Add() will not overwrite values

        return;
    }
}

