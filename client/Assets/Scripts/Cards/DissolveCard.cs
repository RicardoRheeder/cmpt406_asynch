using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolveCard : MonoBehaviour
{

    Material mat;

    private void Start()
    {
    }

    private void Update()
    {
        
    }

    public void dissolve()
    {
        mat = GetComponent<Image>().material;

//        print("IMAGE: " + mat.name);
//        print("MAT: " + this.GetComponentInParent<CardDisplay_v2>().dissolveMaterial.name);
        if (mat == this.GetComponentInParent<CardDisplay_v2>().dissolveMaterial)
        {
            mat.SetFloat("_Progress", 1);
            StartCoroutine(DissolveOverTime());
        }
 
    }



    IEnumerator DissolveOverTime()
    {
        while (mat.GetFloat("_Progress") > 0)
        {
            mat.SetFloat("_Progress", mat.GetFloat("_Progress") - 0.005f);
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;
    }

    void OnApplicationQuit()
    {
        if (mat != null)
        mat.SetFloat("_Progress", 1);
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}
