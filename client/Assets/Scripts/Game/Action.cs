using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Action {

    [DataMember(Name = "owner")]
    public string Owner { get; private set; }

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
    private int ability;
    public GeneralAbility Ability { get; private set; }

    public Action(string owner, ActionType type, Vector2Int sourceTile, Vector2Int targetTile) {
        this.Owner = owner;
        this.Type = type;
        this.OriginXPos = sourceTile.x;
        this.OriginYPos = sourceTile.y;
        this.TargetXPos = targetTile.x;
        this.TargetYPos = targetTile.y;
    }

    public Action(string owner, ActionType type, Vector2Int sourceTile, Vector2Int targetTile, GeneralAbility ability) {
        this.Owner = owner;
        this.Type = type;
        this.OriginXPos = sourceTile.x;
        this.OriginYPos = sourceTile.y;
        this.TargetXPos = targetTile.x;
        this.TargetYPos = targetTile.y;
        this.Ability = ability;
    }

    public Action(string owner, ActionType type, Vector2Int targetTile, CardFunction function) {
        this.Owner = owner;
        this.Type = type;
        this.TargetXPos = targetTile.x;
        this.TargetYPos = targetTile.y;
        this.CardId = function;
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        this.Type = (ActionType)actionType;
        Ability = (GeneralAbility)ability; ;
        CardId = (CardFunction)cardId;
    }

    [OnSerializing]
    public void OnSerializing(StreamingContext c) {
        actionType = (int)Type;
        ability = (int)Ability;
        cardId = (int)CardId;
    }
}
