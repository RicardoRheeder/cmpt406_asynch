using System.Collections;
using System.Collections.Generic;
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

    //References to texts we update
    private TMP_Text stockNum;
    private TMP_Text healthNum;
    private TMP_Text attackNum;
    private TMP_Text armourNum;
    private TMP_Text rangeNum;
    private TMP_Text pierceNum;
    private TMP_Text aoeNum;
    private TMP_Text movementNum;
    private TMP_Text unitName;

    public void Awake() {
        client = GameObject.Find("Networking").GetComponent<Client>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        armyContent = GameObject.Find("ABListViewport");
        stockNum = GameObject.Find("CostNum").GetComponent<TMP_Text>();
        healthNum = GameObject.Find("HealthNum").GetComponent<TMP_Text>();
        attackNum = GameObject.Find("AttackNum").GetComponent<TMP_Text>();
        armourNum = GameObject.Find("ArmorNum").GetComponent<TMP_Text>();
        rangeNum = GameObject.Find("RangeNum").GetComponent<TMP_Text>();
        pierceNum = GameObject.Find("PierceNum").GetComponent<TMP_Text>();
        aoeNum = GameObject.Find("AOENum").GetComponent<TMP_Text>();
        movementNum = GameObject.Find("MovementNum").GetComponent<TMP_Text>();
        unitName = GameObject.Find("UnitName").GetComponent<TMP_Text>();

        ConfigureOnClick(GameObject.Find("TrooperButton").GetComponent<Button>(), UnitType.trooper);
        ConfigureOnClick(GameObject.Find("ReconButton").GetComponent<Button>(), UnitType.recon);
        ConfigureOnClick(GameObject.Find("SteamerButton").GetComponent<Button>(), UnitType.steamer);
        ConfigureOnClick(GameObject.Find("PewpewButton").GetComponent<Button>(), UnitType.pewpew);
        ConfigureOnClick(GameObject.Find("CompensatorButton").GetComponent<Button>(), UnitType.compensator);
        ConfigureOnClick(GameObject.Find("FoundationButton").GetComponent<Button>(), UnitType.foundation);
        ConfigureOnClick(GameObject.Find("ClaymoreButton").GetComponent<Button>(), UnitType.claymore);
        ConfigureOnClick(GameObject.Find("MidasButton").GetComponent<Button>(), UnitType.midas);
        ConfigureOnClick(GameObject.Find("General1").GetComponent<Button>(), UnitType.heavy_albarn);
        ConfigureOnClick(GameObject.Find("General2").GetComponent<Button>(), UnitType.piercing_tungsten);
        ConfigureOnClick(GameObject.Find("General3").GetComponent<Button>(), UnitType.light_adren);
        ConfigureOnClick(GameObject.Find("General4").GetComponent<Button>(), UnitType.support_sandman);
    }

    private void ConfigureOnClick(Button button, UnitType type) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            selectedUnit = type;
            UpdateDisplay();
            audioManager.Play("ButtonPress");
        });
    }

    //creates a new army
    public void CreateArmy() {
        string newArmyName = "Name Army";
        ArmyPreset newPreset = new ArmyPreset(
            newArmyName,
            new List<int>(),
            (int)UnitType.piercing_tungsten
        );

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
    }

    //adds a unit to an army
    public void AddUnit() {
        //if it is a unit, add it to the army
        if ((int)selectedUnit < UnitMetadata.GENERAL_THRESHOLD) {//generals are above 100
            selectedArmy.AddUnit(selectedUnit);
            GetCost();
            AddUnitHelper(selectedUnit);
        }
        //else, it is a general and you have to replace the other general
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
        if (!StringValidation.ValidateGameName(armyName)) {
            audioManager.Play("ButtonError");
            Debug.Log("invalid army name");
            //Something has to inform the user here
            return;
        }
        selectedArmy.Name = armyName;
        client.RegisterArmyPreset(selectedArmy);
        audioManager.Play("ButtonPress");
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
}
