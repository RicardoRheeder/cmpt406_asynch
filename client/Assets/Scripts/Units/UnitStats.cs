using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class UnitStats {

    //General Unit information
    [DataMember]
    int serverUnitType = -1;
    UnitType unitType;
    int cost;
    [DataMember]
    string owner;

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

    //This constructor should mainly be used for testing purposes, so currentHp = maxHp
    public UnitStats(UnitType type, int maxHP, int armour, int range, int damage, int pierce, int aoe, int movementSpeed, int cost) {
        this.unitType = type;
        this.serverUnitType = (int)type;
        this.currentHP = maxHP;
        this.maxHP = maxHP;
        this.armour = armour;
        this.range = range;
        this.damage = damage;
        this.pierce = pierce;
        this.aoe = aoe;
        this.movementSpeed = movementSpeed;
        this.cost = cost;
    }

    //TODO: see if HexDistance() works for range finding
    //checks to make sure the target is in range
    public bool InRange(Vector2Int targetPoint) {
//        if (HexUtility.HexDistance(this.geoPoint, targetPoint) < range)
//            return true;
//        else
            return true;
    }

    //Returns a value based on the target
    public int Attack(UnitStats target) {
        return System.Convert.ToInt32(this.damage * UnitMetadata.GetMultiplier(this.unitType, target.unitType));
    }

    //A function to simply take an amount of damage, returning true if the unit dies, false otherwise
    public bool TakeDamage(int damage) {
        damage -= armour;
        damage = damage <= 0 ? 1 : damage;
        currentHP -= damage;
        return currentHP >= 0;
    }

    //TODO
    //Note: the game manager will actually move the unit if valid. This method will basically be to handle animations and thats it.
    public void Move() {

    }

    //We need to convert the xPos and yPos variables to be Position
    //We also need to get a base unit and copy over the stats that weren't stored on the server.
    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context) {
        position = new Vector2Int(xPos, yPos);
        unitType = (UnitType)serverUnitType;
        UnitStats baseUnit = UnitFactory.GetBaseUnit(unitType);
        this.cost = baseUnit.cost;
        this.maxHP = baseUnit.maxHP;
        this.armour = baseUnit.armour;
        this.damage = baseUnit.damage;
        this.pierce = baseUnit.pierce;
        this.range = baseUnit.range;
        this.aoe = baseUnit.aoe;
        this.movementSpeed = baseUnit.movementSpeed;
    }

    //Note: the unit type will never change so we don't have to update the int value
    [OnSerializing()]
    internal void OnSerializingMethod(StreamingContext context) {
        xPos = position.x;
        yPos = position.y;
    }
}
