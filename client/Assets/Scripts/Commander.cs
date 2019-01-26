using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Commander {

    [DataMember]
    private CommanderType commanderType;

    [DataMember]
    private int health;

    [DataMember]
    private Vector2Int geoPoint;

    [DataMember]
    private Dictionary<string, int> coolDowns;

    public Commander(int commanderType, int health, Vector2Int geoPoint, Dictionary<string, int> coolDowns) {
        this.commanderType = (CommanderType)commanderType;
        this.health = health;
        this.geoPoint = geoPoint;
        this.coolDowns = coolDowns;
    }

}
