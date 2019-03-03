using UnityEngine;

//A general is a special unit with extra abilities
public class General : UnitStats {

    public General(UnitType type, int maxHP, int armour, int range, int damage, int pierce, int aoe, int movementSpeed, int cost, IAttackStrategy attackStrategy) : 
        base(type, maxHP, armour, range, damage, pierce, aoe, movementSpeed, cost, attackStrategy) {
        
    }
}
