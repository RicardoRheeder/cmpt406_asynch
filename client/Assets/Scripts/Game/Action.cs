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

    [DataMember(Name="originXPos")]
    public int OriginXPos{get; private set;}
    [DataMember(Name="originYPos")]
    public int OriginYPos{get; private set;}

    [DataMember(Name="targetXPos")]
    public int TargetXPos{get; private set;}
    [DataMember(Name="targetYPos")]
    public int TargetYPos{get; private set;}

    [DataMember]
    private int cardId;

    public Action(string owner, ActionType type, Vector2Int sourceTile, Vector2Int targetTile) {
        this.owner = owner;
        this.Type = type;
        this.OriginXPos = sourceTile.x;
        this.OriginYPos = sourceTile.y;
        this.TargetXPos = targetTile.x;
        this.TargetYPos = targetTile.y;
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        Type = (ActionType)actionType;
    }

    [OnSerializing]
    public void OnSerializing(StreamingContext c) {
        actionType = (int)Type;
    }
}
