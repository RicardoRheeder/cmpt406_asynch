using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameMenu : MonoBehaviour
{

    private GameObject settingsPanel;
    private GameObject placeUnitsPanel;
    private GameObject unitSnapPanel;
    private GameObject topMenuPanel;
    private GameObject generalAbilitiesPanel;
    private GameObject cardPanel;
    private GameObject unitImagePanel;
    private GameObject unitStatsPanel;
    private GameObject menuPanel;
    private GameObject actionsPanel;
	public TMP_Text tooltip;
	private UnitStats general;
    public GameObject returningToMainMenuPanel;
    public GameObject replayOpponentTurnsPanel;
    public GameObject replayDonePanel;
    public GameObject victoryPanel;
    public Button victoryButton;
    public GameObject defeatPanel;
    public Button defeatButton;
	

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
        actionsPanel = GameObject.Find("ActionsPanel");
    }

    public void SetupPanels(bool isPlacing, UnitStats general=null) {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        returningToMainMenuPanel.SetActive(false);
        replayDonePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        this.general = general;

        if (isPlacing) {
            unitSnapPanel.SetActive(false);
            cardPanel.SetActive(false);
            unitStatsPanel.SetActive(false);
            generalAbilitiesPanel.SetActive(false);
            actionsPanel.SetActive(false);
        }
        else
        {
            replayOpponentTurnsPanel.SetActive(true);
            placeUnitsPanel.SetActive(false);
        }
    }
	
	public void Ability1Tooltip() {
        if(general != null) {
            tooltip.SetText(GeneralMetadata.GeneralAbilityTooltip[general.Ability1]);
        }
	}
	
	public void Ability2Tooltip() {
        if (general != null) {
            tooltip.SetText(GeneralMetadata.GeneralAbilityTooltip[general.Ability2]);
        }
	}
}
