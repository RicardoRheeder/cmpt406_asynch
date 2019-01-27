using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class General {

    [DataMember]
    private GeneralType generalType;

    [DataMember]
    private int health;

    [DataMember]
    private Vector2Int geoPoint;

    [DataMember]
    private Dictionary<string, int> coolDowns;

    public General(int generalType, int health, Vector2Int geoPoint, Dictionary<string, int> coolDowns) {
        this.generalType = (GeneralType)generalType;
        this.health = health;
        this.geoPoint = geoPoint;
        this.coolDowns = coolDowns;
    }

}
