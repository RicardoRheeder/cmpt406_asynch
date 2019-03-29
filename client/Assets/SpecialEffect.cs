using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecialEffect : MonoBehaviour
{
    public Transform particleEffect;
    public Transform startPoint;

    // Update is called once per frame
    public void PlayAttackEffect(Vector3 sourceWorldPos, Vector3 targetWorldPos) {
        GameObject gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);

        float distance = Vector3.Distance(sourceWorldPos, targetWorldPos);
        Debug.Log(distance);

        gm.transform.localPosition = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        Destroy(gm, 3f);
    }
}
