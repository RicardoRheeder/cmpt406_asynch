using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecialEffect : MonoBehaviour
{
    private UnitType unitType;

    private GameObject gm;
    private GameObject gm2;

    public Transform particleEffect;
    public Transform startPoint;
    private float distance;

    Color m_changeColor;
    bool isChangeColor = false;
    Renderer[] m_rnds;

    private Transform[] allChildren;
    private Transform[] allChildren2;

    private float m_changeScale = 1;
    private bool isChangeScale = false;
    private bool isChangeScaleGameObject = false;

    public GameObject effect47Explosion;
    public GameObject effect06CardAura;

    void Update()
    {
        if (isChangeColor && gm != null)
        {
            m_rnds = gm.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer rend in m_rnds)
            {
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    rend.materials[i].SetColor("_TintColor", m_changeColor);
                    rend.materials[i].SetColor("_Color", m_changeColor);
                    rend.materials[i].SetColor("_RimColor", m_changeColor);
                }
            }
        }
        if (isChangeScale && gm != null)
        {
            allChildren = gm.GetComponentsInChildren<Transform>();

            foreach (Transform child in allChildren)
            {
                if (child.GetComponent<ParticleSystem>())
                {
                    child.localScale = new Vector3(m_changeScale, m_changeScale, m_changeScale);
                }
            }
        }

        if (isChangeScaleGameObject && gm != null)
        {
            allChildren = gm.GetComponentsInChildren<Transform>();

            foreach (Transform child in allChildren) {
                if (child.GetComponent<TranslateMove>()) {child.localScale = new Vector3(m_changeScale, m_changeScale, m_changeScale);}
//                if (child.GetComponent<ParticleSystem>()) {
//                    child.localScale = new Vector3(m_changeScale, m_changeScale, m_changeScale);
//                }
            }

            if (gm2 != null) {
                print("Scale change");
                allChildren2 = gm2.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren2) {
                    if (child.GetComponent<TranslateMove>()) { child.localScale = new Vector3(m_changeScale, m_changeScale, m_changeScale);}
//                    if (child.GetComponent<ParticleSystem>()) {
//                        child.localScale = new Vector3(m_changeScale, m_changeScale, m_changeScale);
//                    }
                }
            }
        }


    }

    public void PlayAttackEffect(Vector3 sourceWorldPos, Vector3 targetWorldPos, UnitType unitType)
    {
        this.unitType = unitType;
        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        distance = Vector3.Distance(sourceWorldPos, targetWorldPos);
        print(distance);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        switch (unitType)
        {
            //------------------------------------------
            /**
            *  TODO: RICARDO's SECTION START
            */

            case UnitType.compensator:
                VECompensatorAttack();
                break;

            case UnitType.foundation:
                VEFoundationAttack();
                break;

            case UnitType.piercing_tungsten:
                VETungstenAttack();
                break;

            /**
             *  RICARDO's SECTION END
            */
            //------------------------------------------
            /**
            *  TODO: GAURAV's SECTION START
            */

            case UnitType.trooper:
                VETrooperAttack();
                break;
            case UnitType.recon:
                VEReconAttack();
                break;
            case UnitType.support_sandman:
                VESandmanAttack();
                break;

            /**
             *  GAURAV's SECTION END
            */
            //------------------------------------------
            /**
            *  TODO: CLAYTON's SECTION START
            */

            case UnitType.pewpew:
                VEPewPewAttack();
                break;
            case UnitType.steamer:
                VESteamerAttack();
                break;
            case UnitType.light_adren:
                VEAdrenAttack();
                break;
            case UnitType.heavy_albarn:
                VEAlbarnAttack(sourceWorldPos, targetWorldPos);
                break;

            /**
             *  CLAYTONS's SECTION END
            */
            //------------------------------------------
            /**
            *  TODO: JAAMI's SECTION START
            */

            case UnitType.claymore:
                VEClaymoreAttack();
                break;
            case UnitType.powerSurge:
                VEPowerSurgeAttack();
                break;
            case UnitType.midas:
                VEMidasAttack();
                break;

            /**
             *  JAAMI's SECTION END
            */
            //------------------------------------------

        }

        Destroy(gm, 3f); //Destroys the Visual Effect
    }


    //------------------------------------------
    /**
    *  TODO: RICARDO's SECTION START
    */
    void VECompensatorAttack()
    {
        StartCoroutine(findMakeObject());
    }

    void VEFoundationAttack()
    {
        //TODO: Clean up script
        ChangeScaleGameObject(0.5f);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);
        gm.transform.localPosition = new Vector3(startPoint.localPosition.x + 0.60f, startPoint.localPosition.y, startPoint.localPosition.z);


        gm2 = Instantiate(particleEffect, this.transform).gameObject;
        gm2.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);
        gm.transform.localPosition = new Vector3(startPoint.localPosition.x + 0.60f, startPoint.localPosition.y, startPoint.localPosition.z);

        StartCoroutine(setArrowDirection());
    }

    void VETungstenAttack()
    {
        ChangeColor(Color.red);
        ChangeScale(2);
        StartCoroutine(findMakeObject());
    }

    public void VECardEffect()
    {
        gm = Instantiate(effect06CardAura, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        ChangeColor(new Color(0.298f, 0.796f, 1f));
        ChangeScale(0.5f);
        Destroy(gm, 1.5f);
    }

    public void VEDeath()
    {
        gm = Instantiate(effect47Explosion, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        ChangeColor(new Color(1.0f, 0.27f, 0.0f));
    }

    void ChangeColor(Color desiredColor)
    {
        m_changeColor = desiredColor;
        isChangeColor = true;
    }

    void ChangeScale(float scaleAmount)
    {
        m_changeScale = scaleAmount;
        isChangeScale = true;
    }

    void ChangeScaleGameObject(float scaleAmout)
    {
        m_changeScale = scaleAmout;
        isChangeScaleGameObject = true;
    }

    /**
     *  RICARDO's SECTION END
    */
    //------------------------------------------
    /**
     *  TODO: GAURAV's SECTION START
     */

    IEnumerator setArrowDirection(){
        Debug.Log(distance);
        yield return new WaitForSeconds(0.1f);
        Transform[] allChildren = this.GetComponentsInChildren<Transform>();
        while(allChildren.Length != 0) {
            allChildren = this.GetComponentsInChildren<Transform>();
            for (int i = 0; i < allChildren.Length - 1; i++)
            {
                if (allChildren[i].GetComponent<TranslateMove>())
                {
                    allChildren[i].GetComponent<TranslateMove>().m_fowardMove = false;
                    allChildren[i].GetComponent<TranslateMove>().m_upMove = true;
                    allChildren[i].GetComponent<TranslateMove>().m_power = -1;
                    if (allChildren[i].transform.localPosition.z >= distance)
                    {
                        Destroy(allChildren[i].gameObject);
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
    void VETrooperAttack() {
        StartCoroutine(setArrowDirection());
    }

    void VEReconAttack(){
        StartCoroutine(setArrowDirection());
    }

    void VESandmanAttack()
    {

    }

    /**
     *  GAURAV's SECTION END
    */
    //------------------------------------------
    /**
    *  CLAYTON's SECTION START
    */

    public Transform attackPoint;

    void VEPewPewAttack()
    {
        // Logic that was used when trying to figure out what was wrong with orb
        //        particleEffect.GetComponent<TranslateMove>().m_fowardMove = false;
        //        particleEffect.GetComponent<TranslateMove>().m_upMove = true;
        //        particleEffect.GetComponent<TranslateMove>().m_rightMove = false;

    }

    void VESteamerAttack()
    {
        startPoint.localPosition = new Vector3(0, 0, distance);
    }

    void VEAdrenAttack()
    {

    }

    void VEAlbarnAttack(Vector3 sourceWorldPos, Vector3 targetWorldPos)
    {
        Debug.Log("Albarn Attack");
        //distance = Vector3.Distance(sourceWorldPos, targetWorldPos);
        //print(distance);
        //attackPoint = startPoint.parent.Find("AttackPoint");

        //attackPoint.transform.localPosition = new Vector3(0, 0, distance);
        
        GameObject gm2 = Instantiate(particleEffect.gameObject) as GameObject;
        //gm2.transform.SetParent(this.transform);
        gm2.transform.position = new Vector3(targetWorldPos.x, targetWorldPos.y, targetWorldPos.z);
        gm2.transform.eulerAngles = new Vector3(0, 180, 0);
        Destroy(gm, 5f);
    }

    /**
     *  CLAYTONS's SECTION END
    */
    //------------------------------------------
    /**
    *  JAAMI's SECTION START
    */

    void VEClaymoreAttack()
    {

    }

    void VEPowerSurgeAttack()
    {

    }

    void VEMidasAttack()
    {

    }

    /**
     *  JAAMI's SECTION END
    */
    //------------------------------------------

    IEnumerator findMakeObject()
    {
        GameObject MakeObject = null;
        while (MakeObject == null)
        {
            MakeObject = GameObject.Find("MakeObject");
            yield return new WaitForSeconds(0.1f);
        }
        MakeObject.transform.localPosition = new Vector3(0, 0, distance);
    }
}
