using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class GameState {

    [DataMember(IsRequired=true)]
    public string id;

    [DataMember(IsRequired = true)]
    private string gameName;

    [DataMember(IsRequired = true)]
    private string createdBy;

    [DataMember(IsRequired = true)]
    public BoardType boardId;

    [DataMember]
    private int maxUsers;

    [DataMember]
    private int spotsAvailable;

    [DataMember]
    private bool isPublic;

    [DataMember]
    private List<string> users;

    [DataMember]
    private List<string> acceptedUsers;

    [DataMember]
    private List<string> readyUsers;

    [DataMember]
    private List<string> aliveUsers;

    [DataMember]
    private string usersTurn;

    [DataMember]
    private List<UnitStats> units;
    public Dictionary<string, List<UnitStats>> userUnitsMap;

    [DataMember]
    private List<CardController> cards;
    public Dictionary<string, CardController> userCardsMap;

    [DataMember]
    public List<Action> actions;

    [DataMember]
    private int turnTime;

    [DataMember(Name="timeToStartTurn")]
    private int forfeitTime;

    public override string ToString() {
        return JsonConversion.ConvertObjectToJson(typeof(GameState), this);
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        foreach(UnitStats unit in units) {
            if (userUnitsMap.ContainsKey(unit.Owner))
                userUnitsMap[unit.Owner].Add(unit);
            else
                userUnitsMap.Add(unit.Owner, new List<UnitStats>() { unit });
        }

        foreach(CardController card in cards) {
            userCardsMap.Add(card.owner, card);
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
        return JsonConversion.ConvertObjectToJson(typeof(GameStateCollection), this);
    }
}

//Special object used to create a game
[DataContract]
public class CreatePrivateGameState {

    [DataMember]
    private string gameName;

    [DataMember]
    private int turnTime;

    [DataMember( Name = "timeToStartTurn")]
    private int forfeitTime;

    [DataMember]
    private List<string> opponentUsernames;

    [DataMember]
    private int boardId;

    public CreatePrivateGameState(string name, int turnTime, int forfeitTime, List<string> opponents, int boardId) {
        this.gameName = name;
        this.turnTime = turnTime;
        this.forfeitTime = forfeitTime;
        this.opponentUsernames = opponents;
        this.boardId = boardId;
    }
}
