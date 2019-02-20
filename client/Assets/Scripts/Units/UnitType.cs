﻿//Enum to keep track of unit types, used for server communication
public enum UnitType {
    tile = -1,
    trooper = 0,
    reacon = 1,
    steamer = 2,
    pewpew = 3,
    compensator = 4,
    foundation = 5,
    powerSurge = 6,
    midas = 7,
    claymore = 8,

    //All generals are above 100s
}

public enum UnitClass {
    heavy = 0,
    light = 1,
    piercing = 2,
    support = 3,
    general = 100,
}
