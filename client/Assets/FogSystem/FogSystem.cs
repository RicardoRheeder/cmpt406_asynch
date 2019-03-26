using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSystem : MonoBehaviour
{
    public float speed = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.eulerAngles = new Vector3(Random.Range(0, 360), 90, 90);
//        setMistColor(2);
        speed = Random.Range(0.001f, 0.04f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(0, Time.time* speed);
        this.GetComponent<MeshRenderer>().material.mainTextureOffset = offset;

    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void setMistColor(int dayCycle)
    {
        float alphaAmount;
//        this.GetComponent<MeshRenderer>().material.color = new Color(200, 200, 200, 1f);
        if (dayCycle == 0)
        {
            alphaAmount = Random.Range(0, 0.15f);
            this.GetComponent<MeshRenderer>().material.SetColor("_TintColor", new Color(0.4f, 0.4f, 0.4f, alphaAmount));

        }
        else if (dayCycle == 1)
        {
            alphaAmount = Random.Range(0.15f, 0.45f);
            this.GetComponent<MeshRenderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, alphaAmount));

        }
        else
        {
            alphaAmount = Random.Range(0.45f, 0.7f);
            this.GetComponent<MeshRenderer>().material.SetColor("_TintColor", new Color(0.6f, 0.6f, 0.6f, alphaAmount));

        }

    }
}
