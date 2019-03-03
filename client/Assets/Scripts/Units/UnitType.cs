//Enum to keep track of unit types, used for server communication
public enum UnitType {
    tile = -1,
    trooper = 0,
    recon = 1,
    steamer = 2,
    pewpew = 3,
    compensator = 4,
    foundation = 5,
    powerSurge = 6,
    midas = 7,
    claymore = 8,

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
