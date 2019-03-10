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
    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }
    public int Armour { get; set; }

    //offense stats
    public int Damage { get; set; }
    public int Pierce { get; set; }
    public int Range { get; set; }
    //Note: the aoe works as follows: 0 means "just hit the hex you're targeting"
    public int Aoe { get; private set; }
    public IAttackStrategy attackStrategy;

    //mobility
    public int MovementSpeed { get; set; }
    public Vector2Int Position { get; private set; }
    //xPos and yPos are the variables sent by the server, so we have to convert them to position
    [DataMember]
    private int xPos;
    [DataMember]
    private int yPos;

    //Variables for the generals
    public GeneralAbility Ability1 { get; private set; }
    [DataMember(Name = "ability1CoolDown", EmitDefaultValue = false)]
    public int Ability1Cooldown { get; set; }
    [DataMember(Name = "ability1Duration", EmitDefaultValue = false)]
    public int Ability1Duration { get; set; }

    public GeneralAbility Ability2 { get; private set; }
    [DataMember(Name = "ability2CoolDown", EmitDefaultValue = false)]
    public int Ability2Cooldown { get; set; }
    [DataMember(Name = "ability2Duration", EmitDefaultValue = false)]
    public int Ability2Duration { get; set; }

    public GeneralPassive Passive { get; private set; }

    //A reference to the script attached to the gameobject
    public Unit MyUnit { get; private set; }

    //Vision variables
    public int Vision { get; set; }

    //Variables used to dictate how much movements/attacks can be done on each turn
    public int AttackActions { get; set; } = 1;
    public int MovementActions { get; set; } = 1;

    //This constructor should mainly be used for testing purposes, so currentHp = maxHp
    public UnitStats(UnitType type, int maxHP, int armour, int range, int damage, int pierce, int aoe, int movementSpeed, int vision, int cost, IAttackStrategy attackStrategy) {
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
        this.Vision = vision;
    }

    public string GetDisplayName() {
        return UnitMetadata.ReadableNames[this.UnitType];
    }

    public void SetUnit(Unit unit) {
        this.MyUnit = unit;
    }

    //Returns a value based on the target
    public List<Tuple<Vector2Int, int>> Attack(Vector2Int target, bool specialMove = false) {
        if (!specialMove) {
            this.MovementActions = 0;
            this.AttackActions--;
        }
        return attackStrategy.Attack(this, target);
    }

    //A function to simply take an amount of damage, returning true if the unit dies, false otherwise
    public bool TakeDamage(int damage, int pierce) {
        int resistance = Armour - pierce;
        resistance = resistance < 0 ? 0 : resistance;
        damage -= resistance;
        damage = damage <= 0 ? 1 : damage;
        CurrentHP -= damage;
        return CurrentHP <= 0;
    }

    public void Heal(int amount) {
        this.CurrentHP += amount;
        CurrentHP = CurrentHP > MaxHP ? MaxHP : CurrentHP;
    }
    
    public void Kill() {
        MyUnit.Kill();
    }

    //Note: we don't need to update  xPos and yPos because that will be done when we send the data to the server
    public void Move(Vector2Int position, ref BoardController board, bool specialMove = false) {
        if(!specialMove)
            this.MovementActions--;
        this.Position = position;
        MyUnit.MoveTo(position,ref board);
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

    //Note: this list must have two abilities
    public void SetAbilities( List<GeneralAbility> abilityList) {
        this.Ability1 = abilityList[0];
        this.Ability2 = abilityList[1];
    }

    public void SetAbilities(List<GeneralAbility> abilityList, UnitStats serverUnit) {
        this.Ability1 = abilityList[0];
        this.Ability2 = abilityList[1];
        this.Ability1Cooldown = serverUnit.Ability1Cooldown <= 0 ? 0 : serverUnit.Ability1Cooldown - 1;
        this.Ability2Cooldown = serverUnit.Ability2Cooldown <= 0 ? 0 : serverUnit.Ability2Cooldown - 1;
        this.Ability2Duration = serverUnit.Ability2Duration <= 0 ? 0 : serverUnit.Ability2Duration - 1;
        this.Ability2Duration = serverUnit.Ability2Duration <= 0 ? 0 : serverUnit.Ability2Duration - 1;
    }

    public void SetPassive(GeneralPassive passive) {
        this.Passive = passive;
    }
}
