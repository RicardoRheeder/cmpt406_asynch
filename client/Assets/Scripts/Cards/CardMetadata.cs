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

    public static readonly Dictionary<CardFunction, CardEffect<UnitStats>> CardEffectDictionary = new Dictionary<CardFunction, CardEffect<UnitStats>>() {
    };

    private static void IncreaseMovement(ref UnitStats unit) {

    }
}
