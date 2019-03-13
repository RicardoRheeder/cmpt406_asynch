using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class GameState {

    [DataMember(IsRequired=true)]
    public readonly string id;

    [DataMember(IsRequired = true)]
    public readonly string gameName;

    [DataMember(IsRequired = true)]
    public readonly string createdBy;

    [DataMember(IsRequired = true)]
    public readonly BoardType boardId;

    [DataMember]
    public readonly int maxUsers;

    [DataMember]
    public readonly int spotsAvailable;

    [DataMember]
    public readonly bool isPublic;

    [DataMember(Name = "users")]
    public List<string> Users { get; private set; }

    [DataMember(Name = "acceptedUsers")]
    public List<string> AcceptedUsers { get; private set; }

    [DataMember(Name = "readyUsers")]
    public List<string> ReadyUsers { get; private set; }

    [DataMember(Name = "aliveUsers")]
    public List<string> AliveUsers { get; private set; }

    [DataMember(Name = "usersTurn")]
    public string UsersTurn { get; private set; }

    [DataMember]
    private List<UnitStats> units;
    public Dictionary<string, List<UnitStats>> UserUnitsMap { get; private set; }

    [DataMember(Name="initUnits")]
    public List<UnitStats> InitUnits { get; private set; }

    [DataMember]
    private List<UnitStats> generals;
    public Dictionary<string, List<UnitStats>> UserGeneralsMap { get; private set; }

    [DataMember]
    private List<CardController> cards;
    public Dictionary<string, CardController> UserCardsMap { get; private set; }

    [DataMember(Name = "actions")]
    public List<Action> Actions { get; private set; }

    [DataMember(Name="forfeitTime")]
    public int ForfeitTime { get; private set; }

    [DataMember(Name = "turnNumber")]
    public int TurnNumber { get; private set; }

    public override string ToString() {
        return JsonConversion.ConvertObjectToJson(this);
    }

    public string GetDescription() {
        return this.gameName + ", " + this.createdBy;
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        if (units == null) units = new List<UnitStats>();
        if (Users == null) Users = new List<string>();
        if (AcceptedUsers == null) AcceptedUsers = new List<string>();
        if (ReadyUsers == null) ReadyUsers = new List<string>();
        if (cards == null) cards = new List<CardController>();
        if (Actions == null) Actions = new List<Action>();
        UserUnitsMap = new Dictionary<string, List<UnitStats>>();
        foreach(UnitStats unit in units) {
            if (UserUnitsMap.ContainsKey(unit.Owner))
                UserUnitsMap[unit.Owner].Add(unit);
            else
                UserUnitsMap.Add(unit.Owner, new List<UnitStats>() { unit });
        }

        if (generals == null) generals = new List<UnitStats>();
        UserGeneralsMap = new Dictionary<string, List<UnitStats>>();
        foreach(UnitStats general in generals) {
            if (UserGeneralsMap.ContainsKey(general.Owner))
                UserGeneralsMap[general.Owner].Add(general);
            else
                UserGeneralsMap.Add(general.Owner, new List<UnitStats>() { general });
        }

        UserCardsMap = new Dictionary<string, CardController>();
        foreach(CardController card in cards) {
            UserCardsMap.Add(card.owner, card);
        }
    }
}

//Used to create a collection of gamestates from a server response
[DataContract]
public class GameStateCollection {

    [DataMember]
    public List<GameState> states;
    public Dictionary<string, GameState> idToStateMap;

    //This function will be run after the class is serialized from the JSON string
    //you can use this to set default values, or ensure values are within some valid context
    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        idToStateMap = new Dictionary<string, GameState>();
        foreach(var state in states) {
            idToStateMap.Add(state.id, state);
        }
    }

    public override string ToString() {
        return JsonConversion.ConvertObjectToJson(this);
    }
}

//Special object used to create a game
[DataContract]
public class CreatePrivateGameState {

    [DataMember]
    private string gameName;

    [DataMember]
    private int forfeitTime;

    [DataMember]
    private List<string> opponentUsernames;

    [DataMember]
    private int boardId;

    public CreatePrivateGameState(string name, int forfeitTime, List<string> opponents, int boardId) {
        this.gameName = name;
        this.forfeitTime = forfeitTime;
        this.opponentUsernames = opponents;
        this.boardId = boardId;
    }
}

//Special object used to create a public game
[DataContract]
public class CreatePublicGameState {
    [DataMember]
    private string gameName;

    [DataMember]
    private int forfeitTime;

    [DataMember]
    private int maxUsers;

    [DataMember]
    private int boardId;

    public CreatePublicGameState(string name, int forfeitTime, int maxPlayers, int boardId) {
        this.gameName = name;
        this.forfeitTime = forfeitTime;
        this.maxUsers = maxPlayers;
        this.boardId = boardId;
    }
}

//Special object used to ready Units
[DataContract]
public class ReadyUnitsGameState {

    [DataMember]
    private string gameId;

    [DataMember]
    private List<UnitStats> units;

    [DataMember]
    private UnitStats general;

    [DataMember]
    private CardController cards;

    public ReadyUnitsGameState(string gameId, List<UnitStats> units, UnitStats general, CardController cards) {
        this.gameId = gameId;
        this.units = units;
        this.general = general;
        this.cards = cards;
    }
}

//Special objected used to make a move
[DataContract]
public class EndTurnState {

    [DataMember]
    private string gameId;

    [DataMember]
    private List<UnitStats> units;

    [DataMember]
    private List<UnitStats> generals;

    [DataMember]
    private List<CardController> cards;

    [DataMember]
    private List<Action> actions;

    [DataMember]
    private List<string> killedUsers;

    public EndTurnState(GameState state, string currentUser, List<Action> turnActions, List<UnitStats> allUnits, List<CardFunction> cards) {
        gameId = state.id;
        actions = turnActions;

        CardController userCards = new CardController(currentUser, cards);
        this.cards = new List<CardController>();
        foreach (CardController controller in state.UserCardsMap.Values) {
            if(controller.owner != currentUser) {
                this.cards.Add(controller);
            }
            else {
                userCards.id = controller.id;
            }
        }
        this.cards.Add(userCards);

        List<string> aliveUsers = new List<string>(state.AliveUsers);
        units = new List<UnitStats>();
        generals = new List<UnitStats>();
        foreach(var unit in allUnits) {
            if ((int)unit.UnitType > UnitMetadata.GENERAL_THRESHOLD) {
                generals.Add(unit);
            }
            else {
                units.Add(unit);
            }
            aliveUsers.Remove(unit.Owner);
        }
        killedUsers = new List<string>(aliveUsers);
    }
}
