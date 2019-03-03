using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class UnitStats {

    //General Unit information
    [DataMember(Name = "unitType")]
    private int serverUnitType = -1;
    public UnitType UnitType { get; private set; }
    public UnitClass UnitClass { get; private set; }
    public int Cost { get; private set; }
    [DataMember(Name = "owner", IsRequired = true)]
    public string Owner { get; set; }

    //defense stats
    [DataMember(Name = "health")]
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    public int Armour { get; private set; }

    //offense stats
    public int Damage { get; private set; }
    public int Pierce { get; private set; }
    public int Range { get; private set; }
    //Note: the aoe works as follows: 0 means "just hit the hex you're targeting"
    public int Aoe { get; private set; }
    private IAttackStrategy attackStrategy;

    //mobility
    public int MovementSpeed { get; private set; }
    public Vector2Int Position { get; private set; }
    //xPos and yPos are the variables sent by the server, so we have to convert them to position
    [DataMember]
    private int xPos;
    [DataMember]
    private int yPos;

    public Unit MyUnit { get; private set; }

    //This constructor should mainly be used for testing purposes, so currentHp = maxHp
    public UnitStats(UnitType type, int maxHP, int armour, int range, int damage, int pierce, int aoe, int movementSpeed, int cost, IAttackStrategy attackStrategy) {
        this.UnitType = type;
        this.serverUnitType = (int)type;
        this.CurrentHP = maxHP;
        this.MaxHP = maxHP;
        this.Armour = armour;
        this.Range = range;
        this.Damage = damage;
        this.Pierce = pierce;
        this.Aoe = aoe;
        this.MovementSpeed = movementSpeed;
        this.Cost = cost;
        this.attackStrategy = attackStrategy;
        this.UnitClass = UnitMetadata.UnitAssociations[UnitType];
    }

    public string GetDisplayName() {
        return UnitMetadata.ReadableNames[this.UnitType];
    }

    public void SetUnit(Unit unit) {
        this.MyUnit = unit;
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
    public List<Tuple<Vector2Int, int>> Attack(Vector2Int target) {
        return attackStrategy.Attack(this, target);
    }

    //A function to simply take an amount of damage, returning true if the unit dies, false otherwise
    public bool TakeDamage(int damage, int pierce) {
        int resistance = Armour - pierce;
        resistance = resistance < 0 ? 0 : resistance;
        damage -= resistance;
        damage = damage <= 0 ? 1 : damage;
        CurrentHP -= damage;
        return CurrentHP >= 0;
    }

    public void Heal(int amount) {
        this.CurrentHP += amount;
        CurrentHP = CurrentHP > MaxHP ? MaxHP : CurrentHP;
    }

    //Note: we don't need to update  xPos and yPos because that will be done when we send the data to the server
    public void Move(Vector2Int position, ref BoardController board) {
        this.Position = position;
        MyUnit.transform.position = board.CellToWorld(position);
    }

    //We need to convert the xPos and yPos variables to be Position
    //We also need to get a base unit and copy over the stats that weren't stored on the server.
    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context) {
        this.Position = new Vector2Int(xPos, yPos);
        UnitType = (UnitType)serverUnitType;
        UnitStats baseUnit = UnitFactory.GetBaseUnit(UnitType);
        this.Cost = baseUnit.Cost;
        this.MaxHP = baseUnit.MaxHP;
        this.Armour = baseUnit.Armour;
        this.Damage = baseUnit.Damage;
        this.Pierce = baseUnit.Pierce;
        this.Range = baseUnit.Range;
        this.Aoe = baseUnit.Aoe;
        this.MovementSpeed = baseUnit.MovementSpeed;
    }

    //Note: the unit type will never change so we don't have to update the int value
    [OnSerializing()]
    internal void OnSerializingMethod(StreamingContext context) {
        xPos = Position.x;
        yPos = Position.y;
    }
}
