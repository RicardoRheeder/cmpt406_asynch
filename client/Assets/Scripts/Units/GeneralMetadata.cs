using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CardsAndCarnage;

public enum GeneralAbility {
    NONE = -1,
    TROJAN_SHOT = 1,
    ARMOUR_PIERCING_AMMO = 2,
    STEAM_OVERLOAD = 3,
    THE_BEST_OFFENSE = 4,
    STICK_AND_POKE = 5,
    DEEP_PENETRATION = 6,
    SAHARA_MINE = 7,
    SANDSTORM = 8
};

public enum GeneralPassive {
    HEAVY_ALBARN = 0,
    PIERCING_TUNGSTEN = 1,
    LIGHT_ADREN = 2,
    SUPPORT_SANDMAN = 3
};

public static class GeneralMetadata {

    public static readonly Dictionary<UnitType, List<GeneralAbility>> GeneralAbilityDictionary = new Dictionary<UnitType, List<GeneralAbility>>() {
        {UnitType.heavy_albarn, new List<GeneralAbility>{ GeneralAbility.STEAM_OVERLOAD, GeneralAbility.THE_BEST_OFFENSE } },
        {UnitType.piercing_tungsten, new List<GeneralAbility>{ GeneralAbility.TROJAN_SHOT, GeneralAbility.ARMOUR_PIERCING_AMMO } },
        {UnitType.light_adren, new List<GeneralAbility>{ GeneralAbility.STICK_AND_POKE, GeneralAbility.DEEP_PENETRATION } },
        {UnitType.support_sandman, new List<GeneralAbility>{ GeneralAbility.SAHARA_MINE, GeneralAbility.SANDSTORM } }
    };

    public static readonly Dictionary<GeneralAbility, String> ReadableAbilityNameDict = new Dictionary<GeneralAbility, string>() {
        {GeneralAbility.TROJAN_SHOT, "Trojan Shot" },
        {GeneralAbility.ARMOUR_PIERCING_AMMO, "Armour Piercing Ammo" },
        {GeneralAbility.STEAM_OVERLOAD, "Steam Overload" },
        {GeneralAbility.THE_BEST_OFFENSE, "The Best Offense" },
        {GeneralAbility.STICK_AND_POKE, "Stick and Poke" },
        {GeneralAbility.DEEP_PENETRATION, "Deep Penetration" },
        {GeneralAbility.SAHARA_MINE, "Sahara Mine" },
        {GeneralAbility.SANDSTORM, "Sandstorm" }
    };

    public static readonly Dictionary<GeneralAbility, int> AbilityCooldownDictionary = new Dictionary<GeneralAbility, int>() {
        { GeneralAbility.TROJAN_SHOT, 6 },
        { GeneralAbility.ARMOUR_PIERCING_AMMO, 6 },
        { GeneralAbility.STEAM_OVERLOAD, 3 },
        { GeneralAbility.THE_BEST_OFFENSE, 2 },
        { GeneralAbility.STICK_AND_POKE, 3 },
        { GeneralAbility.DEEP_PENETRATION, 3 },
        { GeneralAbility.SAHARA_MINE, 4 },
        { GeneralAbility.SANDSTORM, 4 }
    };

    public static readonly Dictionary<GeneralAbility, int> AbilityDurationDictionary = new Dictionary<GeneralAbility, int>() {
        { GeneralAbility.TROJAN_SHOT, 0 },
        { GeneralAbility.ARMOUR_PIERCING_AMMO, 4 },
        { GeneralAbility.STEAM_OVERLOAD, 0 },
        { GeneralAbility.THE_BEST_OFFENSE, 1 },
        { GeneralAbility.STICK_AND_POKE, 2 },
        { GeneralAbility.DEEP_PENETRATION, 0 },
        { GeneralAbility.SAHARA_MINE, 0 },
        { GeneralAbility.SANDSTORM, 2 }
    };

    public static readonly Dictionary<GeneralAbility, int> AbilityRangeDictionary = new Dictionary<GeneralAbility, int>() {
        { GeneralAbility.TROJAN_SHOT, 0 },
        { GeneralAbility.ARMOUR_PIERCING_AMMO, 10 },
        { GeneralAbility.STEAM_OVERLOAD, 10 },
        { GeneralAbility.THE_BEST_OFFENSE, 10 },
        { GeneralAbility.STICK_AND_POKE, 0 },
        { GeneralAbility.DEEP_PENETRATION, 10 },
        { GeneralAbility.SAHARA_MINE, 2 },
        { GeneralAbility.SANDSTORM, 0 }
    };

    //Note: to work with function pointers all of these functions have to take the same arguments, even if they don't require them all
    private static void TrojanShot(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        source.attackStrategy = new LineStrategy();
        source.AlterRange(20);
    }

    private static void ArmourPiercingAmmo(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        source.AlterPierce(10);
    }

