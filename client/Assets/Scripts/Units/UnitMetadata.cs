using System.Collections.Generic;

//Helper class to get a damage multiplier
public static class UnitMetadata {

    private readonly static float LIGHT_STRENGTH = 1.5f;
    private readonly static float HEAVY_STRENGHTH = 1.5f;
    private readonly static float PIERCING_STRENGTH = 1.5f;

    //Update this with any new pairings that come up.
    //The floating point value represents the multiplier, we might want to change this in the future.
    private static Dictionary<Tuple<UnitType, UnitType>, float> Strengths = new Dictionary<Tuple<UnitType, UnitType>, float>() {
        //Light unit strengths
        {new Tuple<UnitType, UnitType>(UnitType.trooper, UnitType.compensator), LIGHT_STRENGTH },
        {new Tuple<UnitType, UnitType>(UnitType.trooper, UnitType.foundation), LIGHT_STRENGTH },
        {new Tuple<UnitType, UnitType>(UnitType.reacon, UnitType.compensator), LIGHT_STRENGTH },
        {new Tuple<UnitType, UnitType>(UnitType.reacon, UnitType.foundation), LIGHT_STRENGTH },

        //Heavy unit strengths
        {new Tuple<UnitType, UnitType>(UnitType.steamer, UnitType.trooper), HEAVY_STRENGHTH },
        {new Tuple<UnitType, UnitType>(UnitType.steamer, UnitType.reacon), HEAVY_STRENGHTH },
        {new Tuple<UnitType, UnitType>(UnitType.pewpew, UnitType.trooper), HEAVY_STRENGHTH },
        {new Tuple<UnitType, UnitType>(UnitType.pewpew, UnitType.reacon), HEAVY_STRENGHTH },

        //Piercing unit strengths
        {new Tuple<UnitType, UnitType>(UnitType.compensator, UnitType.steamer), PIERCING_STRENGTH },
        {new Tuple<UnitType, UnitType>(UnitType.compensator, UnitType.pewpew), PIERCING_STRENGTH },
        {new Tuple<UnitType, UnitType>(UnitType.foundation, UnitType.steamer), PIERCING_STRENGTH },
        {new Tuple<UnitType, UnitType>(UnitType.foundation, UnitType.pewpew), PIERCING_STRENGTH },
    };

    public readonly static Dictionary<UnitType, UnitClass> UnitAssociations = new Dictionary<UnitType, UnitClass>() {
        //Support Associations
        {UnitType.claymore, UnitClass.support },
        {UnitType.powerSurge, UnitClass.support },
        {UnitType.midas, UnitClass.support },

        //Piercing Associations
        {UnitType.compensator, UnitClass.piercing },
        {UnitType.foundation, UnitClass.piercing },

        //Light Associations
        {UnitType.reacon, UnitClass.light },
        {UnitType.trooper, UnitClass.light },

        //Heavy Associations
        {UnitType.pewpew, UnitClass.heavy },
        {UnitType.steamer, UnitClass.heavy },

        //General Associations
    };

    public static float GetMultiplier(UnitType attacker, UnitType victim) {
        Tuple<UnitType, UnitType> pair = new Tuple<UnitType, UnitType>(attacker, victim);
        if(Strengths.ContainsKey(pair)) {
            return Strengths[pair];
        }
        return 1.0f; //do default damage;
    }
}
