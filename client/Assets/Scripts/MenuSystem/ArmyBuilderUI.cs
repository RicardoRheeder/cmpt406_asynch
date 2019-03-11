using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmyBuilderUI : MonoBehaviour
{
    public ArmyPreset selectedArmy;

    //the current unit being looked at
    public int selectedUnit;

    private Canvas canvas;

    public GameObject stockNum;
    public GameObject armyContent;
    public GameObject friendsListCellPrefab;

    private Client client;

    public void Start()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");


    }

    public void Update()
    {
        if (client == null)
        {

        }
    }

    public void Awake()
    {
        armyContent = GameObject.Find("ABListViewport");
        client = GameObject.Find("Networking").GetComponent<Client>();
    }

    //creates a new army
    public void createArmy()
    {
        string newArmyName = "Name Army";
        ArmyPreset newPreset = new ArmyPreset(
            newArmyName,
            new List<int>(),
            101
        );

        selectedArmy = newPreset;
        getCost();
    }

    //changes the name of an army
    public void changeName(string newName)
    {
        selectedArmy.Name = newName;
    }

    //adds a unit to an army
    public void addUnit()
    {
        //if it is a unit, add it to the army
        if ((int)selectedUnit < 99)//generals are above 100
            selectedArmy.Units.Add(selectedUnit);

        getCost();
        AddUnitHelper(selectedUnit);
    }

    public void deleteUnit()
    {
        //only remove the unit if it is not a general
        //a general must be in an army
        if ((int)selectedUnit < 99)
            selectedArmy.Units.Remove(selectedUnit);

        getCost();
        deleteUnitHelper(selectedUnit);

    }

    //gets the cost of an army
    public void getCost()
    {
        //the army cost
        int cost = 0;

        //for each unit in the army
        for (int i = 0; i < selectedArmy.Units.Count; i++)
        {
            //add the units cost to the army cost
            cost += UnitFactory.GetBaseUnit((UnitType)selectedArmy.Units[i]).Cost;
        }

        stockNum.GetComponent<TextMeshProUGUI>().text = cost.ToString();
    }


    public void changeSelectedUnit(int num)
    {
        selectedUnit = num;
    }

    //TODO: add saving army to server
    public void saveArmy()
    {
        client.RegisterArmyPreset(selectedArmy);

    }

    public void deleteArmy(string presetID)
    {
        client.RemoveArmyPreset(selectedArmy.Id);
    }

    private void AddUnitHelper(int name)
    {
        GameObject unitText = Instantiate(friendsListCellPrefab);
        if (name == 0)
            unitText.GetComponent<TMP_Text>().text = "Trooper";
        if (name == 1)
            unitText.GetComponent<TMP_Text>().text = "Reacon";
        if (name == 2)
            unitText.GetComponent<TMP_Text>().text = "Steamer";
        if (name == 3)
            unitText.GetComponent<TMP_Text>().text = "Pewpew";
        if (name == 4)
            unitText.GetComponent<TMP_Text>().text = "Compensator";
        if (name == 5)
            unitText.GetComponent<TMP_Text>().text = "Foundation";
        if (name == 6)
            unitText.GetComponent<TMP_Text>().text = "Power Surge";
        if (name == 7)
            unitText.GetComponent<TMP_Text>().text = "Midas";
        if (name == 8)
            unitText.GetComponent<TMP_Text>().text = "Claymore";
        unitText.transform.SetParent(armyContent.transform, false);
    }

    public void deleteUnitHelper(int unit)
    {
        string name = null;
        if (unit == 0)
            name = "Trooper";
        if (unit == 1)
            name = "Reacon";
        if (unit == 2)
            name = "Steamer";
        if (unit == 3)
            name = "Pewpew";
        if (unit == 4)
            name = "Compensator";
        if (unit == 5)
            name = "Foundation";
        if (unit == 6)
            name = "Power Surge";
        if (unit == 7)
            name = "Midas";
        if (unit == 8)
            name = "Claymore";
        foreach (Transform child in armyContent.transform)
        {
            if (child.transform.name != "Content" &&
               child.GetComponent<TextMeshProUGUI>().text == name)
            {
                Destroy(child.gameObject);
                return;
            }
        }

    }


}
