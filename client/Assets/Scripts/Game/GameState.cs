using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class GameState {

    [DataMember]
    private string id;

    [DataMember]
    private BoardType boardId;

    [DataMember]
    private int maxUsers;

    [DataMember]
    private bool isPublic;

    [DataMember]
    private string[] users;

    [DataMember]
    private string[] acceptedUsers;

    [DataMember]
    private string[] readyUsers;

    [DataMember]
    private string[] aliveUsers;

    [DataMember]
    private string usersTurn;

    [DataMember]
    private Dictionary<string, Unit[]> units;

    [DataMember]
    private Dictionary<string, Cards> cards;

    [DataMember]
    private Dictionary<string, General> generals;

    public GameState (string id, int type, int maxUsers, bool isPublic, string[] users, string[] acceptedUsers, string[] readyUsers, string[] aliveUsers, string usersTurn, Dictionary<string, Unit[]> units, Dictionary<string, Cards> cards, Dictionary<string, General> generals) {
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
        this.cards = cards;
        this.generals = generals;
    }
}

//Used to create a collection of gamestates from a server response
[DataContract]
public class GameStateCollection {

    [DataMember]
    public List<GameState> states;
}
