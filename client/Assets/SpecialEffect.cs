using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecialEffect : MonoBehaviour
{
    private UnitType unitType;

    private GameObject gm;

    public Transform particleEffect;
    public Transform startPoint;
    private float distance;

    Color m_changeColor;
    bool isChangeColor = false;
    Renderer[] m_rnds;

    private Transform[] allChildren;
    private float m_changeScale = 1;
    private bool isChangeScale = false;

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

    }

    public void PlayAttackEffect(Vector3 sourceWorldPos, Vector3 targetWorldPos, UnitType unitType)
    {
        this.unitType = unitType;
        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        distance = Vector3.Distance(sourceWorldPos, targetWorldPos);
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
                VEAlbarnAttack();
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

    }

    void VETungstenAttack()
    {
        ChangeColor(Color.red);
        ChangeScale(3);
        StartCoroutine(findMakeObject());
    }

    void VECardEffect() { }

    void VEDeath() { }

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

    /**
     *  RICARDO's SECTION END
    */
    //------------------------------------------
    /**
     *  TODO: GAURAV's SECTION START
     */

    void VETrooperAttack()
    {
        StartCoroutine(findMakeObject());
    }

    void VEReconAttack()
    {
        StartCoroutine(findMakeObject());
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

    void VEPewPewAttack()
    {
        // Logic that was used when trying to figure out what was wrong with orb
//        particleEffect.GetComponent<TranslateMove>().m_fowardMove = false;
//        particleEffect.GetComponent<TranslateMove>().m_upMove = true;
//        particleEffect.GetComponent<TranslateMove>().m_rightMove = false;

    }

    void VESteamerAttack()
    {

    }

    void VEAdrenAttack()
    {

    }

    void VEAlbarnAttack()
    {

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
        print("FOUND");
        MakeObject.transform.localPosition = new Vector3(0, 0, distance);
    }
}
