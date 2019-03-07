﻿//Helper class to get base units
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
    private readonly static int TROOPER_VISION = 5;
    private readonly static int TROOPER_COST = 1;
    private readonly static IAttackStrategy TROOPER_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int REACON_HP = 85;
    private readonly static int REACON_ARMOUR = 0;
    private readonly static int REACON_RANGE = 3;
    private readonly static int REACON_DAMAGE = 15;
    private readonly static int REACON_PIERCE = 0;
    private readonly static int REACON_AOE = 0;
    private readonly static int REACON_SPEED = 9;
    private readonly static int REACON_VISION = 5;
    private readonly static int REACON_COST = 1;
    private readonly static IAttackStrategy REACON_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int STEAMER_HP = 120;
    private readonly static int STEAMER_ARMOUR = 50;
    private readonly static int STEAMER_RANGE = 1;
    private readonly static int STEAMER_DAMAGE = 30;
    private readonly static int STEAMER_PIERCE = 0;
    private readonly static int STEAMER_AOE = 0;
    private readonly static int STEAMER_SPEED = 6;
    private readonly static int STEAMER_VISION = 5;
    private readonly static int STEAMER_COST = 2;
    private readonly static IAttackStrategy STEAMER_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int PEWPEW_HP = 150;
    private readonly static int PEWPEW_ARMOUR = 20;
    private readonly static int PEWPEW_RANGE = 3;
    private readonly static int PEWPEW_DAMAGE = 20;
    private readonly static int PEWPEW_PIERCE = 0;
    private readonly static int PEWPEW_AOE = 0;
    private readonly static int PEWPEW_SPEED = 5;
    private readonly static int PEWPEW_VISION = 5;
    private readonly static int PEWPEW_COST = 1;
    private readonly static IAttackStrategy PEWPEW_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int COMPENSATOR_HP = 90;
    private readonly static int COMPENSATOR_ARMOUR = 0;
    private readonly static int COMPENSATOR_RANGE = 3;
    private readonly static int COMPENSATOR_DAMAGE = 35;
    private readonly static int COMPENSATOR_PIERCE = 20;
    private readonly static int COMPENSATOR_AOE = 0;
    private readonly static int COMPENSATOR_SPEED = 7;
    private readonly static int COMPENSATOR_VISION = 5;
    private readonly static int COMPENSATOR_COST = 3;
    private readonly static IAttackStrategy COMPENSATOR_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int FOUNDATION_HP = 75;
    private readonly static int FOUNDATION_ARMOUR = 10;
    private readonly static int FOUNDATION_RANGE = 2;
    private readonly static int FOUNDATION_DAMAGE = 25;
    private readonly static int FOUNDATION_PIERCE = 15;
    private readonly static int FOUNDATION_AOE = 0;
    private readonly static int FOUNDATION_SPEED = 6;
    private readonly static int FOUNDATION_VISION = 5;
    private readonly static int FOUNDATION_COST = 2;
    private readonly static IAttackStrategy FOUNDATION_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int POWER_SURGE_HP = 80;
    private readonly static int POWER_SURGE_ARMOUR = 10;
    private readonly static int POWER_SURGE_RANGE = 2;
    private readonly static int POWER_SURGE_DAMAGE = 15;
    private readonly static int POWER_SURGE_PIERCE = 0;
    private readonly static int POWER_SURGE_AOE = 0;
    private readonly static int POWER_SURGE_SPEED = 6;
    private readonly static int POWER_SURGE_VISION = 5;
    private readonly static int POWER_SURGE_COST = 1;
    private readonly static IAttackStrategy POWER_SURGE_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int MIDAS_HP = 80;
    private readonly static int MIDAS_ARMOUR = 20;
    private readonly static int MIDAS_RANGE = 3;
    private readonly static int MIDAS_DAMAGE = -20;
    private readonly static int MIDAS_PIERCE = 0;
    private readonly static int MIDAS_AOE = 0;
    private readonly static int MIDAS_SPEED = 4;
    private readonly static int MIDAS_VISION = 5;
    private readonly static int MIDAS_COST = 2;
    private readonly static IAttackStrategy MIDAS_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int CLAYMORE_HP = 50;
    private readonly static int CLAYMORE_ARMOUR = 0;
    private readonly static int CLAYMORE_RANGE = 1;
    private readonly static int CLAYMORE_DAMAGE = 30;
    private readonly static int CLAYMORE_PIERCE = 0;
    private readonly static int CLAYMORE_AOE = 0;
    private readonly static int CLAYMORE_SPEED = 9;
    private readonly static int CLAYMORE_VISION = 5;
    private readonly static int CLAYMORE_COST = 2;
    private readonly static IAttackStrategy CLAYMORE_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int HEAVY_ALBARN_HP = 200;
    private readonly static int HEAVY_ALBARN_ARMOUR = 30;
    private readonly static int HEAVY_ALBARN_RANGE = 10;
    private readonly static int HEAVY_ALBARN_DAMAGE = 25;
    private readonly static int HEAVY_ALBARN_PIERCE = 0;
    private readonly static int HEAVY_ALBARN_AOE = 0;
    private readonly static int HEAVY_ALBARN_SPEED = 0;
    private readonly static int HEAVY_ALBARN_VISION = 5;
    private readonly static int HEAVY_ALBARN_COST = 0;
    private readonly static IAttackStrategy HEAVY_ALBARN_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int PIERCING_TUNGSTEN_HP = 200;
    private readonly static int PIERCING_TUNGSTEN_ARMOUR = 30;
    private readonly static int PIERCING_TUNGSTEN_RANGE = 10;
    private readonly static int PIERCING_TUNGSTEN_DAMAGE = 25;
    private readonly static int PIERCING_TUNGSTEN_PIERCE = 0;
    private readonly static int PIERCING_TUNGSTEN_AOE = 0;
    private readonly static int PIERCING_TUNGSTEN_SPEED = 0;
    private readonly static int PIERCING_TUNGSTEN_VISION = 5;
    private readonly static int PIERCING_TUNGSTEN_COST = 0;
    private readonly static IAttackStrategy PIERCING_TUNGSTEN_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int LIGHT_ADREN_HP = 200;
    private readonly static int LIGHT_ADREN_ARMOUR = 30;
    private readonly static int LIGHT_ADREN_RANGE = 10;
    private readonly static int LIGHT_ADREN_DAMAGE = 25;
    private readonly static int LIGHT_ADREN_PIERCE = 0;
    private readonly static int LIGHT_ADREN_AOE = 0;
    private readonly static int LIGHT_ADREN_SPEED = 0;
    private readonly static int LIGHT_ADREN_VISION = 5;
    private readonly static int LIGHT_ADREN_COST = 0;
    private readonly static IAttackStrategy LIGHT_ADREN_ATTACK_STRATEGY = new AreaStrategy();

    private readonly static int SUPPORT_SANDMAN_HP = 200;
    private readonly static int SUPPORT_SANDMAN_ARMOUR = 30;
    private readonly static int SUPPORT_SANDMAN_RANGE = 10;
    private readonly static int SUPPORT_SANDMAN_DAMAGE = 25;
    private readonly static int SUPPORT_SANDMAN_PIERCE = 0;
    private readonly static int SUPPORT_SANDMAN_AOE = 0;
    private readonly static int SUPPORT_SANDMAN_SPEED = 0;
    private readonly static int SUPPORT_SANDMAN_VISION = 5;
    private readonly static int SUPPORT_SANDMAN_COST = 0;
    private readonly static IAttackStrategy SUPPORT_SANDMAN_ATTACK_STRATEGY = new AreaStrategy();

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
            TROOPER_VISION,
            TROOPER_COST,
            TROOPER_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateReacon() {
        return new UnitStats(
            UnitType.recon,
            REACON_HP,
            REACON_ARMOUR,
            REACON_RANGE,
            REACON_DAMAGE,
            REACON_PIERCE,
            REACON_AOE,
            REACON_SPEED,
            REACON_VISION,
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
            STEAMER_VISION,
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
            PEWPEW_VISION,
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
            COMPENSATOR_VISION,
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
            FOUNDATION_VISION,
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
            POWER_SURGE_VISION,
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
            MIDAS_VISION,
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
            CLAYMORE_VISION,
            CLAYMORE_COST,
            CLAYMORE_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateHeavyAlbarn() {
        return new UnitStats(
            UnitType.heavy_albarn,
            HEAVY_ALBARN_HP,
            HEAVY_ALBARN_ARMOUR,
            HEAVY_ALBARN_RANGE,
            HEAVY_ALBARN_DAMAGE,
            HEAVY_ALBARN_PIERCE,
            HEAVY_ALBARN_AOE,
            HEAVY_ALBARN_SPEED,
            HEAVY_ALBARN_VISION,
            HEAVY_ALBARN_COST,
            HEAVY_ALBARN_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreatePiercingTungsten() {
        return new UnitStats(
            UnitType.piercing_tungsten,
            PIERCING_TUNGSTEN_HP,
            PIERCING_TUNGSTEN_ARMOUR,
            PIERCING_TUNGSTEN_RANGE,
            PIERCING_TUNGSTEN_DAMAGE,
            PIERCING_TUNGSTEN_PIERCE,
            PIERCING_TUNGSTEN_AOE,
            PIERCING_TUNGSTEN_SPEED,
            PIERCING_TUNGSTEN_VISION,
            PIERCING_TUNGSTEN_COST,
            PIERCING_TUNGSTEN_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateLightAdren() {
        return new UnitStats(
            UnitType.light_adren,
            LIGHT_ADREN_HP,
            LIGHT_ADREN_ARMOUR,
            LIGHT_ADREN_RANGE,
            LIGHT_ADREN_DAMAGE,
            LIGHT_ADREN_PIERCE,
            LIGHT_ADREN_AOE,
            LIGHT_ADREN_SPEED,
            LIGHT_ADREN_VISION,
            LIGHT_ADREN_COST,
            LIGHT_ADREN_ATTACK_STRATEGY
        );
    }

    private static UnitStats CreateSupportSandman() {
        return new UnitStats(
            UnitType.support_sandman,
            SUPPORT_SANDMAN_HP,
            SUPPORT_SANDMAN_ARMOUR,
            SUPPORT_SANDMAN_RANGE,
            SUPPORT_SANDMAN_DAMAGE,
            SUPPORT_SANDMAN_PIERCE,
            SUPPORT_SANDMAN_AOE,
            SUPPORT_SANDMAN_SPEED,
            SUPPORT_SANDMAN_VISION,
            SUPPORT_SANDMAN_COST,
            SUPPORT_SANDMAN_ATTACK_STRATEGY
        );
    }

    private static readonly Dictionary<UnitType, Func<UnitStats>> UnitCreationMethods = new Dictionary<UnitType, Func<UnitStats>>() {
        {UnitType.trooper, CreateTrooper},
        {UnitType.recon, CreateReacon},
        {UnitType.steamer, CreateSteamer},
        {UnitType.pewpew, CreatePewPew},
        {UnitType.compensator, CreateCompensator},
        {UnitType.foundation, CreateFoundation},
        {UnitType.powerSurge, CreatePowerSurge},
        {UnitType.midas, CreateMidas},
        {UnitType.claymore, CreateClaymore},

        //Generals
        {UnitType.heavy_albarn, CreateHeavyAlbarn},
        {UnitType.piercing_tungsten, CreatePiercingTungsten},
        {UnitType.light_adren, CreateLightAdren},
        {UnitType.support_sandman, CreateSupportSandman},
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