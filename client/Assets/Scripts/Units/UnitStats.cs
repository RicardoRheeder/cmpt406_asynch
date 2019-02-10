using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class UnitStats {

    //an enum that represents the unit type
    UnitType unitType;

    //defense stats
    [DataMember]
    public int currentHP;
    public int maxHP;
    public int armour;

    //offense stats
    public int damage;
    public int pierce;
    public int range;
    public int aoe;

    //mobility
    public int movementSpeed;
    private Vector2Int position;
    //xPos and yPos are the variables sent by the server, so we have to convert them to position
    [DataMember]
    int xPos;
    [DataMember]
    int yPos;


    //Constructor
    public UnitStats(int currentHP, int maxHP, int armour, int range, int damage, int pierce, int aoe, int movementSpeed)
    {
        this.currentHP = currentHP;
        this.maxHP = maxHP;
        this.armour = armour;
        this.range = range;
        this.damage = damage;
        this.pierce = pierce;
        this.aoe = aoe;
        this.movementSpeed = movementSpeed;
    }
    //others
    //a list of the card affects on the unit?

    //TODO: see if HexDistance() works for range finding
    //checks to make sure the target is in range
    public bool InRange(Vector2Int targetPoint) {
//        if (HexUtility.HexDistance(this.geoPoint, targetPoint) < range)
//            return true;
//        else
            return true;
    }

    //sends a damage value, and this units type to the target of the attack
    public void Attack(UnitStats target) {
        if(InRange(target.position))
            target.TakeDamage(damage,this.pierce, this.unitType);
    }

    //TODO: apply type advantage
    //receives a damage value, and a unit type and calcualtes the damage taken
    public void TakeDamage(int damage, int pierce, UnitType type) {
        //the final damage taken
        int finalDamage = damage;
        int effectiveArmour = this.armour;
        
        //if this unit is weak to the attaking unit, increase damage
        if (IsWeak(type))
            //apply type advantage;

        //calculate armour
        effectiveArmour -= pierce;
        if (effectiveArmour < 0)
            effectiveArmour = 0;

        //apply armour to damage
        finalDamage -= effectiveArmour;

        //take the damage
        currentHP -= finalDamage;
    }

    public bool IsWeak(UnitType type) {
        //if this is a light, and attacker is heavy, return true
        if (this.unitType == UnitType.light && type == UnitType.heavy )
            return true;
        //if this is a heavy, and attacker is pierce, return true
        else if (this.unitType == UnitType.heavy && type == UnitType.pierce)
            return true;
        //if this is a pierce, and attacker is light, return true
        else if (this.unitType == UnitType.pierce && type == UnitType.light)
            return true;
        //else, return false
        else
            return false;
    }

    //TODO
    public void Move() {

    }

    //We need to convert the xPos and yPos variables to be Position
    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context) {
        position = new Vector2Int(xPos, yPos);
    }

    [OnSerializing()]
    internal void OnSerializingMethod(StreamingContext context) {
        xPos = position.x;
        yPos = position.y;
    }
}
