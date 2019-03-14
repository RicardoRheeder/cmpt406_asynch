﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public static class ArmyBuilder {

    private static Dictionary<string, ArmyPreset> ARMY_PRESETS = new Dictionary<string, ArmyPreset>();

    //This method will used to cache the players army presets, so all of the logic is in a single place
    public static void AddPreset(string armyName, ArmyPreset army) {
        ARMY_PRESETS[armyName] = army;
    }

    public static List<ArmyPreset> GetPresetsUnderCost(int cost) {
        List<ArmyPreset> presets = new List<ArmyPreset>();
        foreach (var pair in ARMY_PRESETS) {
            if(pair.Value.Cost < cost) {
                presets.Add(pair.Value);
            }
        }
        return presets;
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


    private static readonly string presetOneName = "Steamy Ray Vaughn";
    private static readonly ArmyPreset presetOne = new ArmyPreset(
        presetOneName,
        new List<int>() {
            (int)UnitType.steamer,
            (int)UnitType.steamer,
            (int)UnitType.steamer,
            (int)UnitType.steamer,
            (int)UnitType.steamer,
            (int)UnitType.steamer,
            (int)UnitType.powerSurge,
            (int)UnitType.powerSurge,
            (int)UnitType.trooper,
            (int)UnitType.trooper,
            
            
        },
        (int)UnitType.heavy_albarn
    );
    private static readonly string presetTwoName = "\"Big\" Donny";
    private static readonly ArmyPreset presetTwo = new ArmyPreset(
        presetTwoName,
        new List<int>() {
            (int)UnitType.compensator,
            (int)UnitType.compensator,
            (int)UnitType.compensator,
            (int)UnitType.compensator,
            (int)UnitType.compensator,
            (int)UnitType.foundation,
            (int)UnitType.foundation,
            (int)UnitType.foundation,
            (int)UnitType.midas,
            (int)UnitType.midas,
        },
        (int)UnitType.piercing_tungsten
    );
    private static readonly string presetThreeName = "Super Friendship Force";
    private static readonly ArmyPreset presetThree = new ArmyPreset(
        presetThreeName,
        new List<int>() {
            (int)UnitType.trooper,
            (int)UnitType.trooper,
            (int)UnitType.foundation,
            (int)UnitType.foundation,
            (int)UnitType.recon,
            (int)UnitType.recon,
            (int)UnitType.compensator,
            (int)UnitType.compensator,
            (int)UnitType.powerSurge,
            (int)UnitType.powerSurge,
        },
        (int)UnitType.light_adren
    );
    private static readonly string presetFourName = "Trash";
    private static readonly ArmyPreset presetFour = new ArmyPreset(
        presetFourName,
        new List<int>() {
            (int)UnitType.trooper,
        },
        (int)UnitType.light_adren
    );
    //Method used to create the default presets and add them to the dictionary
    public static void InsertDefaultPresets() {
        if (!ARMY_PRESETS.ContainsKey(presetOneName)) {
            presetOne.Cost = UnitFactory.CalculateCost(presetOne.Units);
            AddPreset(presetOneName, presetOne);
        }
        if (!ARMY_PRESETS.ContainsKey(presetTwoName)) {
            presetTwo.Cost = UnitFactory.CalculateCost(presetTwo.Units);
            AddPreset(presetTwoName, presetTwo);
        }
        if (!ARMY_PRESETS.ContainsKey(presetThreeName)) {
            presetThree.Cost = UnitFactory.CalculateCost(presetThree.Units);
            AddPreset(presetThreeName, presetThree);
        }
        if (!ARMY_PRESETS.ContainsKey(presetFourName)) {
            presetThree.Cost = UnitFactory.CalculateCost(presetFour.Units);
            AddPreset(presetFourName, presetFour);
        }
    }
}

//This class will be used within the player metadata
[DataContract]
public class ArmyPreset {

    [DataMember(Name = "id")]
    public string Id;

    [DataMember(Name = "name")]
    public string Name;

    public int Cost { get; set; }

    [DataMember(Name = "units")]
    public List<int> Units { get; private set; }

    [DataMember(Name = "general")]
    public int General { get; private set; }

    public ArmyPreset(string presetName, List<int> units, int general) {
        Id = "";
        this.Name = presetName;
        this.Units = units;
        this.General = general;
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        Cost = UnitFactory.CalculateCost(Units);
    }

    public void AddUnit(UnitType unit) {
        Units.Add((int)unit);
    }

    public void RemoveUnit(UnitType unit) {
        Units.Remove((int)unit);
    }

    public string GetDescription() {
        return Name;
    }

    public void ReplaceGeneral(int newGeneral){
        General = newGeneral;
    }
}
