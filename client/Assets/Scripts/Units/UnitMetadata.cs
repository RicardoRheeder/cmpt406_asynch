using System.Collections.Generic;

//Helper class to get a damage multiplier
public static class UnitMetadata {

    //Update this with any new pairings that come up.
    //The floating point value represents the multiplier, we might want to change this in the future.
    private static Dictionary<Tuple<UnitType, UnitType>, float> Strengths = new Dictionary<Tuple<UnitType, UnitType>, float>() {
        //Light unit strengths
        {new Tuple<UnitType, UnitType>(UnitType.trooper, UnitType.compensator), 1.5f },
        {new Tuple<UnitType, UnitType>(UnitType.trooper, UnitType.foundation), 1.5f },
        {new Tuple<UnitType, UnitType>(UnitType.reacon, UnitType.compensator), 1.5f },
        {new Tuple<UnitType, UnitType>(UnitType.reacon, UnitType.foundation), 1.5f },

        //Heavy unit strengths
        {new Tuple<UnitType, UnitType>(UnitType.steamer, UnitType.trooper), 1.5f },
        {new Tuple<UnitType, UnitType>(UnitType.steamer, UnitType.reacon), 1.5f },
        {new Tuple<UnitType, UnitType>(UnitType.pewpew, UnitType.trooper), 1.5f },
        {new Tuple<UnitType, UnitType>(UnitType.pewpew, UnitType.reacon), 1.5f },

        //Piercing unit strengths
        {new Tuple<UnitType, UnitType>(UnitType.compensator, UnitType.steamer), 1.5f },
        {new Tuple<UnitType, UnitType>(UnitType.compensator, UnitType.pewpew), 1.5f },
        {new Tuple<UnitType, UnitType>(UnitType.foundation, UnitType.steamer), 1.5f },
        {new Tuple<UnitType, UnitType>(UnitType.foundation, UnitType.pewpew), 1.5f },

    };

    public static float GetMultiplier(UnitType attacker, UnitType victim) {
        Tuple<UnitType, UnitType> pair = new Tuple<UnitType, UnitType>(attacker, victim);
        if(Strengths.ContainsKey(pair)) {
            return Strengths[pair];
        }
        return 1.0f; //do default damage;
    }
}
