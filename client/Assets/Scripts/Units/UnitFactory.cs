//Helper class to get base units
using System;
using System.Collections.Generic;

public static class UnitFactory {

    private readonly static int TROOPER_HP = 100;
    private readonly static int TROOPER_ARMOUR = 10;
    private readonly static int TROOPER_RANGE = 2;
    private readonly static int TROOPER_DAMAGE = 25;
    private readonly static int TROOPER_PIERCE = 0;
    private readonly static int TROOPER_AOE = 0;
    private readonly static int TROOPER_SPEED = 6;
    private readonly static int TROOPER_COST = 1;
    private readonly static IAttackStrategy TROOPER_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int REACON_HP = 85;
    private readonly static int REACON_ARMOUR = 0;
    private readonly static int REACON_RANGE = 3;
    private readonly static int REACON_DAMAGE = 15;
    private readonly static int REACON_PIERCE = 0;
    private readonly static int REACON_AOE = 0;
    private readonly static int REACON_SPEED = 9;
    private readonly static int REACON_COST = 1;
    private readonly static IAttackStrategy REACON_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int STEAMER_HP = 120;
    private readonly static int STEAMER_ARMOUR = 50;
    private readonly static int STEAMER_RANGE = 1;
    private readonly static int STEAMER_DAMAGE = 30;
    private readonly static int STEAMER_PIERCE = 0;
    private readonly static int STEAMER_AOE = 0;
    private readonly static int STEAMER_SPEED = 6;
    private readonly static int STEAMER_COST = 2;
    private readonly static IAttackStrategy STEAMER_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int PEWPEW_HP = 150;
    private readonly static int PEWPEW_ARMOUR = 20;
    private readonly static int PEWPEW_RANGE = 3;
    private readonly static int PEWPEW_DAMAGE = 20;
    private readonly static int PEWPEW_PIERCE = 0;
    private readonly static int PEWPEW_AOE = 0;
    private readonly static int PEWPEW_SPEED = 5;
    private readonly static int PEWPEW_COST = 1;
    private readonly static IAttackStrategy PEWPEW_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int COMPENSATOR_HP = 90;
    private readonly static int COMPENSATOR_ARMOUR = 0;
    private readonly static int COMPENSATOR_RANGE = 3;
    private readonly static int COMPENSATOR_DAMAGE = 35;
    private readonly static int COMPENSATOR_PIERCE = 20;
    private readonly static int COMPENSATOR_AOE = 0;
    private readonly static int COMPENSATOR_SPEED = 7;
    private readonly static int COMPENSATOR_COST = 3;
    private readonly static IAttackStrategy COMPENSATOR_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int FOUNDATION_HP = 75;
    private readonly static int FOUNDATION_ARMOUR = 10;
    private readonly static int FOUNDATION_RANGE = 2;
    private readonly static int FOUNDATION_DAMAGE = 25;
    private readonly static int FOUNDATION_PIERCE = 15;
    private readonly static int FOUNDATION_AOE = 0;
    private readonly static int FOUNDATION_SPEED = 6;
    private readonly static int FOUNDATION_COST = 2;
    private readonly static IAttackStrategy FOUNDATION_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int POWER_SURGE_HP = 80;
    private readonly static int POWER_SURGE_ARMOUR = 10;
    private readonly static int POWER_SURGE_RANGE = 2;
    private readonly static int POWER_SURGE_DAMAGE = 15;
    private readonly static int POWER_SURGE_PIERCE = 0;
    private readonly static int POWER_SURGE_AOE = 0;
    private readonly static int POWER_SURGE_SPEED = 6;
    private readonly static int POWER_SURGE_COST = 1;
    private readonly static IAttackStrategy POWER_SURGE_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int MIDAS_HP = 80;
    private readonly static int MIDAS_ARMOUR = 20;
    private readonly static int MIDAS_RANGE = 3;
    private readonly static int MIDAS_DAMAGE = -20;
    private readonly static int MIDAS_PIERCE = 0;
    private readonly static int MIDAS_AOE = 0;
    private readonly static int MIDAS_SPEED = 4;
    private readonly static int MIDAS_COST = 2;
    private readonly static IAttackStrategy MIDAS_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int CLAYMORE_HP = 50;
    private readonly static int CLAYMORE_ARMOUR = 0;
    private readonly static int CLAYMORE_RANGE = 1;
    private readonly static int CLAYMORE_DAMAGE = 30;
    private readonly static int CLAYMORE_PIERCE = 0;
    private readonly static int CLAYMORE_AOE = 0;
    private readonly static int CLAYMORE_SPEED = 9;
    private readonly static int CLAYMORE_COST = 2;
    private readonly static IAttackStrategy CLAYMORE_ATTACK_STRATEGY = new AreaStrategy();

