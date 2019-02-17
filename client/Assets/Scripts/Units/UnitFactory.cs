//Helper class to get base units
using System;
using System.Collections.Generic;

public static class UnitFactory {

    private static UnitStats CreateTrooper() {
        return new UnitStats(UnitType.trooper, 20, 0, 2, 3, 0, 0, 2, 1, new AreaStrategy());
    }

    private static UnitStats CreateReacon() {
        return new UnitStats(UnitType.reacon, 20, 0, 3, 2, 0, 0, 5, 1, new AreaStrategy());
    }

    private static UnitStats CreateSteamer() {
        return new UnitStats(UnitType.steamer, 25, 4, 1, 3, 0, 0, 3, 1, new AreaStrategy());
    }

    private static UnitStats CreatePewPew() {
        return new UnitStats(UnitType.pewpew, 35, 2, 3, 2, 0, 0, 2, 1, new AreaStrategy());
    }

    private static UnitStats CreateCompensator() {
        return new UnitStats(UnitType.compensator, 15, 0, 3, 4, 3, 0, 3, 1, new AreaStrategy());
    }

    private static UnitStats CreateFoundation() {
        return new UnitStats(UnitType.foundation, 20, 0, 2, 3, 3, 0, 2, 1, new AreaStrategy());
    }

    private static UnitStats CreatePowerSurge() {
        return new UnitStats(UnitType.powerSurge, 15, 0, 2, 2, 0, 0, 3, 1, new AreaStrategy());
    }

    private static UnitStats CreateMidas() {
        return new UnitStats(UnitType.midas, 15, 0, 3, -3, 0, 0, 3, 1, new AreaStrategy());
    }

    private static UnitStats CreateClaymore() {
        return new UnitStats(UnitType.claymore, 10, 0, 1, 4, 0, 0, 5, 1, new AreaStrategy());
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
        {UnitType.claymore, CreateClaymore}
    };

    public static UnitStats GetBaseUnit(UnitType type) {
        return UnitCreationMethods[type]();
    }
}
