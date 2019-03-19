using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolveCard : MonoBehaviour {

    Material mat;

    public void Dissolve() {
        mat = GetComponent<Image>().material;

        if (mat == this.GetComponentInParent<CardDisplay>().dissolveMaterial) {
            mat.SetFloat("_Progress", 1);
            StartCoroutine(DissolveOverTime());
        }
    }

    IEnumerator DissolveOverTime() {
        while (mat.GetFloat("_Progress") > 0) {
            mat.SetFloat("_Progress", mat.GetFloat("_Progress") - 0.025f);
            yield return new WaitForSeconds(0.005f);
        }
        yield return null;
    }

    void OnApplicationQuit() {
        if (mat != null)
            mat.SetFloat("_Progress", 1);
    }
}