    private static void SteamOverload(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (source.TakeDamage(30, 10000)) {
            source.Kill();
            allUnits.Remove(source.Position);
        }
        List<Vector2Int> unitsInRange = HexUtility.GetTilePositionsInRangeWithoutMapWithoutStarting(source.Position, 2);
        for(int i = 0; i < 6; i++) {
            Vector2Int unitPos = unitsInRange[i];
            if(allUnits.ContainsKey(unitPos)) {
                UnitStats targetUnit = allUnits[unitPos];
                if (targetUnit.TakeDamage(20, 10000)) {
                    targetUnit.Kill();
                    allUnits.Remove(unitPos);
                }
            }
        }
        for(int i = 6; i < 18; i++) {
            Vector2Int unitPos = unitsInRange[i];
            if (allUnits.ContainsKey(unitPos)) {
                UnitStats targetUnit = allUnits[unitPos];
                if (targetUnit.TakeDamage(10, 10000)) {
                    targetUnit.Kill();
                    allUnits.Remove(unitPos);
                }
            }
        }
    }

    private static void TheBestOffense(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        source.AlterDamage(source.Armour);
    }

    private static void StickAndPoke(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        foreach(var key in allUnits.Keys) {
            UnitStats unit = allUnits[key];
            if(unit.UnitClass == UnitClass.light && unit.Owner == username) {
                unit.MovementActions += 1;
                unit.AlterSpeed(3);
            }
        }
    }

    private static void DeepPenetration(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        source.Armour = 0;
    }

    private static void SaharaMine(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        List<Vector2Int> unitsInRange = HexUtility.GetTilePositionsInRangeWithoutMap(source.Position, 1);
        foreach(Vector2Int pos in unitsInRange) {
            if (allUnits.TryGetValue(pos, out UnitStats target)) {
                target.AlterArmour(-10);
                if (target.TakeDamage(10, 10000)) {
                    target.Kill();
                    allUnits.Remove(pos);
                }
            }
        }
    }

    private static void Sandstorm(ref UnitStats source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        List<Vector2Int> unitsInRange = HexUtility.GetTilePositionsInRangeWithoutMap(source.Position, 2);
        foreach (Vector2Int pos in unitsInRange) {
            if (allUnits.TryGetValue(pos, out UnitStats target)) {
                target.AlterVision(-3);
                target.AlterSpeed(-3);
            }
        }
    }

    public static readonly Dictionary<GeneralAbility, AbilityAction<UnitStats, Dictionary<Vector2Int, UnitStats>, string>> ActiveAbilityFunctionDictionary = new Dictionary<GeneralAbility, AbilityAction<UnitStats, Dictionary<Vector2Int, UnitStats>, string>>() {
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

    public static readonly Dictionary<GeneralPassive, PassiveAction<Dictionary<Vector2Int, UnitStats>, string>> PassiveEffectsDictionary = new Dictionary<GeneralPassive, PassiveAction<Dictionary<Vector2Int, UnitStats>, string>>() {
        {GeneralPassive.HEAVY_ALBARN, AlbarnPassive },
        {GeneralPassive.PIERCING_TUNGSTEN, TungstenPassive },
        {GeneralPassive.LIGHT_ADREN, AdrenPassive },
        {GeneralPassive.SUPPORT_SANDMAN, SandmanPassive }
    };

    private static void AlbarnPassive(Dictionary<Vector2Int, UnitStats> unitPositions, string username) {
        foreach (var key in unitPositions.Keys) {
            UnitStats unit = unitPositions[key];
            if (unit.Owner == username) {
                unit.AlterArmour(5);
            }
        }
    }

    private static void TungstenPassive(Dictionary<Vector2Int, UnitStats> unitPositions, string username) {
        foreach (var key in unitPositions.Keys) {
            UnitStats unit = unitPositions[key];
            if (unit.Owner == username) {
                unit.AlterVision(2);
            }
        }
    }

    private static void AdrenPassive(Dictionary<Vector2Int, UnitStats> unitPositions, string username) {
        foreach (var key in unitPositions.Keys) {
            UnitStats unit = unitPositions[key];
            if (unit.Owner == username) {
                unit.AlterSpeed(2);
            }
        }
    }

    private static void SandmanPassive(Dictionary<Vector2Int, UnitStats> unitPositions, string username) {
        foreach (var key in unitPositions.Keys) {
            UnitStats unit = unitPositions[key];
            switch (unit.UnitClass) {
                case UnitClass.heavy:
                    unit.AlterArmour(-5);
                    break;
                case UnitClass.piercing:
                    unit.AlterVision(-1);
                    break;
                case UnitClass.light:
                    unit.AlterSpeed(-1);
                    break;
                case UnitClass.support:
                    unit.AlterMoveAfterAttack();
                    break;
            }
        }
    }
}