    private static UnitStats CreateTrooper() {
        return new UnitStats(
            UnitType.trooper,
            TROOPER_HP,
            TROOPER_ARMOUR,
            TROOPER_RANGE,
            TROOPER_DAMAGE,
            TROOPER_PIERCE,
            TROOPER_AOE,
            TROOPER_SPEED,
            TROOPER_COST,
            TROOPER_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateReacon() {
        return new UnitStats(
            UnitType.reacon,
            REACON_HP,
            REACON_ARMOUR,
            REACON_RANGE,
            REACON_DAMAGE,
            REACON_PIERCE,
            REACON_AOE,
            REACON_SPEED,
            REACON_COST,
            REACON_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateSteamer() {
        return new UnitStats(
            UnitType.steamer,
            STEAMER_HP,
            STEAMER_ARMOUR,
            STEAMER_RANGE,
            STEAMER_DAMAGE,
            STEAMER_PIERCE,
            STEAMER_AOE,
            STEAMER_SPEED,
            STEAMER_COST,
            STEAMER_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreatePewPew() {
        return new UnitStats(
            UnitType.pewpew,
            PEWPEW_HP,
            PEWPEW_ARMOUR,
            PEWPEW_RANGE,
            PEWPEW_DAMAGE,
            PEWPEW_PIERCE,
            PEWPEW_AOE,
            PEWPEW_SPEED,
            PEWPEW_COST,
            PEWPEW_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateCompensator() {
        return new UnitStats(
            UnitType.compensator,
            COMPENSATOR_HP,
            COMPENSATOR_ARMOUR,
            COMPENSATOR_RANGE,
            COMPENSATOR_DAMAGE,
            COMPENSATOR_PIERCE,
            COMPENSATOR_AOE,
            COMPENSATOR_SPEED,
            COMPENSATOR_COST,
            COMPENSATOR_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateFoundation() {
        return new UnitStats(
            UnitType.foundation,
            FOUNDATION_HP,
            FOUNDATION_ARMOUR,
            FOUNDATION_RANGE,
            FOUNDATION_DAMAGE,
            FOUNDATION_PIERCE,
            FOUNDATION_AOE,
            FOUNDATION_SPEED,
            FOUNDATION_COST,
            FOUNDATION_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreatePowerSurge() {
        return new UnitStats(
            UnitType.powerSurge,
            POWER_SURGE_HP,
            POWER_SURGE_ARMOUR,
            POWER_SURGE_RANGE,
            POWER_SURGE_DAMAGE,
            POWER_SURGE_PIERCE,
            POWER_SURGE_AOE,
            POWER_SURGE_SPEED,
            POWER_SURGE_COST,
            POWER_SURGE_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateMidas() {
        return new UnitStats(
            UnitType.midas,
            MIDAS_HP,
            MIDAS_ARMOUR,
            MIDAS_RANGE,
            MIDAS_DAMAGE,
            MIDAS_PIERCE,
            MIDAS_AOE,
            MIDAS_SPEED,
            MIDAS_COST,
            MIDAS_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateClaymore() {
        return new UnitStats(
            UnitType.claymore,
            CLAYMORE_HP,
            CLAYMORE_ARMOUR,
            CLAYMORE_RANGE,
            CLAYMORE_DAMAGE,
            CLAYMORE_PIERCE,
            CLAYMORE_AOE,
            CLAYMORE_SPEED,
            CLAYMORE_COST,
            CLAYMORE_ATTACK_STRATEGY
        );
    }


    private readonly static int General1_HP = 50;
    private readonly static int General1_ARMOUR = 0;
    private readonly static int General1_RANGE = 1;
    private readonly static int General1_DAMAGE = 30;
    private readonly static int General1_PIERCE = 0;
    private readonly static int General1_AOE = 0;
    private readonly static int General1_SPEED = 9;
    private readonly static int General1_COST = 2;
    private readonly static IAttackStrategy General1_ATTACK_STRATEGY = new AreaStrategy();

    private static UnitStats CreateGeneral1() {
        return new UnitStats(
            UnitType.general1,
            General1_HP,
            General1_ARMOUR,
            General1_RANGE,
            General1_DAMAGE,
            General1_PIERCE,
            General1_AOE,
            General1_SPEED,
            General1_COST,
            General1_ATTACK_STRATEGY
        );
    }

    private static Dictionary<UnitType, Func<UnitStats>> UnitCreationMethods = new Dictionary<UnitType, Func<UnitStats>>() {
        {UnitType.trooper, CreateTrooper},
        {UnitType.reacon, CreateReacon},
        {UnitType.steamer, CreateSteamer},
        {UnitType.pewpew, CreatePewPew},
        {UnitType.compensator, CreateCompensator},
        {UnitType.foundation, CreateFoundation},
        {UnitType.powerSurge, CreatePowerSurge},
        {UnitType.midas, CreateMidas},
        {UnitType.claymore, CreateClaymore},
        {UnitType.general1, CreateGeneral1}
    };

    public static UnitStats GetBaseUnit(UnitType type) {
        return UnitCreationMethods[type]();
    }
    
    public static int GetCost(List<int> units) {
        int cost = 0;
        foreach (int i in units) {
            cost += GetBaseUnit((UnitType)i).Cost;
        }
        return cost;
    }
}
