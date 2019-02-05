using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class GameState {

    [DataMember(IsRequired=true)]
    private string id;

    [DataMember(IsRequired=true)]
    private BoardType boardId;

    [DataMember(IsRequired=true)]
    private int maxUsers;

    [DataMember(IsRequired=true)]
    private int spotsAvailable;

    [DataMember(IsRequired=true)]
    private bool isPublic;

    [DataMember(IsRequired=true)]
    private List<string> users;

    [DataMember(IsRequired=true)]
    private List<string> acceptedUsers;

    [DataMember(IsRequired=true)]
    private List<string> readyUsers;

    [DataMember(IsRequired=true)]
    private List<string> aliveUsers;

    [DataMember(IsRequired=true)]
    private string usersTurn;

    [DataMember]
    private Dictionary<string, List<UnitStats>> units;

    [DataMember]
    public Dictionary<string, Card[]> hand;

    [DataMember]
    public Dictionary<string, Card[]> drawPile;

    [DataMember]
    public Dictionary<string, Card[]> discardPile;

    [DataMember]
    private Dictionary<string, General> generals;

    public GameState (string id, int type, int maxUsers, bool isPublic, List<string> users, List<string> acceptedUsers, List<string> readyUsers, List<string> aliveUsers, string usersTurn, Dictionary<string, List<UnitStats>> units, Dictionary<string, Card[]> hand, Dictionary<string, Card[]> drawPile, Dictionary<string, Card[]> discardPile, Dictionary<string, General> generals) {
        this.id = id;
        this.boardId = (BoardType)type;
        this.maxUsers = maxUsers;
        this.isPublic = isPublic;
        this.users = users;
        this.acceptedUsers = acceptedUsers;
        this.readyUsers = readyUsers;
        this.aliveUsers = aliveUsers;
        this.usersTurn = usersTurn;
        this.units = units;
        this.hand = hand;
        this.drawPile = drawPile;
        this.discardPile = discardPile;
        this.generals = generals;
    }
}

//Used to create a collection of gamestates from a server response
[DataContract]
public class GameStateCollection {

    [DataMember]
    public List<GameState> states;
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
