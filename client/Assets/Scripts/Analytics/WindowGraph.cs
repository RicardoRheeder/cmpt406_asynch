using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour {

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

    /* Given primary and secondary data to display, display the graph */
    public void ShowGraph(List<AnalyticsManager.UnitValuePair> valueList, Dictionary<UnitType, AnalyticsManager.UnitAnalyticsValue> breakDown, string graphTitle) {
        if (valueList == null || valueList.Count <= 0) {
            GraphTitle.GetComponent<Text>().text = "No data to show";
            return;
        }

        Func<float, string> getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };

        int maxVisibleValueAmount = valueList.Count;

        foreach (GameObject gameObject in gameObjectList) {
            Destroy(gameObject);
        }
        gameObjectList.Clear();
        
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximum = valueList[0].value;
        float yMinimum = valueList[0].value;
        
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            float value = valueList[i].value;
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

        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i].value - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject barGameObject = CreateBar(new Vector2(xPosition, yPosition), xSize * .9f);
            gameObjectList.Add(barGameObject);

            /* on click, give a breakdown of totals (if provided) */
            if (breakDown != null && breakDown.ContainsKey(valueList[i].unitType)) {
                UnitType ut = valueList[i].unitType;
                Dictionary<UnitType, float> DamagePerUnitType = breakDown[ut].TotalPerUnitType;
                barGameObject.AddComponent<Button>().onClick.AddListener(delegate {breakDownDamages(ut, DamagePerUnitType);});
            }

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -55f);
            labelX.GetComponent<Text>().text = valueList[i].unitType.ToString() + "\n" + valueList[i].value;
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

    private void breakDownDamages(UnitType unit, Dictionary<UnitType, float> DamagePerUnitType) {
        List<AnalyticsManager.UnitValuePair> inputToShowGraph = new List<AnalyticsManager.UnitValuePair>();

        foreach(KeyValuePair<UnitType, float> pair in DamagePerUnitType) {
            AnalyticsManager.UnitValuePair sd = new AnalyticsManager.UnitValuePair();
            sd.value = pair.Value;
            sd.unitType = pair.Key;
            inputToShowGraph.Add(sd);
        }

        string title = unit + " Damage per Unit";
        ShowGraph(inputToShowGraph, null, title);
    }

}
