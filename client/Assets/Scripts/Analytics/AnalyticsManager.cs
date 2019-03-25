using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public Client client;
    public WindowGraph wg;

    /* We're gonna sum up the total damage each UnitType did */
    /* As well as the total damage it did to each individual other UnitType */
    public struct UnitAnalyticsValue {
        public float Total;
        public Dictionary<UnitType, float> TotalPerUnitType;
    }

    private Dictionary<Vector2Int, UnitStats> localBoard;
    
    public struct UnitValuePair {
        public float value;
        public UnitType unitType;
    }

    private Dictionary<UnitType, UnitAnalyticsValue> unitDamageStats;
    private List<UnitValuePair> totalDamagesInput;

    private Dictionary<UnitType, int> unitTotalMovementStats;
    private List<UnitValuePair> totalMovementInput;
    private List<UnitValuePair> avgMovementInput;

    private Dictionary<UnitType, int> gamesPlayed;
    private List<UnitValuePair> avgDamagesInput;

    private Dictionary<UnitType, int> timesPurchased;
    private List<UnitValuePair> timesPurchasedInput;
    private List<UnitValuePair> avgTimesPurchased;

    void Start()
    {
        if (!client.LoginUser("ParkerReese1", "5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8")) {
            Debug.LogError("Failed to Login");
            return;
        }

        Tuple<bool, GameStateCollection> response = client.GetAllCompletedGames();

        if (!response.First) {
            Debug.LogError("Failed to get completed games");
            return;
        }
        int totalGames = response.Second.states.Count;
        if (totalGames <= 0) {
            Debug.Log("There are no completed games");
            return;
        }
        
        Debug.Log("Recieved " + totalGames + " Completed Games.");

        unitDamageStats = new Dictionary<UnitType, UnitAnalyticsValue>();
        gamesPlayed = new Dictionary<UnitType, int>();
        timesPurchased = new Dictionary<UnitType, int>();
        unitTotalMovementStats = new Dictionary<UnitType, int>();
        
        /* We now have a list of GameStates to loop over and build data with */
        foreach(GameState gameState in response.Second.states) {
            if (gameState.InitUnits == null || gameState.InitUnits.Count <= 0) {
                Debug.LogError("There are no units for this game?? : " + gameState.id);
                continue;
            }

            if (gameState.Actions.Count <= 0) {
                Debug.LogError("There are no actions for this game?? : " + gameState.id);
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
                /* Increment unit in Times Purchased Dict */
                if (!timesPurchased.ContainsKey(unit.UnitType)) {
                    timesPurchased.Add(unit.UnitType, 1);
                } else {
                    timesPurchased[unit.UnitType] = (timesPurchased[unit.UnitType] + 1);
                }
            }
            if (gameState.InitGenerals != null && gameState.InitGenerals.Count > 0) {
                /* this is a copy paste of the above loop cuz im lazy and it's a dev scene */
                foreach(UnitStats unit in gameState.InitGenerals) {
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
                    /* Increment unit in Times Purchased Dict */
                    if (!timesPurchased.ContainsKey(unit.UnitType)) {
                        timesPurchased.Add(unit.UnitType, 1);
                    } else {
                        timesPurchased[unit.UnitType] = (timesPurchased[unit.UnitType] + 1);
                    }
                }
            }

            /* Loop over the actions and pool the data */
            foreach(Action action in gameState.Actions) {
                switch (action.Type)
                {
                    case ActionType.Movement:
                        movementAction(action.OriginXPos, action.OriginYPos, action.TargetXPos, action.TargetYPos);
                        break;
                    case ActionType.Attack:
                        attackAction(action.OriginXPos, action.OriginYPos, action.TargetXPos, action.TargetYPos);
                        break;
                    default:
                        Debug.Log("Unhandled Action: " + action.Type);
                        break;
                }
            }
        }

        /* compile data for Total and Avg Damages Graph */
        totalDamagesInput = new List<UnitValuePair>();
        avgDamagesInput = new List<UnitValuePair>();
        foreach(KeyValuePair<UnitType, UnitAnalyticsValue> pair in unitDamageStats) {
            UnitValuePair totalSD = new UnitValuePair();
            totalSD.value = pair.Value.Total;
            totalSD.unitType = pair.Key;
            totalDamagesInput.Add(totalSD);

            UnitValuePair avgSD = new UnitValuePair();
            avgSD.value = pair.Value.Total/gamesPlayed[pair.Key];
            avgSD.unitType = pair.Key;
            avgDamagesInput.Add(avgSD);
        }

        /* show the first graph while we quickly build up the other graphs */
        wg.ShowGraph(totalDamagesInput, unitDamageStats, "Total Damages Per Unit");

        /* Compile data for Total and Avg Movement */
        totalMovementInput = new List<UnitValuePair>();
        avgMovementInput = new List<UnitValuePair>();
        foreach(KeyValuePair<UnitType, int> pair in unitTotalMovementStats) {
            UnitValuePair totalSD = new UnitValuePair();
            totalSD.value = pair.Value;
            totalSD.unitType = pair.Key;
            totalMovementInput.Add(totalSD);

            UnitValuePair avgSD = new UnitValuePair();
            avgSD.value = pair.Value / gamesPlayed[pair.Key];
            avgSD.unitType = pair.Key;
            avgMovementInput.Add(avgSD);
        }

        /* compile data for Total Purchased times */
        timesPurchasedInput = new List<UnitValuePair>();
        foreach(KeyValuePair<UnitType, int> pair in timesPurchased) {
            UnitValuePair sd = new UnitValuePair();
            sd.value = pair.Value;
            sd.unitType = pair.Key;
            timesPurchasedInput.Add(sd);
        }

        /* Compile data for average army composition */
        avgTimesPurchased = new List<UnitValuePair>();
        float totalRatio = 0;
        foreach(KeyValuePair<UnitType, int> pair in timesPurchased) {
            float ratio = (pair.Value / totalGames);
            totalRatio += ratio;
            UnitValuePair sd = new UnitValuePair();
            sd.value = ratio;
            sd.unitType = pair.Key;
            avgTimesPurchased.Add(sd);
        }
        for(int i =0; i < avgTimesPurchased.Count; i++) {
            float percentage = (avgTimesPurchased[i].value / totalRatio) * 100;
            UnitValuePair sd = new UnitValuePair();
            sd.value = percentage;
            sd.unitType = avgTimesPurchased[i].unitType;
            avgTimesPurchased[i] = sd;
        }
    }

    private void movementAction(int sourceXPos, int sourceYPos, int targetXPos, int targetYPos) {
        UnitStats unitThatMoved;
        Vector2Int source = new Vector2Int(sourceXPos, sourceYPos);
        Vector2Int target = new Vector2Int(targetXPos, targetYPos);

        /* Error checks */
        if (!localBoard.TryGetValue(source, out unitThatMoved)){
            Debug.LogError("No unit with this movement source. X:" + source.x + ", Y:" + source.y);
            return;
        }
        if (localBoard.ContainsKey(target)) {
            Debug.LogError("Unit can't move to occupied target");
            return;
        }
        /* Get the Distance between the two locations */
        int distance = (int) HexUtility.HexDistance(source, target);

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
            Debug.LogError("No unit with this attack source. X:" + source.x + ", Y:" + source.y);
            return;
        }

        /* Get the array of simulated damages */
        List<UnitValuePair> damages = GetSimulatedDamage(sourceUnit, target, localBoard);
        if (damages.Count > 0) {
            /* They hit something(s) */
            foreach(UnitValuePair damage in damages) {
            /* Update the Dict to increase total damages */
            UnitAnalyticsValue curUav;
            if (!unitDamageStats.TryGetValue(sourceUnit.UnitType, out curUav)) {
                // UnitType is not yet in Dict
                curUav.Total = damage.value;
                curUav.TotalPerUnitType = new Dictionary<UnitType, float>();
                curUav.TotalPerUnitType[damage.unitType] = damage.value;
            } else {
                /* UnitType is in the Dict already, don't erase values, increment them */
                curUav.Total = curUav.Total + damage.value;
                
                /* Also increment the specific damage done to that UnitType */
                float totalDamagePerUnitType;
                if (!curUav.TotalPerUnitType.TryGetValue(damage.unitType, out totalDamagePerUnitType)) {
                    curUav.TotalPerUnitType[damage.unitType] = damage.value;
                } else {
                    curUav.TotalPerUnitType[damage.unitType] = totalDamagePerUnitType + damage.value;
                }
            }
            /* Put all changes */
            unitDamageStats[sourceUnit.UnitType] = curUav;
            }

        }
        return;
    }

    /* Function to get the list of damages on each unit that an attack inflicts */
    private List<UnitValuePair> GetSimulatedDamage(UnitStats sourceUnit, Vector2Int target, Dictionary<Vector2Int, UnitStats> fakeBoard) {
        List<UnitValuePair> damageList = new List<UnitValuePair>();

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
                    UnitValuePair sd;
                    sd.value = damageDiff;
                    sd.unitType = targetUnit.UnitType;
                    damageList.Add(sd);
                }
            }
        }
        return damageList;
    }

    public void setToDamageTotals() {
        wg.ShowGraph(totalDamagesInput, unitDamageStats, "Total Damages Per Unit");
    }

    public void setToDamageAverages() {
        wg.ShowGraph(avgDamagesInput, null, "Average Damages Per Unit Per Game");
    }

    public void setToMovementTotals() {
        wg.ShowGraph(totalMovementInput, null, "Total Movement Per Unit");
    }

    public void setToMovementAvgs() {
        wg.ShowGraph(avgMovementInput, null, "Average Movement Per Unit Per Game");
    }

    public void setToTimesPurchased() {
        wg.ShowGraph(timesPurchasedInput, null, "Times Purchased");
    }

    public void setToAvgArmyComposition() {
        wg.ShowGraph(avgTimesPurchased, null, "Average Army Composition (ratio : 100%)");
    }
}

