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

    public Unit(int unitType, int health, Vector2Int geoPoint) {
        this.unitType = (UnitType)unitType;
        this.health = health;
        this.geoPoint = geoPoint;
    }
}
