using System.Collections.Generic;

//Helper class to get a damage multiplier
public static class UnitMetadata {

    //Update this with any new pairings that come up.
    //The floating point value represents the multiplier, we might want to change this in the future.
    private static Dictionary<Tuple<UnitType, UnitType>, float> Strengths = new Dictionary<Tuple<UnitType, UnitType>, float>() {
    };

    public static float GetMultiplier(UnitType attacker, UnitType victim) {
        Tuple<UnitType, UnitType> pair = new Tuple<UnitType, UnitType>(attacker, victim);
        if(Strengths.ContainsKey(pair)) {
            return Strengths[pair];
        }
        return 1.0f; //do default damage;
    }
}
