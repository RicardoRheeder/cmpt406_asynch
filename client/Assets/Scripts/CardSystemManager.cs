using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

#pragma warning disable 649
public class CardSystemManager : MonoBehaviour {

    [SerializeField]
    private GameObject CardTemplate;

    [HideInInspector] public GameObject cardDisplayPanel;
    [HideInInspector] public bool cardBeingDragged = false;

    public int totalCardPoints = 10;
    private int remainingCardPoints;
    public int discardRegainPointsAmount = 1;
    public TextMeshProUGUI CardPointsText;

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
    private Card Reposition;
    [SerializeField]
    private Card Retreat;
    [SerializeField]
    private Card StandYourGround;
    [SerializeField]
    private Card DoubleDown;
    [SerializeField]
    private Card Breakthrough;
    [SerializeField]
    private Card MassEffect;
    [SerializeField]
    private Card ValiantEffort;
    [SerializeField]
    private Card ValianterEffort;
    [SerializeField]
    private Card ValiantestEffort;
    [SerializeField]
    private Card Oopsie;
    [SerializeField]
    private Card AnArrowToTheKnee;
    [SerializeField]
    private Card WeShouldRun;
    [SerializeField]
    private Card LeftForDead;
    [SerializeField]
    private Card Suicide;
    [SerializeField]
    private Card ReallyBadLigma;
    [SerializeField]
    private Card AKneeToTheArrow;
    [SerializeField]
    private Card WeShouldRunRightFuckingNow;
    [SerializeField]
    private Card SurvivalRuleNumber1;
    [SerializeField]
    private Card BigPP;
    [SerializeField]
    private Card Fallout;
    [SerializeField]
    private Card OOPSIE;
    [SerializeField]
    private Card Ligma;
    [SerializeField]
    private Card QualifiedDoctor;
    [SerializeField]
    private Card ItAintMuch;
    [SerializeField]
    private Card PitifulAdvantage;
    [SerializeField]
    private Card OnePunch;
    [SerializeField]
    private Card TakingAdvantage;
    [SerializeField]
    private Card Formidibility;
    [SerializeField]
    private Card Oof;
    [SerializeField]
    private Card Reinforcements;
    [SerializeField]
    private Card DesperateAttempt;
    [SerializeField]
    private Card Ehttack;
    [SerializeField]
    private Card EHTTACK;
    [SerializeField]
    private Card Multistrike;
    [SerializeField]
    private Card Snipershot;
    [SerializeField]
    private Card SecondAttempt;
    [SerializeField]
    private Card AllInOne;
    [SerializeField]
    private Card MinorPrice;
    [SerializeField]
    private Card GirlNextDoor;
    [SerializeField]
    private Card Slowpoke;
    [SerializeField]
    private Card EndlessRunner;
    [SerializeField]
    private Card BigChungus;
    [SerializeField]
    private Card Foreground;
    [SerializeField]
    private Card RunAndGun;
    [SerializeField]
    private Card OnTheJuice;
    [SerializeField]
    private Card EagleAspect;
    [SerializeField]
    private Card NoobTube;
    [SerializeField]
    private Card WeGetItYouVape;
    [SerializeField]
    private Card RealSteel;
    [SerializeField]
    private Card AntiballisticShieldMatrix;
    [SerializeField]
    private Card PewPewsRevenge;
    [SerializeField]
    private Card SizeMatters;
    [SerializeField]
    private Card OverCompensation;
    [SerializeField]
    private Card BothBarrels;
    [SerializeField]
    private Card ToTheSkies;
    [SerializeField]
    private Card DeathBall;
    [SerializeField]
    private Card HighGroundSimulator;
    [SerializeField]
    private Card DeployWeldATron3000;
    [SerializeField]
    private Card Overclocked;
    [SerializeField]
    private Card OilSlick;
    [SerializeField]
    private Card PoppinSmoke;

    private static Dictionary<CardFunction, Card> library;

