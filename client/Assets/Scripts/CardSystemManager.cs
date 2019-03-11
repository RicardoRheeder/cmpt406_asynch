using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class CardSystemManager : MonoBehaviour {

    [SerializeField]
    private GameObject CardTemplate; //Card_V2

    [SerializeField]
    private Deck genericDeck;
    [SerializeField]
    private Deck compensatorDeck;
    [SerializeField]
    private Deck foundationDeck;
    [SerializeField]
    private Deck reconDeck;
    [SerializeField]
    private Deck trooperDeck;
    [SerializeField]
    private Deck midasDeck;
    [SerializeField]
    private Deck powerSurgeDeck;
    [SerializeField]
    private Deck claymoreDeck;
    [SerializeField]
    private Deck pewpewDeck;
    [SerializeField]
    private Deck steamerDeck;

    private List<Card> currentHand;
    private Card currentCard;
    private GameObject TableTop;
    private GameObject TableHand;

    [SerializeField]
    private Card card1;

    private Card Reposition;
    private Card Retreat;
    private Card StandYourGround;
    private Card DoubleDown;
    private Card Breakthrough;
    private Card MassEffect;
    private Card ValiantEffort;
    private Card ValianterEffort;
    private Card ValiantestEffort;
    private Card Oopsie;
    private Card AnArrowToTheKnee;
    private Card WeShouldRun;
    private Card LeftForDead;
    private Card Suicide;
    private Card ReallyBadLigma;
    private Card AKneeToTheArrow;
    private Card WeShouldRunRightFuckingNow;
    private Card SurvivalRuleNumber1;
    private Card BigPP;
    private Card Fallout;
    private Card OOPSIE;
    private Card Ligma;
    private Card QualifiedDoctor;
    private Card ItAintMuch;
    private Card PitifulAdvantage;
    private Card OnePunch;
    private Card TakingAdvantage;
    private Card Formidibility;
    private Card Oof;
    private Card Reinforcements;
    private Card DesperateAttempt;
    private Card Ehttack;
    private Card EHTTACK;
    private Card Multistrike;
    private Card Snipershot;
    private Card SecondAttempt;
    private Card AllInOne;
    private Card MinorPrice;
    private Card GirlNextDoor;
    private Card Slowpoke;
    private Card EndlessRunner;
    private Card BigChungus;
    private Card Foreground;

    private static Dictionary<CardFunction, Card> library;

    void Awake() {
        TableTop = GameObject.Find("Tabletop");
        TableTop.SetActive(false);

        TableHand = GameObject.Find("Hand");

        library = new Dictionary<CardFunction, Card>() {
            {CardFunction.Reposition, Reposition },
            {CardFunction.Retreat, Retreat },
            {CardFunction.StandYourGround, StandYourGround},
            {CardFunction.DoubleDown, DoubleDown},
            {CardFunction.Breakthrough, Breakthrough},
            {CardFunction.MassEffect, MassEffect},
            {CardFunction.ValiantEffort, ValiantEffort},
            {CardFunction.ValianterEffort, ValianterEffort },
            {CardFunction.ValiantestEffort, ValiantestEffort },
            {CardFunction.Oopsie, Oopsie },
            {CardFunction.AnArrowToTheKnee, AnArrowToTheKnee},
            {CardFunction.WeShouldRun, WeShouldRun},
            {CardFunction.LeftForDead, LeftForDead},
            {CardFunction.Suicide, Suicide},
            {CardFunction.ReallyBadLigma, ReallyBadLigma},
            {CardFunction.AKneeToTheArrow, AKneeToTheArrow},
            {CardFunction.WeShouldRunRightFuckingNow, WeShouldRunRightFuckingNow},
            {CardFunction.SurvivalRuleNumber1, SurvivalRuleNumber1},
            {CardFunction.BigPP, BigPP},
            {CardFunction.Fallout, Fallout},
            {CardFunction.OOPSIE, OOPSIE},
            {CardFunction.Ligma, Ligma},
            {CardFunction.QualifiedDoctor, QualifiedDoctor },
            {CardFunction.ItAintMuch, ItAintMuch},
            {CardFunction.PitifulAdvantage, PitifulAdvantage},
            {CardFunction.OnePunch, OnePunch},
            {CardFunction.TakingAdvantage, TakingAdvantage},
            {CardFunction.Formidibility, Formidibility},
            {CardFunction.Oof, Oof},
            {CardFunction.Reinforcements, Reinforcements},
            {CardFunction.DesperateAttempt, DesperateAttempt},
            {CardFunction.Ehttack, Ehttack},
            {CardFunction.EHTTACK, EHTTACK},
            {CardFunction.Multistrike, Multistrike},
            {CardFunction.Snipershot, Snipershot},
            {CardFunction.SecondAttempt, SecondAttempt},
            {CardFunction.AllInOne, AllInOne},
            {CardFunction.MinorPrice, MinorPrice},
            {CardFunction.GirlNextDoor, GirlNextDoor},
            {CardFunction.Slowpoke, Slowpoke},
            {CardFunction.EndlessRunner, EndlessRunner},
            {CardFunction.BigChungus, BigChungus},
            {CardFunction.Foreground, Foreground},
        };
    }

    public void Initialize(List<CardFunction> startingHand, List<UnitStats> playerUnits) {
        currentHand = new List<Card>();
        foreach(var card in startingHand) {
            if(library.TryGetValue(card, out Card prefab)) { //TODO MAKE THIS CRASH ON MISSING CARDS
                currentHand.Add(prefab);
            }
            else {
                currentHand.Add(card1);
            }
        }

        foreach(var card in currentHand) {
            GameObject newCard = (GameObject)Instantiate(CardTemplate, transform.position, Quaternion.identity);
            newCard.GetComponent<CardDisplay_v2>().setCard(currentCard);
            newCard.GetComponent<Draggable>().SetTableTop(TableTop);
            newCard.transform.SetParent(TableHand.transform, false);
        }

        int genericCards = 0;
        int uniqueCards = 0;
        foreach (var card in currentHand) {
            if (card.type == UnitType.none) {
                genericCards++;
            }
            else {
                uniqueCards++;
            }
        }
        List<UnitType> types = new List<UnitType>();
        for(int i = 0; i < playerUnits.Count; i++) {
            UnitStats unit = playerUnits[i];
            if((int)unit.UnitType < UnitMetadata.GENERAL_THRESHOLD) {
                types.Add(unit.UnitType);
            }
        }
        while (genericCards < CardMetadata.GENERIC_CARD_LIMIT) {
            DrawCard();
            genericCards++;
        }
        while (uniqueCards < CardMetadata.UNIQUE_CARD_LIMIT) {
            DrawCard(types[Random.Range(0, types.Count)]);
            uniqueCards++;
        }
    }

    public List<CardFunction> EndTurn() {
        List<CardFunction> returnList = new List<CardFunction>();
        foreach(var card in currentHand) {
            returnList.Add(card.func);
        }
        return returnList;
    }

    private void DrawCard(UnitType unit = UnitType.none) {
        Deck currentDeck = genericDeck;
        switch (unit) {
            case UnitType.foundation:
                currentDeck = foundationDeck;
                break;
            case UnitType.compensator:
                currentDeck = compensatorDeck;
                break;
            case UnitType.recon:
                currentDeck = reconDeck;
                break;
            case UnitType.trooper:
                currentDeck = trooperDeck;
                break;
            case UnitType.pewpew:
                currentDeck = pewpewDeck;
                break;
            case UnitType.steamer:
                currentDeck = steamerDeck;
                break;
            case UnitType.midas:
                currentDeck = midasDeck;
                break;
            case UnitType.claymore:
                currentDeck = claymoreDeck;
                break;
            case UnitType.powerSurge:
                currentDeck = powerSurgeDeck;
                break;
            default:
                break;
        }
        currentCard = currentDeck.DrawCard();
        currentHand.Add(currentCard);

        GameObject drawnCard = (GameObject)Instantiate(CardTemplate, transform.position, Quaternion.identity);
        drawnCard.GetComponent<CardDisplay_v2>().setCard(currentCard);
        drawnCard.GetComponent<Draggable>().SetTableTop(TableTop);
        drawnCard.transform.SetParent(TableHand.transform, false);
    }
}
