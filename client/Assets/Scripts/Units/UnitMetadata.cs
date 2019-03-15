using System.Collections.Generic;

//Helper class to get a damage multiplier
public static class UnitMetadata {

    public readonly static int GENERAL_THRESHOLD = 100;

    private readonly static float LIGHT_STRENGTH = 1.0f;
    private readonly static float HEAVY_STRENGHTH = 1.0f;
    private readonly static float PIERCING_STRENGTH = 1.0f;

    //Update this with any new pairings that come up.
    //The floating point value represents the multiplier, we might want to change this in the future.
    private static Dictionary<Tuple<UnitType, UnitType>, float> Strengths = new Dictionary<Tuple<UnitType, UnitType>, float>() {
        //Light unit strengths
        {new Tuple<UnitType, UnitType>(UnitType.trooper, UnitType.compensator), LIGHT_STRENGTH },
        {new Tuple<UnitType, UnitType>(UnitType.trooper, UnitType.foundation), LIGHT_STRENGTH },
        {new Tuple<UnitType, UnitType>(UnitType.recon, UnitType.compensator), LIGHT_STRENGTH },
        {new Tuple<UnitType, UnitType>(UnitType.recon, UnitType.foundation), LIGHT_STRENGTH },

        //Heavy unit strengths
        {new Tuple<UnitType, UnitType>(UnitType.steamer, UnitType.trooper), HEAVY_STRENGHTH },
        {new Tuple<UnitType, UnitType>(UnitType.steamer, UnitType.recon), HEAVY_STRENGHTH },
        {new Tuple<UnitType, UnitType>(UnitType.pewpew, UnitType.trooper), HEAVY_STRENGHTH },
        {new Tuple<UnitType, UnitType>(UnitType.pewpew, UnitType.recon), HEAVY_STRENGHTH },

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
        {UnitType.recon, UnitClass.light },
        {UnitType.trooper, UnitClass.light },

        //Heavy Associations
        {UnitType.pewpew, UnitClass.heavy },
        {UnitType.steamer, UnitClass.heavy },

        //General Associations
        { UnitType.heavy_albarn,  UnitClass.general },
        { UnitType.piercing_tungsten,  UnitClass.general },
        { UnitType.light_adren,  UnitClass.general },
        { UnitType.support_sandman,  UnitClass.general },
    };

    public readonly static Dictionary<UnitType, string> ReadableNames = new Dictionary<UnitType, string>() {
        //Support names
        { UnitType.powerSurge, "Power Surge" },
        { UnitType.midas, "M.I.D.A.S" },
        { UnitType.claymore, "Claymore" },

        //Piercing names
        { UnitType.compensator, "Compensator" },
        { UnitType.foundation, "Foundation" },

        //Light names
        { UnitType.trooper, "Trooper" },
        { UnitType.recon, "Recon" },

        //Heavy names
        { UnitType.steamer, "Steamer" },
        { UnitType.pewpew, "Pew Pew" },

        //General names
        { UnitType.heavy_albarn, "Albarn" },
        { UnitType.piercing_tungsten, "Tungsten" },
        { UnitType.light_adren, "Adren-LN" },
        { UnitType.support_sandman, "The Sandman" },
    };

    public static float GetMultiplier(UnitType attacker, UnitType victim) {
        Tuple<UnitType, UnitType> pair = new Tuple<UnitType, UnitType>(attacker, victim);
        if(Strengths.ContainsKey(pair)) {
            return Strengths[pair];
        }
        return 1.0f; //do default damage;
    }
}
