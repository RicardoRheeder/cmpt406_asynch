using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecialEffect : MonoBehaviour
{
    private UnitType unitType;

    public Transform particleEffect;
    public Transform startPoint;
    private float distance;

    public void PlayAttackEffect(Vector3 sourceWorldPos, Vector3 targetWorldPos, UnitType unitType)
    {
        this.unitType = unitType;
        GameObject gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        distance = Vector3.Distance(sourceWorldPos, targetWorldPos);
        gm.transform.localPosition = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);


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

    }

    void VECardEffect() { }

    void VEDeath() { }

    /**
     *  RICARDO's SECTION END
    */
    //------------------------------------------
    /**
     *  TODO: GAURAV's SECTION START
     */

    void VETrooperAttack()
    {

    }

    void VEReconAttack()
    {

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
