using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Action {

    [DataMember(Name = "username")]
    public string Username { get; private set; }

    [DataMember]
    private int actionType;
    public ActionType Type;

    [DataMember(Name = "originXPos")]
    public int OriginXPos{get; private set;}
    [DataMember(Name = "originYPos")]
    public int OriginYPos{get; private set;}

    [DataMember(Name = "targetXPos")]
    public int TargetXPos{get; private set;}
    [DataMember(Name = "targetYPos")]
    public int TargetYPos{get; private set;}

    [DataMember]
    private int cardId;
    public CardFunction CardId { get; private set; }

    [DataMember]
    private int abilityNumber;
    public GeneralAbility Ability { get; private set; }

    public Action(string username, ActionType type, Vector2Int sourceTile, Vector2Int targetTile, GeneralAbility ability, CardFunction function) {
        this.Username = username;
        this.Type = type;
        this.OriginXPos = sourceTile.x;
        this.OriginYPos = sourceTile.y;
        this.TargetXPos = targetTile.x;
        this.TargetYPos = targetTile.y;
        this.Ability = ability;
        this.CardId = function;
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        this.Type = (ActionType)actionType;
        Ability = (GeneralAbility)abilityNumber;
        CardId = (CardFunction)cardId;
    }

    [OnSerializing]
    public void OnSerializing(StreamingContext c) {
        actionType = (int)Type;
        abilityNumber = (int)Ability;
        cardId = (int)CardId;
    }
}
