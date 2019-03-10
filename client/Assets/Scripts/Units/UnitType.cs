//Enum to keep track of unit types, used for server communication
public enum UnitType {
    none = -2,
    tile = -1,
    trooper = 1,
    recon = 2,
    steamer = 3,
    pewpew = 4,
    compensator = 5,
    foundation = 6,
    powerSurge = 7,
    midas = 8,
    claymore = 9,

    //All generals are above 100s
    heavy_albarn = 101,
    piercing_tungsten = 102,
    light_adren = 103,
    support_sandman = 104
}

public enum UnitClass {
    heavy = 0,
    light = 1,
    piercing = 2,
    support = 3,
    general = 100,
}
