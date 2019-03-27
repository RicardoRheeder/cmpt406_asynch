using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecialEffect : MonoBehaviour
{
    public Transform particleEffect;
    public Transform startPoint;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void playAttackEffect()
    {
        GameObject gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);

        gm.transform.localPosition = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        Destroy(gm, 3f);
    }

}