    void Awake() {
        cardDisplayPanel = GameObject.Find("CardDisplayPanel");
        cardDisplayPanel.SetActive(false);

        library = new Dictionary<CardFunction, Card>() {
            { CardFunction.Reposition, Reposition },
            { CardFunction.Retreat, Retreat },
            { CardFunction.StandYourGround, StandYourGround},
            { CardFunction.DoubleDown, DoubleDown},
            { CardFunction.Breakthrough, Breakthrough},
            { CardFunction.MassEffect, MassEffect},
            { CardFunction.ValiantEffort, ValiantEffort},
            { CardFunction.ValianterEffort, ValianterEffort },
            { CardFunction.ValiantestEffort, ValiantestEffort },
            { CardFunction.Oopsie, Oopsie },
            { CardFunction.AnArrowToTheKnee, AnArrowToTheKnee},
            { CardFunction.WeShouldRun, WeShouldRun},
            { CardFunction.LeftForDead, LeftForDead},
            { CardFunction.Suicide, Suicide},
            { CardFunction.ReallyBadLigma, ReallyBadLigma},
            { CardFunction.AKneeToTheArrow, AKneeToTheArrow},
            { CardFunction.WeShouldRunRightFuckingNow, WeShouldRunRightFuckingNow},
            { CardFunction.SurvivalRuleNumber1, SurvivalRuleNumber1},
            { CardFunction.BigPP, BigPP},
            { CardFunction.Fallout, Fallout},
            { CardFunction.OOPSIE, OOPSIE},
            { CardFunction.Ligma, Ligma},
            { CardFunction.QualifiedDoctor, QualifiedDoctor },
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
            { CardFunction.Foreground, Foreground},{ CardFunction.RunAndGun, RunAndGun},
            { CardFunction.OnTheJuice, OnTheJuice},
            { CardFunction.EagleAspect, EagleAspect},
            { CardFunction.NoobTube, NoobTube},
            { CardFunction.WeGetItYouVape, WeGetItYouVape},
            { CardFunction.RealSteel, RealSteel},
            { CardFunction.AntiballisticShieldMatrix, AntiballisticShieldMatrix},
            { CardFunction.PewPewsRevenge, PewPewsRevenge},
            { CardFunction.SizeMatters, SizeMatters},
            { CardFunction.OverCompensation, OverCompensation},
            { CardFunction.BothBarrels, BothBarrels},
            { CardFunction.ToTheSkies, ToTheSkies},
            { CardFunction.DeathBall, DeathBall},
            { CardFunction.HighGroundSimulator, HighGroundSimulator},
            { CardFunction.DeployWeldATron3000, DeployWeldATron3000},
            { CardFunction.Overclocked, Overclocked},
            { CardFunction.OilSlick, OilSlick},
            { CardFunction.PoppinSmoke, PoppinSmoke},
        };
    }

    void Start() {
        this.remainingCardPoints = totalCardPoints;
        CardPointsText.text = "Card Points: " + this.remainingCardPoints;
    }

    public void Initialize(List<CardFunction> startingHand, List<UnitStats> playerUnits, string id) {
        TableTop = GameObject.Find("Tabletop");
        TableTop.SetActive(false);
        TableHand = GameObject.Find("Hand");

        bool previouslyGeneratedCards = false;
        string path = CardMetadata.FILE_PATH_BASE + "/." + id;
        if(System.IO.File.Exists(path)) {
            previouslyGeneratedCards = true;
            StreamReader reader = new StreamReader(path);
            string cardData = reader.ReadToEnd();
            string[] cards = cardData.Split('X');
            startingHand.Clear();
            for(int i = 0; i < CardMetadata.GENERIC_CARD_LIMIT + CardMetadata.UNIQUE_CARD_LIMIT; i++) {
                startingHand.Add((CardFunction)int.Parse(cards[i]));
            }
        }

        currentHand = new List<Card>();
        foreach (var card in startingHand) {
            currentHand.Add(library[card]);
        }
        int genericCards = 0;
        int uniqueCards = 0;
        foreach (var card in currentHand) {
            GameObject newCard = (GameObject)Instantiate(CardTemplate, transform.position, Quaternion.identity);
            newCard.GetComponent<CardDisplay>().SetCard(card);
            newCard.GetComponent<Draggable>().SetTableTop(TableTop);
            newCard.transform.SetParent(TableHand.transform, false);
            if (card.type == UnitType.none) {
                genericCards++;
            }
            else {
                uniqueCards++;
            }
        }

        if (!previouslyGeneratedCards) {
            List<UnitType> types = new List<UnitType>();
            for (int i = 0; i < playerUnits.Count; i++) {
                UnitStats unit = playerUnits[i];
                if ((int)unit.UnitType < UnitMetadata.GENERAL_THRESHOLD) {
                    types.Add(unit.UnitType);
                }
            }
            while (genericCards < CardMetadata.GENERIC_CARD_LIMIT) {
                DrawCard();
                genericCards++;
            }
            while (uniqueCards < CardMetadata.UNIQUE_CARD_LIMIT && types.Count > 0) {
                DrawCard(types[Random.Range(0, types.Count)]);
                uniqueCards++;
            }

            StreamWriter writer = new StreamWriter(path);
            StringBuilder cardData = new StringBuilder();
            foreach(var card in currentHand) {
                cardData.Append((int)card.func + "X");
            }
            writer.WriteLine(cardData.ToString());
            writer.Close();
        }
    }

    public List<CardFunction> EndTurn() {
        List<CardFunction> returnList = new List<CardFunction>();
        foreach(var card in currentHand) {
            returnList.Add(card.func);
        }
        currentHand = null;
        currentCard = null;
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
        drawnCard.GetComponent<CardDisplay>().SetCard(currentCard);
        drawnCard.GetComponent<Draggable>().SetTableTop(TableTop);
        drawnCard.transform.SetParent(TableHand.transform, false);
    }

    public void PlayCard(Card card) {
        if (currentHand.Contains(card)){
            currentHand.Remove(card);
        }
    }

    public void DeductCardPoints(int points) {
        this.remainingCardPoints -= points;
        CardPointsText.text = "Card Points: " + this.remainingCardPoints;
    }

    public void IncrementCardPoints(int points) {
        this.remainingCardPoints += points;
        CardPointsText.text = "Card Points: " + this.remainingCardPoints;
    }

    public int GetRemainingCardPoints() {
        return remainingCardPoints;
    }
}
