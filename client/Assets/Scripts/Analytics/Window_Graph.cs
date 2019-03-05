using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour {

    [SerializeField] private Sprite dotSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private List<GameObject> gameObjectList;
    private GameObject GraphTitle;

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        GraphTitle = graphContainer.Find("graphTitle").gameObject;

        gameObjectList = new List<GameObject>();
    }

    public void ShowGraph(List<AnalyticsManager.SimulatedDamage> valueList, 
                          Dictionary<UnitType, AnalyticsManager.UnitAnalyticsValue> breakDown, Func<float, string> getAxisLabelY = null, string graphTitle = "") {

        if (getAxisLabelY == null) {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        int maxVisibleValueAmount = valueList.Count;

        foreach (GameObject gameObject in gameObjectList) {
            Destroy(gameObject);
        }
        gameObjectList.Clear();
        
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximum = valueList[0].damage;
        float yMinimum = valueList[0].damage;
        
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            int value = valueList[i].damage;
            if (value > yMaximum) {
                yMaximum = value;
            }
            if (value < yMinimum) {
                yMinimum = value;
            }
        }

        float yDifference = yMaximum - yMinimum;
        if (yDifference <= 0) {
            yDifference = 5f;
        }
        yMaximum = yMaximum + (yDifference * 0.2f);
        yMinimum = yMinimum - (yDifference * 0.2f);

        yMinimum = 0f; // Start the graph at zero

        float xSize = graphWidth / (maxVisibleValueAmount + 1);

        int xIndex = 0;

        //GameObject lastDotGameObject = null;
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i].damage - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject barGameObject = CreateBar(new Vector2(xPosition, yPosition), xSize * .9f);
            gameObjectList.Add(barGameObject);

            /* on click, give a breakdown of totals */
            if (breakDown != null && breakDown.ContainsKey(valueList[i].unitType)) {
                UnitType ut = valueList[i].unitType;
                Dictionary<UnitType, int> DamagePerUnitType = breakDown[ut].DamagePerUnitType;
                barGameObject.AddComponent<Button>().onClick.AddListener(delegate {breakDownDamages(ut, DamagePerUnitType);});
            }

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -55f);
            labelX.GetComponent<Text>().text = valueList[i].unitType.ToString() + "\n" + valueList[i].damage;
            gameObjectList.Add(labelX.gameObject);

            xIndex++;
        }

        int separatorCount = 15;
        for (int i = 0; i <= separatorCount; i++) {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-60f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            gameObjectList.Add(labelY.gameObject);
        }

        GraphTitle.GetComponent<Text>().text = graphTitle;
    }

    private GameObject CreateBar(Vector2 graphPosition, float barWidth) {
        GameObject gameObject = new GameObject("bar", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
        rectTransform.sizeDelta = new Vector2(barWidth, graphPosition.y);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(.5f, 0f);
        return gameObject;
    }

    private void breakDownDamages(UnitType unit, Dictionary<UnitType, int> DamagePerUnitType) {
        List<AnalyticsManager.SimulatedDamage> inputToShowGraph = new List<AnalyticsManager.SimulatedDamage>();

        foreach(KeyValuePair<UnitType, int> pair in DamagePerUnitType) {
            AnalyticsManager.SimulatedDamage sd = new AnalyticsManager.SimulatedDamage();
            sd.damage = pair.Value;
            sd.unitType = pair.Key;
            inputToShowGraph.Add(sd);
        }

        string title = unit + " Damage per Unit";
        ShowGraph(inputToShowGraph, null, null, title);
    }

}
