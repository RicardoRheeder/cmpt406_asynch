using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Unit {

    [DataMember]
    private UnitType unitType;

    [DataMember]
    private int health;

    [DataMember]
    private Vector2Int geoPoint;

    [DataMember]
    private string owner;

    public Unit(int unitType, int health, Vector2Int geoPoint, string owner) {
        this.unitType = (UnitType)unitType;
        this.health = health;
        this.geoPoint = geoPoint;
        this.owner = owner;
    }
}
