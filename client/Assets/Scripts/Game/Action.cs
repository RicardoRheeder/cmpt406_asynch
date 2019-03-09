using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Action {

    [DataMember]
    private string owner;

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

    [DataMember(Name = "cardId")]
    public int CardId { get; private set; }

    [DataMember(Name = "ability")]
    public GeneralAbility Ability { get; private set; }

    public Action(string owner, ActionType type, Vector2Int sourceTile, Vector2Int targetTile) {
        this.owner = owner;
        this.Type = type;
        this.OriginXPos = sourceTile.x;
        this.OriginYPos = sourceTile.y;
        this.TargetXPos = targetTile.x;
        this.TargetYPos = targetTile.y;
    }

    public Action(string owner, ActionType type, Vector2Int sourceTile, Vector2Int targetTile, GeneralAbility ability) {
        this.owner = owner;
        this.Type = type;
        this.OriginXPos = sourceTile.x;
        this.OriginYPos = sourceTile.y;
        this.TargetXPos = targetTile.x;
        this.TargetYPos = targetTile.y;
        this.Ability = ability;
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        this.Type = (ActionType)actionType;
    }

    [OnSerializing]
    public void OnSerializing(StreamingContext c) {
        actionType = (int)Type;
    }
}
