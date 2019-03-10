using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class displayStats : MonoBehaviour
{
    public GameObject unitName;
    public GameObject uType;
    public GameObject healthNum;
    public GameObject attackNum;
    public GameObject armourNum;
    public GameObject rangeNum;
    public GameObject pierceNum;
    public GameObject aoeNum;
    public GameObject moveNum;

    public void displayUnitStats(int num)
    {
        if (num == 1)
            unitName.GetComponent<TextMeshProUGUI>().text = "Trooper";
        if (num == 2)
            unitName.GetComponent<TextMeshProUGUI>().text = "Reacon";
        if (num == 3)
            unitName.GetComponent<TextMeshProUGUI>().text = "Steamer";
        if (num == 4)
            unitName.GetComponent<TextMeshProUGUI>().text = "Pewpew";
        if (num == 5)
            unitName.GetComponent<TextMeshProUGUI>().text = "Compensator";
        if (num == 6)
            unitName.GetComponent<TextMeshProUGUI>().text = "Foundation";
        if (num == 7)
            unitName.GetComponent<TextMeshProUGUI>().text = "Power Surge";
        if (num == 8)
            unitName.GetComponent<TextMeshProUGUI>().text = "Midas";
        if (num == 9)
            unitName.GetComponent<TextMeshProUGUI>().text = "Claymore";

        healthNum.GetComponent<TextMeshProUGUI>().text =
            UnitFactory.GetBaseUnit((UnitType)num).CurrentHP.ToString();

        attackNum.GetComponent<TextMeshProUGUI>().text =
            UnitFactory.GetBaseUnit((UnitType)num).Damage.ToString();

        armourNum.GetComponent<TextMeshProUGUI>().text =
            UnitFactory.GetBaseUnit((UnitType)num).Armour.ToString();

        rangeNum.GetComponent<TextMeshProUGUI>().text =
            UnitFactory.GetBaseUnit((UnitType)num).Range.ToString();

        pierceNum.GetComponent<TextMeshProUGUI>().text =
            UnitFactory.GetBaseUnit((UnitType)num).Pierce.ToString();

        aoeNum.GetComponent<TextMeshProUGUI>().text =
            UnitFactory.GetBaseUnit((UnitType)num).Aoe.ToString();

        moveNum.GetComponent<TextMeshProUGUI>().text =
            UnitFactory.GetBaseUnit((UnitType)num).MovementSpeed.ToString();

    }
}
