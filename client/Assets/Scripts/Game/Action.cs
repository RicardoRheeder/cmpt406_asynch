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
    private Vector2Int sourceTile;

    [DataMember]
    private Vector2Int targetTile;

    [DataMember]
    private int cardId;

    public Action(string owner, ActionType type, Vector2Int sourceTile, Vector2Int targetTile) {
        this.owner = owner;
        this.type = type;
        this.sourceTile = sourceTile;
        this.targetTile = targetTile;
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
