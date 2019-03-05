using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public Client client;
    public GameManager gm;
    public Window_Graph wg;

    /* We're gonna sum up the total damage each UnitType did */
    /* As well as the total damage it did to each individual other UnitType */
    private Dictionary<UnitType, UnitAnalyticsValue> unitStats;
    public struct UnitAnalyticsValue {
        public int TotalDamage;
        public Dictionary<UnitType, int> DamagePerUnitType;
    }

    private Dictionary<Vector2Int, UnitStats> localBoard;
    private List<SimulatedDamage> inputToShowGraph;

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
            if (gameState.InitUnits == null || gameState.InitUnits.Count <= 0) {
                Debug.LogError("There are no units for this game??");
                continue;
            }
            if (gameState.Actions.Count <= 0) {
                Debug.LogError("There are no actions for this game??");
                continue;
            }
            
            /* Initialize the local board given the initUnits */
            localBoard = new Dictionary<Vector2Int, UnitStats>();
            foreach(UnitStats unit in gameState.InitUnits) {
                localBoard[unit.Position] = UnitFactory.GetBaseUnit(unit.UnitType);
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
                        Debug.Log("Unhandled Action: " + action.Type);
                        break;
                }
            }
        }

        /* Now give the data to make a graph */
        inputToShowGraph = new List<SimulatedDamage>();
        foreach(KeyValuePair<UnitType, UnitAnalyticsValue> pair in unitStats) {
            SimulatedDamage sd = new SimulatedDamage();
            sd.damage = pair.Value.TotalDamage;
            sd.unitType = pair.Key;
            inputToShowGraph.Add(sd);
        }

        wg.ShowGraph(inputToShowGraph, unitStats, null, "Total Damages Per Unit");
    }

    private void movementAction(int sourceXPos, int sourceYPos, int targetXPos, int targetYPos) {
        UnitStats unitThatMoved;
        Vector2Int source = new Vector2Int(sourceXPos, sourceYPos);
        Vector2Int target = new Vector2Int(targetXPos, targetYPos);
        /* Error checks */
        if (!localBoard.TryGetValue(source, out unitThatMoved)){
            Debug.LogError("No unit with this source");
            return;
        }
        if (localBoard.ContainsKey(target)) {
            Debug.LogError("Unit can't move to occupied target");
            return;
        }

        /* Move the unit */
        localBoard.Remove(source);
        localBoard.Add(target, unitThatMoved);

        return;
    }

    private void attackAction(int sourceXPos, int sourceYPos, int targetXPos, int targetYPos) {
        UnitStats sourceUnit;
        
        Vector2Int source = new Vector2Int(sourceXPos, sourceYPos);
        Vector2Int target = new Vector2Int(targetXPos, targetYPos);

        /* Get the units that are involved */
        if (!localBoard.TryGetValue(source, out sourceUnit)){
            Debug.LogError("No unit with this source");
            return;
        }

        /* Get the array of simulated damages */
        List<SimulatedDamage> damages = GetSimulatedDamage(sourceUnit, target, localBoard);
        if (damages.Count > 0) {
            /* They hit something(s) */
            foreach(SimulatedDamage damage in damages) {
                /* Update the Dict to increase total damages */
            UnitAnalyticsValue curUav;
            if (!unitStats.TryGetValue(sourceUnit.UnitType, out curUav)) {
                // UnitType is not yet in Dict
                curUav.TotalDamage = damage.damage;
                curUav.DamagePerUnitType = new Dictionary<UnitType, int>();
                curUav.DamagePerUnitType[damage.unitType] = damage.damage;
            } else {
                /* UnitType is in the Dict already, don't erase values, increment them */
                curUav.TotalDamage = curUav.TotalDamage + damage.damage;
                
                /* Also increment the specific damage done to that UnitType */
                int totalDamagePerUnitType;
                if (!curUav.DamagePerUnitType.TryGetValue(damage.unitType, out totalDamagePerUnitType)) {
                    curUav.DamagePerUnitType[damage.unitType] = damage.damage;
                } else {
                    curUav.DamagePerUnitType[damage.unitType] = totalDamagePerUnitType + damage.damage;
                }
            }
            /* Put all changes */
            unitStats[sourceUnit.UnitType] = curUav; // Note, using dict[key] = val will overwrite values. dict.Add() will not overwrite values
            }

        }

        return;
    }

    // This is used for analytics. No, I don't like it, but it was the fastest version of doing this.
    public struct SimulatedDamage {
        public int damage;
        public UnitType unitType;
    }

    private List<SimulatedDamage> GetSimulatedDamage(UnitStats sourceUnit, Vector2Int target, Dictionary<Vector2Int, UnitStats> fakeBoard) {
        List<SimulatedDamage> damageList = new List<SimulatedDamage>();

        List<Tuple<Vector2Int, int>> damages = sourceUnit.Attack(target);
        foreach (var damage in damages) {

            bool containsUnit = fakeBoard.ContainsKey(damage.First);
            UnitStats targetUnit = containsUnit ? fakeBoard[damage.First] : null;

            if (targetUnit != null) {
                int initHealth = targetUnit.CurrentHP;

                int modifiedDamage = System.Convert.ToInt32(damage.Second * UnitMetadata.GetMultiplier(sourceUnit.UnitType, targetUnit.UnitType));
                targetUnit.TakeDamage(modifiedDamage, sourceUnit.Pierce);
                int damageDiff = initHealth - targetUnit.CurrentHP;

                // Add this damage too list of damages
                if (damageDiff > 0) {
                    SimulatedDamage sd;
                    sd.damage = damageDiff;
                    sd.unitType = targetUnit.UnitType;
                    damageList.Add(sd);
                }
            }
        }
        return damageList;
    }

    public void setToDamageTotals() {
        wg.ShowGraph(inputToShowGraph, unitStats, null, "Total Damages Per Unit");
    }
}

