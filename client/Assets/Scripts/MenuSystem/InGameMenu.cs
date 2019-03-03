using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour {

    private GameObject settingsPanel;
    private GameObject placeUnitsPanel;
    private GameObject unitSnapPanel;
    private GameObject topMenuPanel;
    private GameObject generalAbilitiesPanel;
    private GameObject cardPanel;
    private GameObject unitImagePanel;
    private GameObject unitStatsPanel;
    private GameObject menuPanel;

    // Start is called before the first frame update
    void Awake() {
        unitStatsPanel = GameObject.Find("UnitStatsPanel");
        unitImagePanel = GameObject.Find("UnitImagePanel");
        cardPanel = GameObject.Find("CardPanel");
        generalAbilitiesPanel = GameObject.Find("GeneralAbilitiesPanel");
        topMenuPanel = GameObject.Find("TopMenuPanel");
        unitSnapPanel = GameObject.Find("UnitSnapPanel");
        placeUnitsPanel = GameObject.Find("PlaceUnitsPanel");
        settingsPanel = GameObject.Find("SettingsPanel");
        menuPanel = GameObject.Find("MenuPanel");
    }

    public void SetupPanels(bool isPlacing) {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);

        if (isPlacing) {
            unitSnapPanel.SetActive(false);
        }
        else {
            placeUnitsPanel.SetActive(false);
        }
    }
}
