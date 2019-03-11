using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CardsAndCarnage;

public enum CardFunction {
    //Generic Cards
    Reposition = 1,
    Retreat = 2, 
    StandYourGround = 3,
    DoubleDown = 4,
    Breakthrough = 5,
    MassEffect = 6, 
    ValiantEffort = 7,
    ValianterEffort = 8,
    ValiantestEffort = 9,
    Oopsie = 10,
    AnArrowToTheKnee = 11,
    WeShouldRun = 12,
    LeftForDead = 13,
    Suicide = 14,
    ReallyBadLigma = 15,
    AKneeToTheArrow = 16,
    WeShouldRunRightFuckingNow = 17,
    SurvivalRuleNumber1 = 18,
    BigPP = 19,
    Fallout = 20,
    OOPSIE = 21,
    Ligma = 22,
    QualifiedDoctor = 23,
    ItAintMuch = 24,
    PitifulAdvantage = 25,
    OnePunch = 26,
    TakingAdvantage = 27,
    Formidibility = 28,
    Oof = 29,
    Reinforcements = 30,
    DesperateAttempt = 31,
    Ehttack = 32,
    EHTTACK = 33,
    Multistrike = 34,
    Snipershot = 35,
    SecondAttempt = 36,
    AllInOne = 37,
    MinorPrice = 38,
    GirlNextDoor = 39,
    Slowpoke = 40,
    EndlessRunner = 41,
    BigChungus = 42,
    Foreground = 43

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

    public static readonly Dictionary<CardFunction, CardEffect<Vector2Int, Dictionary<Vector2Int, UnitStats>, string>> CardEffectDictionary = new Dictionary<CardFunction, CardEffect<Vector2Int, Dictionary<Vector2Int, UnitStats>, string>>() {
        { CardFunction.Reposition, Reposition},
        { CardFunction.Retreat , Retreat},
        { CardFunction.StandYourGround, StandYourGround},
        { CardFunction.DoubleDown, DoubleDown},
        { CardFunction.Breakthrough, Breakthrough},
        { CardFunction.MassEffect, MassEffect},
        { CardFunction.ValiantEffort, ValiantEffort},
        { CardFunction.ValianterEffort, ValianterEffort},
        { CardFunction.ValiantestEffort, ValiantestEffort},
        { CardFunction.Oopsie, Oopsie},
        { CardFunction.AnArrowToTheKnee, AnArrowToTheKnee},
        { CardFunction.WeShouldRun, WeShouldRun},
        { CardFunction.LeftForDead, LeftForDead},
        { CardFunction.Suicide, Suicide},
        { CardFunction.ReallyBadLigma, ReallyBadLigma},
        { CardFunction.AKneeToTheArrow, AKneeToTheArrow},
        { CardFunction.WeShouldRunRightFuckingNow, WeShouldRunRightFuckingNow},
        { CardFunction.SurvivalRuleNumber1, SurvivalRuleNumberOne},
        { CardFunction.BigPP, BigPP},
        { CardFunction.Fallout, Fallout},
        { CardFunction.OOPSIE, OOPSIE},
        { CardFunction.Ligma, Ligma},
        { CardFunction.QualifiedDoctor, QualifiedDoctor},
        { CardFunction.ItAintMuch, ItAintMuch},
        { CardFunction.PitifulAdvantage, PitifulAdvantage},
        { CardFunction.OnePunch, OnePunch},
        { CardFunction.TakingAdvantage, TakingAdvantage},
        { CardFunction.Formidibility, Formidibility},
        { CardFunction.Oof, Oof},
        { CardFunction.Reinforcements, Reinforcements},
        { CardFunction.DesperateAttempt, DesperateAttempt},
        { CardFunction.Ehttack, Ehttack},
        { CardFunction.EHTTACK, EHTTACK},
        { CardFunction.Multistrike, Multistrike},
        { CardFunction.Snipershot, Snipershot},
        { CardFunction.SecondAttempt, SecondAttempt},
        { CardFunction.AllInOne, AllInOne},
        { CardFunction.MinorPrice, MinorPrice},
        { CardFunction.GirlNextDoor, GirlNextDoor},
        { CardFunction.Slowpoke, Slowpoke},
        { CardFunction.EndlessRunner, EndlessRunner},
        { CardFunction.BigChungus, BigChungus},
        { CardFunction.Foreground, Foreground}
    };

    private static bool Reposition(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Retreat(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool StandYourGround(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool DoubleDown (Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Breakthrough(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool MassEffect(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool ValiantEffort(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool ValianterEffort(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Oopsie(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool AnArrowToTheKnee(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool WeShouldRun(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool LeftForDead(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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
    
    private static bool Suicide(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool ReallyBadLigma(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool AKneeToTheArrow(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool WeShouldRunRightFuckingNow(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool SurvivalRuleNumberOne(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool BigPP(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(20);
                unit.AlterRange(3);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool Fallout(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool OOPSIE(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Ligma(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool QualifiedDoctor(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool ItAintMuch(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool PitifulAdvantage(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool OnePunch(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
        if (allUnits.TryGetValue(source, out UnitStats unit)) {
            if (unit.Owner == username) {
                unit.AlterDamage(4);
                unit.AlterRange(-1);
            }
            else {
                return false;
            }
            return true;
        }
        return false;
    }

    private static bool TakingAdvantage(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Formidibility(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Oof(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Reinforcements(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool DesperateAttempt(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Ehttack(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool EHTTACK(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Multistrike(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Snipershot(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool SecondAttempt(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool AllInOne(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool MinorPrice(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool GirlNextDoor(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool ValiantestEffort(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Slowpoke(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool EndlessRunner(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool BigChungus(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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

    private static bool Foreground(Vector2Int source, Dictionary<Vector2Int, UnitStats> allUnits, string username) {
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
