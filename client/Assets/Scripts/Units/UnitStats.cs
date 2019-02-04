using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class UnitStats{

    //an enum that represents the unit type
    UnitType unitType;

    //defense stats
    [DataMember]
    int currentHP;
    int maxHP;
    int armour;

    //offense stats
    int damage;
    int pierce;
    int range;
    int aoe;

    //mobility
    int movementSpeed;
    [DataMember]
    int x;
    [DataMember]
    int y;    
    private Vector2Int position;

    //others
    //a list of the card affects on the unit?

    //TODO: see if HexDistance() works for range finding
    //checks to make sure the target is in range
    public bool inRange(Vector2Int targetPoint){
//        if (HexUtility.HexDistance(this.geoPoint, targetPoint) < range)
//            return true;
//        else
            return true;
    }

    //sends a damage value, and this units type to the target of the attack
    public void attack(UnitStats target){
        if(inRange(target.position))
            target.Damaged(damage,this.pierce, this.unitType);
    }

    //TODO: apply type advantage
    //receives a damage value, and a unit type and calcualtes the damage taken
    public void Damaged(int damage, int pierce, UnitType type){
        //the final damage taken
        int finalDamage = damage;
        int effectiveArmour = this.armour;
        
        //if this unit is weak to the attaking unit, increase damage
        if (isWeak(type))
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

    public bool isWeak(UnitType type){
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
    public void Move(){

    }
}
