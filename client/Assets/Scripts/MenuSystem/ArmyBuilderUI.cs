using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
public class ArmyBuilderUI : MonoBehaviour {

    //References to the used APIs
    private Client client;
    private AudioManager audioManager;

    private UnitType selectedUnit;
    public ArmyPreset selectedArmy;

    //References to the in game panel we populate with unit names
    [SerializeField]
    private GameObject armyListView;
    [SerializeField]
    private GameObject armyListCellPrefab; //Note: this can be a button where we apply a dynamic listener that removes it from the army
    [SerializeField]
    private TMP_InputField armyName;

    //References to texts we update
    [SerializeField]
    private TMP_Text stockNum;
    [SerializeField]
    private TMP_Text healthNum;
    [SerializeField]
    private TMP_Text attackNum;
    [SerializeField]
    private TMP_Text armourNum;
    [SerializeField]
    private TMP_Text rangeNum;
    [SerializeField]
    private TMP_Text pierceNum;
    [SerializeField]
    private TMP_Text aoeNum;
    [SerializeField]
    private TMP_Text movementNum;
    [SerializeField]
    private TMP_Text unitCostNum;
    [SerializeField]
    private TMP_Text unitName;
    [SerializeField]
    private Text cardName1;
    [SerializeField]
    private Text cardEffects1;
    [SerializeField]
    private Text cardName2;
    [SerializeField]
    private Text cardEffects2;
    [SerializeField]
    private Image typeImage;
    [SerializeField]
    private Button trooper;
    [SerializeField]
    private Button recon;
    [SerializeField]
    private Button steamer;
    [SerializeField]
    private Button pewpew;
    [SerializeField]
    private Button compensator;
    [SerializeField]
    private Button foundation;
    [SerializeField]
    private Button claymore;
    [SerializeField]
    private Button midas;
    [SerializeField]
    private Button powerSurge;
    [SerializeField]
    private Button albarn;
    [SerializeField]
    private Button tungsten;
    [SerializeField]
    private Button sandman;
    [SerializeField]
    private Button adren;

    [SerializeField]
    private GameObject addUnitButton;

    [SerializeField]
    private Sprite light;
    [SerializeField]
    private Sprite piercing;
    [SerializeField]
    private Sprite heavy;
    [SerializeField]
    private Sprite support;
    [SerializeField]
    private Sprite general;

    public void Awake() {
        client = GameObject.Find("Networking").GetComponent<Client>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        addUnitButton.SetActive(false);

        ConfigureOnClick(trooper, UnitType.trooper);
        ConfigureOnClick(recon, UnitType.recon);
        ConfigureOnClick(steamer, UnitType.steamer);
        ConfigureOnClick(pewpew, UnitType.pewpew);
        ConfigureOnClick(compensator, UnitType.compensator);
        ConfigureOnClick(foundation, UnitType.foundation);
        ConfigureOnClick(claymore, UnitType.claymore);
        ConfigureOnClick(midas, UnitType.midas);
        ConfigureOnClick(powerSurge, UnitType.powerSurge);
        ConfigureOnClick(albarn, UnitType.heavy_albarn);
        ConfigureOnClick(tungsten, UnitType.piercing_tungsten);
        ConfigureOnClick(adren, UnitType.light_adren);
        ConfigureOnClick(sandman, UnitType.support_sandman);
    }

