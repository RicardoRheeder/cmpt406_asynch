using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CardsAndCarnage;

public enum CardFunction {
    //Generic Cards
    Reposition = 1,
    Retreat = 2, 
    StandYourGround =3,
    DoubleDown=4,
    Breakthrough=5,
    MassEffect=6, 
    ValiantEffort =7,
    ValianterEffort = 8,
    ValiantestEffort =9,
    Oopsie =10,
    AnArrowToTheKnee =11,
    WeShouldRun = 12,
    LeftForDead = 13,
    Suicide =14,
    ReallyBadLigma =15,
    AKneeToTheArrow =16,
    WeShouldRunRightFuckingNow =17,
    SurvivalRuleNumber1 =18,
    BigPP =19,
    Fallout =20,
    OOPSIE = 21,
    Ligma =22,
    QualifiedDoctor =23,
    ItAintMuch =24,
    PitifulAdvantage =25,
    OnePunch =26,
    TakingAdvantage =27,
    Formidibility =28,
    Oof =29,
    Reinforcements =30,
    DesperateAttempt =31,
    Ehttack =32,
    EHTTACK =33,
    Multistrike =34,
    Snipershot =35,
    SecondAttempt =36,
    AllInOne =37,
    MinorPrice =38,
    GirlNextDoor =39,
    Slowpoke =40,
    EndlessRunner =41,
    BigChungus =42,
    Foreground =43

    //Trooper Cards

    //Recon Cards

    //Steamer Cards

    //Pew Pew Cards

    //Compensator Cards

    //Foundation Cards

    //Power Surge Cards

    //Midas Cards

    //Claymore Cards

};

public static class CardMetadata {

    public static readonly int GENERIC_CARD_LIMIT = 4;
    public static readonly int UNIQUE_CARD_LIMIT = 3;

    public static readonly Dictionary<CardFunction, CardEffect<UnitStats, Dictionary<Vector2Int, UnitStats>, string>> CardEffectDictionary = new Dictionary<CardFunction, CardEffect<UnitStats, Dictionary<Vector2Int, UnitStats>, string>>() {
    };

    private static bool Card1(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(4);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card2(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(5);
                unit.AlterDamage(-20);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card3(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(-4);
                unit.AlterDamage(20);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card4(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AttackActions += 1;
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card5(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterPierce(10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card6(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        List<Vector2Int> positions = HexUtility.GetTilePositionsInRangeWithoutMap(source, 1);
        foreach (var pos in positions) {
            if (allUnits.TryGetValue(source, out UnitStats unit)) {
                if (unit.Owner == username) {
                    unit.Heal(5);
                }
            }
        }
        return true;
    }

    private static bool Card7(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(1);
                unit.Heal(10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card8(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(2);
                unit.AlterDamage(10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card9(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(-1);
                unit.AlterDamage(20);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card10(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AttackActions += 1;
                unit.AlterDamage(-10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card11(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        List<Vector2Int> positions = HexUtility.GetTilePositionsInRangeWithoutMap(source, 1);
        foreach (var pos in positions) {
            if (allUnits.TryGetValue(source, out UnitStats unit)) {
                if (unit.Owner == username) {
                    unit.AlterSpeed(2);
                }
            }
        }
        return true;
    }

    private static bool Card12(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.MovementActions = 0;
                unit.AlterDamage(5);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }
    
    private static bool Card13(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.TakeCardDamage(30);
                unit.AlterArmour(60);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card14(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(30);
                unit.Heal(10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card15(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AttackActions += 1;
                unit.AlterDamage(10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card16(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AttackActions += 1;
                unit.MovementActions += 1;
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card17(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(2);
                unit.AlterRange(3);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card18(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(30);
                unit.AlterRange(-1);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card19(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(30);
                unit.AlterSpeed(-1);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card20(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(20);
                unit.Heal(10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card21(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                List<Vector2Int> positions = HexUtility.GetTilePositionsInRangeWithoutMap(source, 1);
                unit.TakeCardDamage(10);
                foreach(var pos in positions) {
                    if(allUnits.TryGetValue(pos, out UnitStats friendly)) {
                        if (friendly.Owner == username) {
                            friendly.Heal(30);
                        }
                    }
                }
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card22(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(-1);
                unit.AlterDamage(-1);
                unit.Heal(3);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card23(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(1);
                unit.AlterRange(1);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card24(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(4);
                unit.AlterRange(-2);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card25(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(3);
                unit.AlterRange(1);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card26(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterArmour(10);
                unit.AlterDamage(10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card27(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterArmour(-5);
                unit.AlterDamage(2);
                unit.TakeCardDamage(5);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card28(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterArmour(1);
                unit.MovementActions += 1;
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card29(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.Heal(3);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card30(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(3);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card31(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(4);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card32(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(2);
                unit.AlterSpeed(-2);
                unit.AttackActions += 1;
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card33(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.MovementActions = 0;
                unit.AlterDamage(3);
                unit.AlterRange(2);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card34(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.MovementActions = 0;
                unit.AttackActions += 1;
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card35(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(2);
                unit.AlterPierce(10);
                unit.AlterDamage(20);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card36(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.TakeCardDamage(2);
                unit.AlterDamage(20);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card37(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.Range = 1;
                unit.AlterDamage(30);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card38(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(3);
                unit.Heal(10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card39(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterSpeed(-1);
                unit.AlterDamage(15);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card40(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AttackActions = 0;
                unit.MovementSpeed = 2 * unit.MovementSpeed;
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card41(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterArmour(10);
                unit.AlterDamage(10);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Card42(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterArmour(10);
                unit.AlterSpeed(2);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }
}
