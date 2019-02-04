using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public abstract class Unit {

    int maxHealth;

    [DataMember]
    int currentHealth;

    int armourValue;
    int armourPenetration;
    int damageValue;
    int attackRange;
    int attackAoe;
    int movementSpeed;

    [DataMember]
    int xPos;

    [DataMember]
    int yPos;

    Vector2Int position;

    [OnDeserializing()]
    internal void OnDeserializingMethod(StreamingContext context) {
        xPos = position.x;
        yPos = position.y;
    }

    [OnSerializing()]
    internal void OnSerializingMethod(StreamingContext context) {
        position = new Vector2Int(xPos, yPos);
    }

    public bool TakeDamage(int damage) {
        return false;
    }

    public void Attack(Vector2Int target) {

    }

    public void Move(Vector2Int target) {

    }

}
