using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Effect {

    [DataMember(Name = "owner")]
    public string Owner { get; private set; }

    [DataMember(Name = "type")]
    private int type;
    public EffectType Type { get; private set; }

    [DataMember(Name = "xPos")]
    private int xPos;
    [DataMember(Name = "yPos")]
    private int yPos;
    public Vector2Int target;

    [DataMember(Name = "durationLeft")]
    public int DurationLeft { get; private set; }

    public Effect(string owner, EffectType type, Vector2Int target, int duration) {
        this.Owner = owner;
        this.Type = type;
        this.type = (int)type;
        this.target = target;
        this.xPos = target.x;
        this.yPos = target.y;
        this.DurationLeft = duration;
    }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        this.Type = (EffectType)type;
        this.target = new Vector2Int(xPos, yPos);
    }
}
