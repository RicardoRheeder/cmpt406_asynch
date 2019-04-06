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
	public UnitHUD unitHUD;

    //defense stats
    [DataMember(Name = "health")]
    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }
    public int Armour { get; set; }

    //offense stats
    public int Damage { get; set; }
    private int previousDamage;
    public int Pierce { get; set; }
    public int Range { get; set; }
    //Note: the aoe works as follows: 0 means "just hit the hex you're targeting"
    public int Aoe { get; set; }
    public IAttackStrategy attackStrategy;
    private bool moveAfterAttack = false;

    //mobility
    public int MovementSpeed { get; set; }
    public int DefaultSpeed { get; set; }
    public Vector2Int Position { get; private set; }
    //xPos and yPos are the variables sent by the server, so we have to convert them to position
    [DataMember]
    private int xPos;
    [DataMember]
    private int yPos;

    [DataMember (Name = "direction")]
    public int Direction { get; set; }

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

    //This constructor should mainly be used for testing purposes, so currentHp = maxHp
    public UnitStats(UnitType type, int maxHP, int armour, int range, int damage, int pierce, int aoe, int movementSpeed, int vision, int cost, int direction, IAttackStrategy attackStrategy) {
        this.UnitType = type;
        this.serverUnitType = (int)type;
        this.CurrentHP = maxHP;
        this.MaxHP = maxHP;
        this.Armour = armour;
        this.Range = range;
        this.Damage = damage;
        previousDamage = damage;
        this.Pierce = pierce;
        this.Aoe = aoe;
        this.MovementSpeed = movementSpeed;
        this.DefaultSpeed = movementSpeed;
        this.Cost = cost;
        this.attackStrategy = attackStrategy;
        this.UnitClass = UnitMetadata.UnitAssociations[UnitType];
        this.Vision = vision;
        this.Direction = direction;
    }
	
    public string GetDisplayName() {
        return UnitMetadata.ReadableNames[this.UnitType];
    }

    public void SetUnit(Unit unit) {
        this.MyUnit = unit;
    }

    //Returns a value based on the target
    public List<Tuple<Vector2Int, int>> Attack(Vector2Int target, Vector3 targetWorldPos, AudioManager audioManager = null, bool specialMove = false) {
        if (!specialMove) {
            if(!moveAfterAttack)
                this.MovementSpeed = 0;
            this.AttackActions--;
        }
        if (MyUnit != null) {
            MyUnit.Attack(targetWorldPos, UnitType, audioManager);
        }
        else {
            Debug.LogError("MyUnit is NULL!");
        }
        return attackStrategy.Attack(this, target);
    }

    //A function to simply take an amount of damage, returning true if the unit dies, false otherwise
    public bool TakeDamage(int damage, int pierce) {
        if (MyUnit != null) {MyUnit.GetHit();} else {Debug.LogError("MyUnit is NULL!");}
        int resistance = Armour - pierce;
        resistance = resistance < 0 ? 0 : resistance;
        damage -= resistance;
        damage = damage <= 0 ? 1 : damage;
        CurrentHP -= damage;
		FloatingTextController.CreateFloatingText(damage.ToString(), MyUnit.transform, false);
        if (unitHUD != null)
            unitHUD.SetHPText(CurrentHP.ToString());
        return CurrentHP <= 0;
    }

    public void TakeCardDamage(int damage) {
        CurrentHP -= damage;
        CurrentHP = CurrentHP <= 0 ? 1 : CurrentHP;
        if(unitHUD != null)
    		unitHUD.SetHPText(CurrentHP.ToString());
		FloatingTextController.CreateFloatingText(damage.ToString(), MyUnit.transform, false);
    }

    public void Heal(int amount) {
        this.CurrentHP += amount;
        CurrentHP = CurrentHP > MaxHP ? MaxHP : CurrentHP;
        if (unitHUD != null)
            unitHUD.SetHPText(CurrentHP.ToString());
		FloatingTextController.CreateFloatingText(amount.ToString(), MyUnit.transform, true);
    }
    
    public void Kill(AudioManager audioManager = null) {
        MyUnit.Kill(UnitType, audioManager);
        if (unitHUD != null)
            unitHUD.DestroyThis();
    }

    //Methods to alter stats
    public void AlterDamage(int change) {
        if(this.Damage < 0 ) {
            this.Damage -= change;
            this.Damage = this.Damage > 0 ? 0 : this.Damage;
        }
        else if(this.Damage > 0 ) {
            this.Damage += change;
            this.Damage = this.Damage < 0 ? 0 : this.Damage;
        }
        else {
            if((previousDamage > 0 && change > 0) || (previousDamage < 0 && change < 0)) {
                this.Damage = change;
            }
        }
        if (unitHUD != null)
            unitHUD.SetDMGText(Damage.ToString());
    }

    public void AlterSpeed(int change) {
        if(this.DefaultSpeed != 0) {
            this.MovementSpeed += change;
            this.MovementSpeed = this.MovementSpeed < 0 ? 0 : this.MovementSpeed;
        }
    }

    public void AlterVision(int change) {
        this.Vision += change;
        this.Vision = this.Vision < 0 ? 0 : this.Vision;
    }

    public void AlterPierce(int change) {
        this.Pierce += change;
        this.Pierce = this.Pierce < 0 ? 0 : this.Pierce;
        if (unitHUD != null)
            unitHUD.SetPENText(Pierce.ToString());
    }

    public void AlterArmour(int change) {
        this.Armour += change;
        this.Armour = this.Armour < 0 ? 0 : this.Armour;
        if (unitHUD != null)
            unitHUD.SetARText(Armour.ToString());
    }

    public void AlterRange(int change) {
        if(!attackStrategy.GetType().Equals(typeof(CleaveStrategy))) { //if we have a cleave strategy, we want the range to be 1
            this.Range += change;
            this.Range = this.Range < 1 ? 1 : this.Range;
        }
    }

    public void AlterMoveAfterAttack() {
        this.moveAfterAttack = true;
    }

    public void AlterAttackType(IAttackStrategy strategy) {
        if(strategy.GetType().Equals(typeof(CleaveStrategy))) { //if we are a cleave unit, our range should always be 1
            this.Range = 1;
        }
        this.attackStrategy = strategy;
    }

    public void DoublePierce() {
        this.Pierce = this.Pierce * 2;
        if (unitHUD != null)
            unitHUD.SetPENText(Pierce.ToString());
    }

    public void DoubleDamage() {
        this.Damage = this.Damage * 2;
        if (unitHUD != null)
            unitHUD.SetDMGText(Damage.ToString());
    }

    public void DoubleSpeed() {
        this.MovementSpeed = 2 * this.MovementSpeed;
    }

    //Note: we don't need to update  xPos and yPos because that will be done when we send the data to the server
    public void Move(Vector2Int position, ref BoardController board, AudioManager audioManager, bool specialMove = false) {
        if (audioManager != null) {
            audioManager.Play(UnitType, SoundType.Move);
        }
        List<Tuple<Vector2Int,int>> pathWithDirection = HexUtility.PathfindingWithDirection(this.Position,position,board.GetTilemap(),false);
        MyUnit.MoveAlongPath(pathWithDirection,ref board);
        this.MovementSpeed -= pathWithDirection.Count;
        this.Position = position;
        if(pathWithDirection.Count > 0) {
            this.Direction = pathWithDirection[pathWithDirection.Count - 1].Second;
        }
    }

    public void Place(Vector2Int position, ref BoardController board) {
        this.Position = position;
        MyUnit.PlaceAt(position, ref board);
    }
	
    public void SandboxMove(Vector2Int position, ref BoardController board, AudioManager audioManager, bool specialMove = false){
        if (audioManager != null) {
            audioManager.Play(UnitType, SoundType.Move);
        }
        List<Tuple<Vector2Int,int>> pathWithDirection = HexUtility.PathfindingWithDirection(this.Position,position,board.GetTilemap(),false);
        MyUnit.MoveAlongPath(pathWithDirection,ref board);
        this.Position = position;
        if(pathWithDirection.Count > 0) {
            this.Direction = pathWithDirection[pathWithDirection.Count - 1].Second;
        }
    }

    //Note: this list must have two abilities
    public void SetAbilities(List<GeneralAbility> abilityList) {
        this.Ability1 = abilityList[0];
        this.Ability2 = abilityList[1];
    }

    public void SetAbilities(List<GeneralAbility> abilityList, UnitStats serverUnit, string username) {
        this.Ability1 = abilityList[0];
        this.Ability2 = abilityList[1];
        this.Ability1Cooldown = serverUnit.Ability1Cooldown;
        this.Ability2Cooldown = serverUnit.Ability2Cooldown;
        this.Ability2Duration = serverUnit.Ability2Duration;
        this.Ability2Duration = serverUnit.Ability2Duration;
    }

    public void SetPassive(GeneralPassive passive) {
        this.Passive = passive;
    }

    public void Select(AudioManager manager) {
        if(Random.Range(0, 4) == 0) {
            manager.Play(UnitType, SoundType.Annoyed, isVoice: true);
        }
        else {
            manager.Play(UnitType, SoundType.Select, isVoice: true);
        }
    }

    public void PlayAttackVoice(AudioManager manager) {
        manager.Play(UnitType, SoundType.Attack, isVoice: true);
    }

    public void PlayMoveVoice(AudioManager manager) {
        manager.Play(UnitType, SoundType.Move, isVoice: true);
    }

    public void PlayAbilityVoice(AudioManager manager) {
        manager.Play(UnitType, SoundType.Ability, isVoice: true);
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
        previousDamage = baseUnit.Damage;
        this.Pierce = baseUnit.Pierce;
        this.Range = baseUnit.Range;
        this.Aoe = baseUnit.Aoe;
        this.MovementSpeed = baseUnit.MovementSpeed;
        this.DefaultSpeed = baseUnit.MovementSpeed;
    }

    //Note: the unit type will never change so we don't have to update the int value
    [OnSerializing()]
    internal void OnSerializingMethod(StreamingContext context) {
        xPos = Position.x;
        yPos = Position.y;
    }
}
