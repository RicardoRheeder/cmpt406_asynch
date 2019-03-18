using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DissolveAttributes : MonoBehaviour {
    public float dissolveDuration = 2.8f;

    public void Dissolve() {
        if (this.GetComponent<Image>() != null) {
            this.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        if (this.GetComponent<TextMeshProUGUI>() != null) {
            this.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
        }
        StartCoroutine(FadeTo(0.0f, dissolveDuration));
    }

    IEnumerator FadeTo(float aValue, float aTime) {
        if (this.GetComponent<Image>() != null) {
            float alpha = this.GetComponent<Image>().color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
                this.GetComponent<Image>().color = newColor;
                yield return null;
            }
        }
        if (this.GetComponent<TextMeshProUGUI>() != null) {
            float alpha = this.GetComponent<TextMeshProUGUI>().color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
                this.GetComponent<TextMeshProUGUI>().color = newColor;
                yield return null;
            }
        }
    }

    void OnApplicationQuit() {
        if (this.GetComponent<Image>() != null) {
            this.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        if (this.GetComponent<TextMeshProUGUI>() != null) {
            this.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
        }
    }
}
