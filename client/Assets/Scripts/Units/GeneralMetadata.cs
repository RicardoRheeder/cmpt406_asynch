using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneralMetadata {
    //Note: this class simply reuses the UnitType enum

    public enum GeneralAbility {
        TROJAN_SHOT = 0,
        ARMOUR_PIERCING_AMMO = 1,
        STEAM_OVERLOAD = 2,
        THE_BEST_OFFENSE = 3,
        STICK_AND_POKE = 4,
        DEEP_PENETRATION = 5,
        SAHARA_MINE = 6,
        SANDSTORM = 7
    };

    public enum GeneralPassive {
        HEAVY_ALBARN = 0,
        PIERCING_TUNGSTEN = 1,
        LIGHT_ADREN = 2,
        SUPPORT_SANDMAN = 3
    };

    public static readonly Dictionary<UnitType, List<GeneralAbility>> GeneralAbilityDictionary = new Dictionary<UnitType, List<GeneralAbility>>() {
        {UnitType.heavy_albarn, new List<GeneralAbility>{ GeneralAbility.STEAM_OVERLOAD, GeneralAbility.THE_BEST_OFFENSE } },
        {UnitType.piercing_tungsten, new List<GeneralAbility>{ GeneralAbility.TROJAN_SHOT, GeneralAbility.ARMOUR_PIERCING_AMMO } },
        {UnitType.light_adren, new List<GeneralAbility>{ GeneralAbility.STEAM_OVERLOAD, GeneralAbility.DEEP_PENETRATION } },
        {UnitType.support_sandman, new List<GeneralAbility>{ GeneralAbility.SAHARA_MINE, GeneralAbility.SANDSTORM } }
    };

    public static readonly Dictionary<GeneralAbility, int> AbilityCooldownDictionary = new Dictionary<GeneralAbility, int>() {
        { GeneralAbility.TROJAN_SHOT, 6 },
        { GeneralAbility.ARMOUR_PIERCING_AMMO, 6 },
        { GeneralAbility.STEAM_OVERLOAD, 3 },
        { GeneralAbility.THE_BEST_OFFENSE, 2 },
        { GeneralAbility.STICK_AND_POKE, 3 },
        { GeneralAbility.DEEP_PENETRATION, 3 },
        { GeneralAbility.SAHARA_MINE, 4 },
        { GeneralAbility.SANDSTORM, 5 }
    };

    public static readonly Dictionary<GeneralAbility, int> AbilityDurationDictionary = new Dictionary<GeneralAbility, int>() {
        { GeneralAbility.TROJAN_SHOT, 0 },
        { GeneralAbility.ARMOUR_PIERCING_AMMO, 4 },
        { GeneralAbility.STEAM_OVERLOAD, 0 },
        { GeneralAbility.THE_BEST_OFFENSE, 1 },
        { GeneralAbility.STICK_AND_POKE, 2 },
        { GeneralAbility.DEEP_PENETRATION, 0 },
        { GeneralAbility.SAHARA_MINE, 0 },
        { GeneralAbility.SANDSTORM, 3 }
    };

    //Note: to work with function pointers all of these functions have to take the same arguments, even if they don't require them all
    private static void TrojanShot(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits) {
        source.attackStrategy = new LineStrategy();
        source.Range = 100000;
    }

    private static void ArmourPiercingAmmo(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits) {
        source.Pierce += 10;
    }


    private static void SteamOverload(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits) {
        if (source.TakeDamage(30, 10000)) {
            allUnits.Remove(source.Position);
        }
        List<Vector2Int> unitsInRange = HexUtility.GetTilePositionsInRangeWithoutMapWithoutStarting(source.Position, 2);
        for(int i = 0; i < 6; i++) {
            Vector2Int unitPos = unitsInRange[i];
            if(allUnits.ContainsKey(unitPos)) {
                if (allUnits[unitPos].TakeDamage(20, 10000)) {
                    allUnits.Remove(unitPos);
                }
            }
        }
        for(int i = 6; i < 18; i++) {
            Vector2Int unitPos = unitsInRange[i];
            if (allUnits.ContainsKey(unitPos)) {
                if (allUnits[unitPos].TakeDamage(10, 10000)) {
                    allUnits.Remove(unitPos);
                }
            }
        }
    }

    private static void TheBestOffense(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits) {
        source.Damage += source.Armour;
        if(source.AttackActions < 1) {
            source.AttackActions = 1;
        }
    }

    private static void StickAndPoke(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits) {
        foreach(var unit in allUnits.Values) {
            if(unit.UnitClass == UnitClass.light) {
                unit.MovementActions += 1;
                unit.MovementSpeed += 3;
            }
        }
    }

    private static void DeepPenetration(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits) {
        source.Armour = 0;
    }

    private static void SaharaMine(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits) {
        //TODO
    }

    private static void Sandstorm(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits) {
        //TODO
    }


    public delegate void RefAction<T1, T2>(ref T1 arg1, T2 arg2); //required to make the delegate functions work with a reference parameter
    public static readonly Dictionary<GeneralAbility, RefAction<UnitStats, Dictionary<Vector2Int, UnitStats>>> ActiveAbilityFunctionDictionary = new Dictionary<GeneralAbility, RefAction<UnitStats, Dictionary<Vector2Int, UnitStats>>>() {
        {GeneralAbility.TROJAN_SHOT, TrojanShot },
        {GeneralAbility.ARMOUR_PIERCING_AMMO, ArmourPiercingAmmo },
        {GeneralAbility.STEAM_OVERLOAD, SteamOverload },
        {GeneralAbility.THE_BEST_OFFENSE, TheBestOffense },
        {GeneralAbility.STICK_AND_POKE, StickAndPoke },
        {GeneralAbility.DEEP_PENETRATION, DeepPenetration },
        {GeneralAbility.SAHARA_MINE, SaharaMine },
        {GeneralAbility.SANDSTORM, Sandstorm }
    };

    public static readonly Dictionary<UnitType, GeneralPassive> GeneralPassiveDictionary = new Dictionary<UnitType, GeneralPassive>() {
        {UnitType.heavy_albarn, GeneralPassive.HEAVY_ALBARN },
        {UnitType.piercing_tungsten, GeneralPassive.PIERCING_TUNGSTEN },
        {UnitType.light_adren, GeneralPassive.LIGHT_ADREN },
        {UnitType.support_sandman, GeneralPassive.SUPPORT_SANDMAN }
    };

    //TODO, finish implementing passives
}