    private void ConfigureOnClick(Button button, UnitType type) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            selectedUnit = type;
            UpdateDisplay();
            addUnitButton.SetActive(true);
            audioManager.Play(SoundName.ButtonPress);
        });
    }

    public void CreateArmy() {
        addUnitButton.SetActive(false);
        string newArmyName = "Name Army";
        ArmyPreset newPreset = new ArmyPreset(
            newArmyName,
            new List<int>(),
            (int)UnitType.piercing_tungsten
        );

        armyName.text = "";
        selectedArmy = newPreset;
        GetCost();
        DeleteUnitsHelper();
        ShowArmy();
    }

    public void UpdateDisplay() {
        unitName.GetComponent<TextMeshProUGUI>().SetText(UnitMetadata.ReadableNames[selectedUnit]);

        UnitStats baseUnit = UnitFactory.GetBaseUnit(selectedUnit);
        healthNum.SetText(baseUnit.MaxHP.ToString());
        attackNum.SetText(baseUnit.Damage.ToString());
        armourNum.SetText(baseUnit.Armour.ToString());
        rangeNum.SetText(baseUnit.Range.ToString());
        pierceNum.SetText(baseUnit.Pierce.ToString());
        aoeNum.SetText(baseUnit.Aoe.ToString());
        movementNum.SetText(baseUnit.MovementSpeed.ToString());
        unitCostNum.SetText(baseUnit.Cost.ToString());

        switch (baseUnit.UnitType){
            case UnitType.claymore:
                cardName1.text = ("Oil Slick");
                cardEffects1.text = ("+1 Movement Action\nEnemies within 1 range of the Claymore cannot move until your next turn");
                cardName2.text = ("Poppin' Smoke");
                cardEffects2.text = ("Enemies within 2 range of the Claymore have their vision reduced to 0 until your next turn");
                typeImage.sprite = support;
                break;
                
            case UnitType.compensator:
                cardName1.text = ("Over Compensation");
                cardEffects1.text = ("+30 Armor Penetration\n+2 Movement Speed\n+10 Attack Damage");
                cardName2.text = ("Size Matters");
                cardEffects2.text = ("+40 Armor Penetration\n+2 Range");
                typeImage.sprite = piercing;
                break;
                
            case UnitType.foundation:
                cardName1.text = ("Both Barrels");
                cardEffects1.text = ("Double Armor Penetration\nDouble Attack Damage");
                cardName2.text = ("To The Skies");
                cardEffects2.text = ("+8 Range\nYour Foundations attacks deal area damage within one tile of your target for this turn");
                typeImage.sprite = piercing;
                break;
                
            case UnitType.midas:
                cardName1.text = ("Deploy Weld-A-Tron 3000");
                cardEffects1.text = ("Heal all allies within 1 range of the M.I.D.A.S for 30 Hit Points");
                cardName2.text = ("Overclocked");
                cardEffects2.text = ("The M.I.D.A.S gains 3 additional attacks for this turn");
                typeImage.sprite = support;
                break;
                
            case UnitType.pewpew:
                cardName1.text = ("Anti-Ballistic Shield Matrix");
                cardEffects1.text = ("+10 armor for adjacent allies");
                cardName2.text = ("PewPew's Revenge");
                cardEffects2.text = ("+2 Movement Speed\n+10 Attack Damage\n+1 Attack Action");
                typeImage.sprite = heavy;
                break;
                
            case UnitType.powerSurge:
                cardName1.text = ("Death Ball");
                cardEffects1.text = ("+10 Armor to allies within 2 range\n+10 Attack Damage to allies within 2 range");
                cardName2.text = ("High Ground Simulator");
                cardEffects2.text = ("+10 Attack Damage to adjacent allies\n+1 Movement Speed to adjacent allie\n+1 Range to adjacent allies");
                typeImage.sprite = support;
                break;
                
            case UnitType.recon:
                cardName1.text = ("Eagle Aspect");
                cardEffects1.text = ("+3 Range\n+2 Movement Speed");
                cardName2.text = ("Noob Tube");
                cardEffects2.text = ("+15 Attack Damage\nThe Recon's attacks deal area damage within 1 range of your target for this turn");
                typeImage.sprite = light;
                break;
                
            case UnitType.steamer:
                cardName1.text = ("Real Steel");
                cardEffects1.text = ("+20 Armor\n-1 Movement Speed");
                cardName2.text = ("We Get It... You Vape");
                cardEffects2.text = ("The Steamer's attacks deal damage in a 3 tile cone for this turn");
                typeImage.sprite = heavy;
                break;
                
            case UnitType.trooper:
                cardName1.text = ("On The Juice");
                cardEffects1.text = ("+10 Attack Damage\n+10 Armor Penetration");
                cardName2.text = ("Run'n'Gun");
                cardEffects2.text = ("+2 Movement Speed\n+1 Movement Action\n+15 Attack Damage");
                typeImage.sprite = light;
                break;
            
            case UnitType.light_adren:
                cardName1.text = ("Stick and Poke");
                cardEffects1.text = ("Cooldown: 5 turns\nYour units gain 3 Movement Speed and units add their default speed for 2 turns");
                cardName2.text = ("Deep Penetration");
                cardEffects2.text = ("Cooldown: 3 turns\nChoose a unit. The unit's Armor is set to 0 for this turn");
                typeImage.sprite = general;
                break;
                
            case UnitType.piercing_tungsten:
                cardName1.text = ("Trojan Shot");
                cardEffects1.text = ("Cooldown: 6 turns\nFor this turn Tungsten's Range becomes 20 and his attack hits all units between him and his target");
                cardName2.text = ("Armor Piercing Ammo");
                cardEffects2.text = ("Cooldown: 6 turns\nSelect a unit within 10 range of Tungsten. This unit gains 10 Armor Penetration for 4 turns");
                typeImage.sprite = general;
                break;
            
            case UnitType.heavy_albarn:
                cardName1.text = ("The Best Offense");
                cardEffects1.text = ("Cooldown: 2 turns\nChoose a unit. The unit's Armor is added to its Attack Damage");
                cardName2.text = ("Steam Overload");
                cardEffects2.text = ("Cooldown: 3 turns\nChoose a location within 10 Range of Albarn. The target takes 30 damage, units within 1 range take 20, units within 2 range take 10");
                typeImage.sprite = general;
                break;
            
            case UnitType.support_sandman:
                cardName1.text = ("Sahara Mine");
                cardEffects1.text = ("Cooldown: 4 turns\nChoose a target within 2 Range of Sandman. All units within 1 range of the target take 10 damage and lose 10 Armor for this turn");
                cardName2.text = ("Sandstorm");
                cardEffects2.text = ("Cooldown: 4 turns\nAll units within 2 range of Sandman have their Vision and Movement Speed reduced by 3. The sandstorm lasts for 2 turns");
                typeImage.sprite = general;
                break;
        }
    }

    public void AddUnit() {
        //if it is a unit, add it to the army
        if ((int)selectedUnit < UnitMetadata.GENERAL_THRESHOLD) {//generals are above 100
            selectedArmy.AddUnit(selectedUnit);
        }
        else {
            selectedArmy.ReplaceGeneral((int)selectedUnit);
        }
        GetCost();
        ShowArmy();
    }

    public void DeleteUnit() {
        //only remove the unit if it is not a general
        //a general must be in an army
        if ((int)selectedUnit < UnitMetadata.GENERAL_THRESHOLD) {
            selectedArmy.RemoveUnit(selectedUnit);
            GetCost();
            ShowArmy();
        }
    }

    public void GetCost() {
        stockNum.SetText(UnitFactory.CalculateCost(selectedArmy.Units).ToString());
    }

    public void DeleteArmy() {
        client.RemoveArmyPreset(selectedArmy.Id);
    }

    private void ShowArmy() {
        //delete list first
        foreach (Transform child in armyListView.transform) {
            if (child.transform.name != "Content") {
                Destroy(child.gameObject);
            }
        }

        //display the general
        GameObject generalText = Instantiate(armyListCellPrefab);
        generalText.GetComponent<TMP_Text>().SetText(UnitMetadata.ReadableNames[(UnitType)selectedArmy.General]);
        generalText.transform.SetParent(armyListView.transform, false);

        //display the army
        for (int i = 0; i < selectedArmy.Units.Count; i++) {

            GameObject unitText = Instantiate(armyListCellPrefab);
            unitText.GetComponent<TMP_Text>().SetText(UnitMetadata.ReadableNames[(UnitType)selectedArmy.Units[i]]);
            unitText.transform.SetParent(armyListView.transform, false);
        }
    }

    public void DeleteUnitsHelper() {
        foreach (Transform child in armyListView.transform) {
            if (child.transform.name != "Content")
                Destroy(child.gameObject);   
        }
    }
    
    public void ChangeFilter() {
        var colors = recon.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        recon.colors = colors;
        
        colors = trooper.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        trooper.colors = colors;
        
        colors = steamer.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        steamer.colors = colors;
        
        colors = pewpew.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        pewpew.colors = colors;
        
        colors = compensator.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        compensator.colors = colors;
        
        colors = foundation.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        foundation.colors = colors;
        
        colors = claymore.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        claymore.colors = colors;
        
        colors = powerSurge.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        powerSurge.colors = colors;
        
        colors = midas.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        midas.colors = colors;
        
        colors = adren.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        adren.colors = colors;
        
        colors = albarn.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        albarn.colors = colors;
        
        colors = tungsten.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        tungsten.colors = colors;
        
        colors = sandman.colors;
        colors.normalColor = ColourConstants.BUTTON_DEFAULT;
        sandman.colors = colors;
    }
    
    public void FilterLight() {
        var colors = recon.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        recon.colors = colors;
        
        colors = trooper.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        trooper.colors = colors;
        
        colors = adren.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        adren.colors = colors;
    }
    
    public void FilterPiercing() {
        var colors = compensator.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        compensator.colors = colors;
        
        colors = foundation.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        foundation.colors = colors;
        
        colors = tungsten.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        tungsten.colors = colors;
    }
    
    public void FilterHeavy() {
        var colors = steamer.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        steamer.colors = colors;
        
        colors = pewpew.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        pewpew.colors = colors;
        
        colors = albarn.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        albarn.colors = colors;
    }
    
    public void FilterSupport() {
        var colors = midas.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        midas.colors = colors;
        
        colors = claymore.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        claymore.colors = colors;
        
        colors = powerSurge.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        powerSurge.colors = colors;
        
        colors = sandman.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        sandman.colors = colors;
    }
    
    public void FilterGeneral() {
        var colors = albarn.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        albarn.colors = colors;
        
        colors = adren.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        adren.colors = colors;
        
        colors = tungsten.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        tungsten.colors = colors;
        
        colors = sandman.colors;
        colors.normalColor = ColourConstants.BUTTON_ACTIVE;
        sandman.colors = colors;
    }
}
