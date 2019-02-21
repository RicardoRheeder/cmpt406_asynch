using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public static class ArmyBuilder {

    private static Dictionary<string, ArmyPreset> ARMY_PRESETS = new Dictionary<string, ArmyPreset>();

    //This method will used to cache the players army presets, so all of the logic is in a single place
    public static void AddPreset(string armyName, ArmyPreset army) {
        ARMY_PRESETS[armyName] = army;
    }

    //This method will be used to get the list of units we are going to be placing on the board
    public static bool GetPreset(string armyName, out ArmyPreset army) {
        if(ARMY_PRESETS.ContainsKey(armyName)) {
            army = ARMY_PRESETS[armyName];
            return true;
        }
        army = null;
        return false;
    }

    //ONLY CALL THIS WHEN WE NEED TO CLEAR THE PRESETS FOR SOME REASON
    //The only use case I can think of for this is when the user is logged out.
    public static void Clear() {
        ARMY_PRESETS.Clear();
    }


    private static readonly string presetOneName = "Preset One";
    private static readonly ArmyPreset presetOne = new ArmyPreset(
        presetOneName,
        new List<UnitType>() {
            UnitType.compensator,
            UnitType.compensator,
            UnitType.compensator
        },
        UnitType.general1
    );
    private static readonly string presetTwoName = "Preset Two";
    private static readonly ArmyPreset presetTwo = new ArmyPreset(
        presetTwoName,
        new List<UnitType>() {
            UnitType.compensator,
            UnitType.compensator,
            UnitType.compensator
        },
        UnitType.general1
    );
    private static readonly string presetThreeName = "Preset Three";
    private static readonly ArmyPreset presetThree = new ArmyPreset(
        presetThreeName,
        new List<UnitType>() {
            UnitType.compensator,
            UnitType.compensator,
            UnitType.compensator
        },
        UnitType.general1
    );
    //Method used to create the default presets and add them to the dictionary
    public static void InsertDefaultPresets() {
        if (!ARMY_PRESETS.ContainsKey(presetOneName)) {
            AddPreset(presetOneName, presetOne);
        }
        if (!ARMY_PRESETS.ContainsKey(presetTwoName)) {
            AddPreset(presetTwoName, presetTwo);
        }
        if (!ARMY_PRESETS.ContainsKey(presetThreeName)) {
            AddPreset(presetThreeName, presetThree);
        }
    }
}

//This class will be used within the player metadata
[DataContract]
public class ArmyPreset {

    [DataMember]
    public string id;

    [DataMember]
    public string name;

    [DataMember(Name = "units")]
    private List<int> serverUnits = new List<int>();
    public List<UnitType> units = new List<UnitType>();

    [DataMember(Name = "general")]
    private int serverGeneral;
    public UnitType general;

    public ArmyPreset(string presetName, List<UnitType> units, UnitType general) {
        id = "";
        this.name = presetName;
        this.units = units;
        this.general = general;
    }

    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context) {
        for(int i = 0; i < serverUnits.Count; i++) {
            units.Add((UnitType)serverUnits[i]);
        }
        general = (UnitType)serverGeneral;
    }
}
