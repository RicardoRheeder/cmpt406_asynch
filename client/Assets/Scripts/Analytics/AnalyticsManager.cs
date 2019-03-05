using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public Client client;
    public Window_Graph wg;

    /* We're gonna sum up the total damage each UnitType did */
    /* As well as the total damage it did to each individual other UnitType */
    private Dictionary<Vector2Int, UnitStats> localBoard;
    public struct UnitAnalyticsValue {
        public int Total;
        public Dictionary<UnitType, int> TotalPerUnitType;
    }

    private Dictionary<UnitType, UnitAnalyticsValue> unitDamageStats;
    private List<SimulatedDamage> totalDamagesInput;

    private Dictionary<UnitType, int> unitTotalMovementStats;
    private List<SimulatedDamage> totalMovementInput;
    private List<SimulatedDamage> avgMovementInput;

    private Dictionary<UnitType, int> gamesPlayed;
    private List<SimulatedDamage> avgDamagesInput;

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

        unitDamageStats = new Dictionary<UnitType, UnitAnalyticsValue>();
        gamesPlayed = new Dictionary<UnitType, int>();
        unitTotalMovementStats = new Dictionary<UnitType, int>();
        
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
            
            Dictionary<UnitType, bool> gamesPlayedChecker = new Dictionary<UnitType, bool>();
            /* Initialize the local board given the initUnits */
            localBoard = new Dictionary<Vector2Int, UnitStats>();

            foreach(UnitStats unit in gameState.InitUnits) {
                localBoard[unit.Position] = UnitFactory.GetBaseUnit(unit.UnitType);

                /* Add unit to GamesPlayed for Damage Averages */
                if (!gamesPlayedChecker.ContainsKey(unit.UnitType)) {
                    gamesPlayedChecker.Add(unit.UnitType, true);
                    if (gamesPlayed.ContainsKey(unit.UnitType)) {
                        gamesPlayed[unit.UnitType] = gamesPlayed[unit.UnitType] + 1;
                    }
                    else {
                        gamesPlayed[unit.UnitType] = 1;
                    }
                }
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

        /* Data for TotalDamages Graph */
        totalDamagesInput = new List<SimulatedDamage>();
        foreach(KeyValuePair<UnitType, UnitAnalyticsValue> pair in unitDamageStats) {
            SimulatedDamage sd = new SimulatedDamage();
            sd.damage = pair.Value.Total;
            sd.unitType = pair.Key;
            totalDamagesInput.Add(sd);
        }

        wg.ShowGraph(totalDamagesInput, unitDamageStats, null, "Total Damages Per Unit");

        /* Data for Avg Damage graph */
        avgDamagesInput = new List<SimulatedDamage>();
        foreach(KeyValuePair<UnitType, UnitAnalyticsValue> pair in unitDamageStats) {
            SimulatedDamage sd = new SimulatedDamage();
            sd.damage = pair.Value.Total/gamesPlayed[pair.Key];
            sd.unitType = pair.Key;
            avgDamagesInput.Add(sd);
        }

        /* Compile data for Total Movement */
        totalMovementInput = new List<SimulatedDamage>();
        foreach(KeyValuePair<UnitType, int> pair in unitTotalMovementStats) {
            SimulatedDamage sd = new SimulatedDamage();
            sd.damage = pair.Value;
            sd.unitType = pair.Key;
            totalMovementInput.Add(sd);
        }

        /* Compile data for Avg Movement */
        avgMovementInput = new List<SimulatedDamage>();
        foreach(KeyValuePair<UnitType, int> pair in unitTotalMovementStats) {
            SimulatedDamage sd = new SimulatedDamage();
            sd.damage = pair.Value / gamesPlayed[pair.Key];
            sd.unitType = pair.Key;
            avgMovementInput.Add(sd);
        }

    }

    private void movementAction(int sourceXPos, int sourceYPos, int targetXPos, int targetYPos) {
        UnitStats unitThatMoved;
        Vector2Int source = new Vector2Int(sourceXPos, sourceYPos);
        Vector2Int target = new Vector2Int(targetXPos, targetYPos);
        Debug.Log("We have a movement");
        /* Error checks */
        if (!localBoard.TryGetValue(source, out unitThatMoved)){
            Debug.LogError("No unit with this source");
            return;
        }
        if (localBoard.ContainsKey(target)) {
            Debug.LogError("Unit can't move to occupied target");
            return;
        }
        /* Get the Distance between the two locations */
        int distance = 3;

        /* Add the distance to the graph data */
        if (unitTotalMovementStats.ContainsKey(unitThatMoved.UnitType)) {
            unitTotalMovementStats[unitThatMoved.UnitType] = unitTotalMovementStats[unitThatMoved.UnitType] + distance;
        }
        else {
            unitTotalMovementStats[unitThatMoved.UnitType] = distance;
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
            if (!unitDamageStats.TryGetValue(sourceUnit.UnitType, out curUav)) {
                // UnitType is not yet in Dict
                curUav.Total = damage.damage;
                curUav.TotalPerUnitType = new Dictionary<UnitType, int>();
                curUav.TotalPerUnitType[damage.unitType] = damage.damage;
            } else {
                /* UnitType is in the Dict already, don't erase values, increment them */
                curUav.Total = curUav.Total + damage.damage;
                
                /* Also increment the specific damage done to that UnitType */
                int totalDamagePerUnitType;
                if (!curUav.TotalPerUnitType.TryGetValue(damage.unitType, out totalDamagePerUnitType)) {
                    curUav.TotalPerUnitType[damage.unitType] = damage.damage;
                } else {
                    curUav.TotalPerUnitType[damage.unitType] = totalDamagePerUnitType + damage.damage;
                }
            }
            /* Put all changes */
            unitDamageStats[sourceUnit.UnitType] = curUav; // Note, using dict[key] = val will overwrite values. dict.Add() will not overwrite values
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
        wg.ShowGraph(totalDamagesInput, unitDamageStats, null, "Total Damages Per Unit");
    }

    public void setToDamageAverages() {
        wg.ShowGraph(avgDamagesInput, null, null, "Average Damages Per Unit Per Game");
    }

    public void setToMovementTotals() {
        wg.ShowGraph(totalMovementInput, null, null, "Total Movement Per Unit");
    }

    public void setToMovementAvgs() {
        wg.ShowGraph(avgMovementInput, null, null, "Average Movement Per Unit Per Game");
    }
}

