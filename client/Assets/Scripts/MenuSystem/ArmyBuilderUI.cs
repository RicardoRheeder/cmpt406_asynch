using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmyBuilderUI : MonoBehaviour {

    //References to the used APIs
    private Client client;
    private AudioManager audioManager;

    //the current unit being looked at
    public UnitType selectedUnit;

    //The army we are creating/editing
    public ArmyPreset selectedArmy;

    //References to the in game panel we populate with unit names
    public GameObject armyContent;
    public GameObject friendsListCellPrefab; //Note: this can be a button where we apply a dynamic listener that removes it from the army
    public TMP_InputField armyName;

    //References to texts we update
    private TMP_Text stockNum;
    private TMP_Text healthNum;
    private TMP_Text attackNum;
    private TMP_Text armourNum;
    private TMP_Text rangeNum;
    private TMP_Text pierceNum;
    private TMP_Text aoeNum;
    private TMP_Text movementNum;
    private TMP_Text unitCostNum;
    private TMP_Text unitName;
    private Text cardName1;
    private Text cardEffects1;
    private Text cardName2;
    private Text cardEffects2;
    private Image typeImage;
    private Button trooper;
    private Button recon;
    private Button steamer;
    private Button pewpew;
    private Button compensator;
    private Button foundation;
    private Button claymore;
    private Button midas;
    private Button powerSurge;
    private Button albarn;
    private Button tungsten;
    private Button sandman;
    private Button adren;
    private Color highlight;
    private Color fade;

    private GameObject addUnitButton;
    
    public new Sprite light;
    public Sprite piercing;
    public Sprite heavy;
    public Sprite support;
    public Sprite general;

    public void Awake() {
        client = GameObject.Find("Networking").GetComponent<Client>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        armyContent = GameObject.Find("ABListViewport");
        armyName = GameObject.Find("ABNameInput").GetComponent<TMP_InputField>();
        stockNum = GameObject.Find("CostNum").GetComponent<TMP_Text>();
        healthNum = GameObject.Find("HealthNum").GetComponent<TMP_Text>();
        attackNum = GameObject.Find("AttackNum").GetComponent<TMP_Text>();
        armourNum = GameObject.Find("ArmorNum").GetComponent<TMP_Text>();
        rangeNum = GameObject.Find("RangeNum").GetComponent<TMP_Text>();
        pierceNum = GameObject.Find("PierceNum").GetComponent<TMP_Text>();
        aoeNum = GameObject.Find("AOENum").GetComponent<TMP_Text>();
        movementNum = GameObject.Find("MovementNum").GetComponent<TMP_Text>();
        unitCostNum = GameObject.Find("UnitCostNum").GetComponent<TMP_Text>();
        unitName = GameObject.Find("UnitName").GetComponent<TMP_Text>();
        cardName1 = GameObject.Find("cardName1").GetComponent<Text>();
        cardEffects1 = GameObject.Find("cardEffects1").GetComponent<Text>();
        cardName2 = GameObject.Find("cardName2").GetComponent<Text>();
        cardEffects2 = GameObject.Find("cardEffects2").GetComponent<Text>();
        typeImage = GameObject.Find("TypeImage").GetComponent<Image>();
        trooper = GameObject.Find("TrooperButton").GetComponent<Button>();
        recon = GameObject.Find("ReconButton").GetComponent<Button>();
        steamer = GameObject.Find("SteamerButton").GetComponent<Button>();
        pewpew = GameObject.Find("PewpewButton").GetComponent<Button>();
        compensator = GameObject.Find("CompensatorButton").GetComponent<Button>();
        foundation = GameObject.Find("FoundationButton").GetComponent<Button>();
        claymore = GameObject.Find("ClaymoreButton").GetComponent<Button>();
        midas = GameObject.Find("MidasButton").GetComponent<Button>();
        powerSurge = GameObject.Find("PowerSurgeButton").GetComponent<Button>();
        albarn = GameObject.Find("General1").GetComponent<Button>();
        tungsten =GameObject.Find("General2").GetComponent<Button>();
        adren = GameObject.Find("General3").GetComponent<Button>();
        sandman = GameObject.Find("General4").GetComponent<Button>();
        highlight = Color.yellow;
        fade = Color.grey;
        addUnitButton = GameObject.Find("SelectUnitButton");
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
                cardEffects1.text = ("+1 Movement Action" + Environment.NewLine + "Enemies within 1 range of the Claymore cannot move until your next turn");
                cardName2.text = ("Poppin' Smoke");
                cardEffects2.text = ("Enemies within 2 range of the Claymore have their vision reduced to 0 until your next turn");
                typeImage.sprite = support;
                break;
                
            case UnitType.compensator:
                cardName1.text = ("Over Compensation");
                cardEffects1.text = ("+30 Armor Penetration" + Environment.NewLine + "+2 Movement Speed" + Environment.NewLine + "+10 Attack Damage");
                cardName2.text = ("Size Matters");
                cardEffects2.text = ("+40 Armor Penetration" + Environment.NewLine + "+2 Range");
                typeImage.sprite = piercing;
                break;
                
            case UnitType.foundation:
                cardName1.text = ("Both Barrels");
                cardEffects1.text = ("Double Armor Penetration" + Environment.NewLine + "Double Attack Damage");
                cardName2.text = ("To The Skies");
                cardEffects2.text = ("+8 Range" + Environment.NewLine + "Your Foundations attacks deal area damage within one tile of your target for this turn");
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
                cardEffects2.text = ("+2 Movement Speed" + Environment.NewLine + "+10 Attack Damage" + Environment.NewLine + "+1 Attack Action");
                typeImage.sprite = heavy;
                break;
                
            case UnitType.powerSurge:
                cardName1.text = ("Death Ball");
                cardEffects1.text = ("+10 Armor to allies within 2 range" + Environment.NewLine + "+10 Attack Damage to allies within 2 range");
                cardName2.text = ("High Ground Simulator");
                cardEffects2.text = ("+10 Attack Damage to adjacent allies" + Environment.NewLine + "+1 Movement Speed to adjacent allies" + Environment.NewLine + "+1 Range to adjacent allies");
                typeImage.sprite = support;
                break;
                
            case UnitType.recon:
                cardName1.text = ("Eagle Aspect");
                cardEffects1.text = ("+3 Range" + Environment.NewLine + "+2 Movement Speed");
                cardName2.text = ("Noob Tube");
                cardEffects2.text = ("+15 Attack Damage" + Environment.NewLine + "The Recon's attacks deal area damage within 1 range of your target for this turn");
                typeImage.sprite = light;
                break;
                
            case UnitType.steamer:
                cardName1.text = ("Real Steel");
                cardEffects1.text = ("+20 Armor" + Environment.NewLine + "-1 Movement Speed");
                cardName2.text = ("We Get It... You Vape");
                cardEffects2.text = ("The Steamer's attacks deal damage in a 3 tile cone for this turn");
                typeImage.sprite = heavy;
                break;
                
            case UnitType.trooper:
                cardName1.text = ("On The Juice");
                cardEffects1.text = ("+10 Attack Damage" + Environment.NewLine + "+10 Armor Penetration");
                cardName2.text = ("Run'n'Gun");
                cardEffects2.text = ("+2 Movement Speed" + Environment.NewLine + "+1 Movement Action" + Environment.NewLine + "+15 Attack Damage");
                typeImage.sprite = light;
                break;
            
            case UnitType.light_adren:
                cardName1.text = ("Stick and Poke");
                cardEffects1.text = ("Cooldown: 5 turns" + Environment.NewLine + "Your units gain 3 Movement Speed and 1 Movement Action for 2 turns");
                cardName2.text = ("Deep Penetration");
                cardEffects2.text = ("Cooldown: 3 turns" + Environment.NewLine + "Choose a unit. The unit's Armor is set to 0 for this turn");
                typeImage.sprite = general;
                break;
                
            case UnitType.piercing_tungsten:
                cardName1.text = ("Trojan Shot");
                cardEffects1.text = ("Cooldown: 6 turns" + Environment.NewLine + "For this turn Tungsten's Range becomes 20 and his attack hits all units between him and his target");
                cardName2.text = ("Armor Piercing Ammo");
                cardEffects2.text = ("Cooldown: 6 turns" + Environment.NewLine + "Select a unit within 10 range of Tungsten. This unit gains 10 Armor Penetration for 4 turns");
                typeImage.sprite = general;
                break;
            
            case UnitType.heavy_albarn:
                cardName1.text = ("The Best Offense");
                cardEffects1.text = ("Cooldown: 2 turns" + Environment.NewLine + "Choose a unit. The unit's Armor is added to its Attack Damage");
                cardName2.text = ("Steam Overload");
                cardEffects2.text = ("Cooldown: 3 turns" + Environment.NewLine + "Choose a location within 10 Range of Albarn. The target takes 30 damage, units within 1 range take 20, units within 2 range take 10");
                typeImage.sprite = general;
                break;
            
            case UnitType.support_sandman:
                cardName1.text = ("Sahara Mine");
                cardEffects1.text = ("Cooldown: 4 turns" + Environment.NewLine + "Choose a target within 2 Range of Sandman. All units within 1 range of the target take 10 damage and lose 10 Armor for this turn");
                cardName2.text = ("Sandstorm");
                cardEffects2.text = ("Cooldown: 4 turns" + Environment.NewLine + "All units within 2 range of Sandman have their Vision and Movement Speed reduced by 3. The sandstorm lasts for 2 turns");
                typeImage.sprite = general;
                break;
        }
    }

    public void AddUnit() {
        //if it is a unit, add it to the army
        if ((int)selectedUnit < UnitMetadata.GENERAL_THRESHOLD) {//generals are above 100
            selectedArmy.AddUnit(selectedUnit);
            GetCost();
            AddUnitHelper(selectedUnit);
        }
        else {
            selectedArmy.ReplaceGeneral((int)selectedUnit);
            GetCost();
            DeleteGeneralHelper();
            AddUnitHelper(selectedUnit);
        }
    }

    public void DeleteUnit() {
        //only remove the unit if it is not a general
        //a general must be in an army
        if ((int)selectedUnit < UnitMetadata.GENERAL_THRESHOLD) {
            selectedArmy.RemoveUnit(selectedUnit);
            GetCost();
            DeleteUnitHelper(selectedUnit);
        }
    }

    public void GetCost() {
        stockNum.SetText(UnitFactory.CalculateCost(selectedArmy.Units).ToString());
    }

    public void SaveArmy() {
        string armyName = GameObject.Find("ABNameInput").GetComponent< TMP_InputField>().text;
        if (!StringValidation.ValidateArmyName(armyName)) {
            audioManager.Play(SoundName.ButtonError);
            Debug.Log("invalid army name");
            //Something has to inform the user here
            return;
        }
        selectedArmy.Name = armyName;
        client.RegisterArmyPreset(selectedArmy);
        audioManager.Play(SoundName.ButtonPress);
    }

    public void DeleteArmy() {
        client.RemoveArmyPreset(selectedArmy.Id);
    }

    private void AddUnitHelper(UnitType type) {
        GameObject unitText = Instantiate(friendsListCellPrefab);
        unitText.GetComponent<TMP_Text>().SetText(UnitMetadata.ReadableNames[type]);
        unitText.transform.SetParent(armyContent.transform, false);
    }

    public void DeleteUnitHelper(UnitType unit) {
        string name = UnitMetadata.ReadableNames[unit];
        foreach (Transform child in armyContent.transform) {
            if (child.transform.name != "Content" && child.GetComponent<TextMeshProUGUI>().text == name){
                Destroy(child.gameObject);
                return;
            }
        }
    }

    public void DeleteGeneralHelper() {
        foreach (Transform child in armyContent.transform) {
            if (child.transform.name != "Content" &&
                (child.GetComponent<TextMeshProUGUI>().text == "Albarn" ||
                child.GetComponent<TextMeshProUGUI>().text == "Tungsten" ||
                child.GetComponent<TextMeshProUGUI>().text == "Adren-LN" ||
                child.GetComponent<TextMeshProUGUI>().text == "The Sandman")) {
                Destroy(child.gameObject);
                return;
            }
        }
    }

    public void DeleteUnitsHelper() {
        foreach (Transform child in armyContent.transform) {
            if (child.transform.name != "Content")
                Destroy(child.gameObject);   
        }
    }
    
    public void ChangeFilter() {
        var colors = recon.colors;
        colors.normalColor = fade;
        recon.colors = colors;
        
        colors = trooper.colors;
        colors.normalColor = fade;
        trooper.colors = colors;
        
        colors = steamer.colors;
        colors.normalColor = fade;
        steamer.colors = colors;
        
        colors = pewpew.colors;
        colors.normalColor = fade;
        pewpew.colors = colors;
        
        colors = compensator.colors;
        colors.normalColor = fade;
        compensator.colors = colors;
        
        colors = foundation.colors;
        colors.normalColor = fade;
        foundation.colors = colors;
        
        colors = claymore.colors;
        colors.normalColor = fade;
        claymore.colors = colors;
        
        colors = powerSurge.colors;
        colors.normalColor = fade;
        powerSurge.colors = colors;
        
        colors = midas.colors;
        colors.normalColor = fade;
        midas.colors = colors;
        
        colors = adren.colors;
        colors.normalColor = fade;
        adren.colors = colors;
        
        colors = albarn.colors;
        colors.normalColor = fade;
        albarn.colors = colors;
        
        colors = tungsten.colors;
        colors.normalColor = fade;
        tungsten.colors = colors;
        
        colors = sandman.colors;
        colors.normalColor = fade;
        sandman.colors = colors;
    }
    
    public void FilterLight() {
        var colors = recon.colors;
        colors.normalColor = highlight;
        recon.colors = colors;
        
        colors = trooper.colors;
        colors.normalColor = highlight;
        trooper.colors = colors;
        
        colors = adren.colors;
        colors.normalColor = highlight;
        adren.colors = colors;
    }
    
    public void FilterPiercing() {
        var colors = compensator.colors;
        colors.normalColor = highlight;
        compensator.colors = colors;
        
        colors = foundation.colors;
        colors.normalColor = highlight;
        foundation.colors = colors;
        
        colors = tungsten.colors;
        colors.normalColor = highlight;
        tungsten.colors = colors;
    }
    
    public void FilterHeavy() {
        var colors = steamer.colors;
        colors.normalColor = highlight;
        steamer.colors = colors;
        
        colors = pewpew.colors;
        colors.normalColor = highlight;
        pewpew.colors = colors;
        
        colors = albarn.colors;
        colors.normalColor = highlight;
        albarn.colors = colors;
    }
    
    public void FilterSupport() {
        var colors = midas.colors;
        colors.normalColor = highlight;
        midas.colors = colors;
        
        colors = claymore.colors;
        colors.normalColor = highlight;
        claymore.colors = colors;
        
        colors = powerSurge.colors;
        colors.normalColor = highlight;
        powerSurge.colors = colors;
        
        colors = sandman.colors;
        colors.normalColor = highlight;
        sandman.colors = colors;
    }
    
    public void FilterGeneral() {
        var colors = albarn.colors;
        colors.normalColor = highlight;
        albarn.colors = colors;
        
        colors = adren.colors;
        colors.normalColor = highlight;
        adren.colors = colors;
        
        colors = tungsten.colors;
        colors.normalColor = highlight;
        tungsten.colors = colors;
        
        colors = sandman.colors;
        colors.normalColor = highlight;
        sandman.colors = colors;
    }
}
