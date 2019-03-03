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
    private ActionType type;

    [DataMember]
    private int originXPos;
    [DataMember]
    private int originYPos;

    [DataMember]
    private int targetXPos;
    [DataMember]
    private int targetYPos;

    [DataMember]
    private int cardId;

    public Action(string owner, ActionType type, Vector2Int sourceTile, Vector2Int targetTile) {
        this.owner = owner;
        this.type = type;
        this.originXPos = sourceTile.x;
        this.originYPos = sourceTile.y;
        this.targetXPos = targetTile.x;
        this.targetYPos = targetTile.y;
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        type = (ActionType)actionType;
    }

    [OnSerializing]
    public void OnSerializing(StreamingContext c) {
        actionType = (int)type;
    }
}
