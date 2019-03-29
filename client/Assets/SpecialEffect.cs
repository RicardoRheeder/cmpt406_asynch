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

    public void PlayAttackEffect(Vector3 sourceWorldPos, Vector3 targetWorldPos) {
        GameObject gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        distance = Vector3.Distance(sourceWorldPos, targetWorldPos);
        gm.transform.localPosition = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);


        //        switch (unitType)
        //        {
        //            case UnitType.compensator:
        //                VECompensatorAttack();
        //                break;
        //        }

        //------------------------------------------
        /**
        *  TODO: RICARDO's SECTION START
        */
        VECompensatorAttack();





        /**
         *  RICARDO's SECTION END
        */
        //------------------------------------------
        /**
         *  TODO: GAURAV's SECTION START
         */
        VEPewPewAttack();




        /**
         *  GAURAV's SECTION END
        */
        //------------------------------------------
        /**
        *  TODO: CLAYTON's SECTION START
        */





        /**
         *  CLAYTONS's SECTION END
        */
        //------------------------------------------
        /**
        *  TODO: JAAMI's SECTION START
        */





        /**
         *  JAAMI's SECTION END
        */
        //------------------------------------------

        Destroy(gm, 3f); //Destroys the Visual Effect
    }


    //------------------------------------------
    /**
    *  RICARDO's SECTION START
    */
    private void VECompensatorAttack()
    {
        StartCoroutine(findMakeObject());
    }

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

    private void VECardEffect() { }

    private void VEDeath() { }

    /**
     *  RICARDO's SECTION END
    */
    //------------------------------------------
    /**
     *  GAURAV's SECTION START
     */
    void VEPewPewAttack()
    {
        particleEffect.GetComponent<TranslateMove>().m_fowardMove = false;
        particleEffect.GetComponent<TranslateMove>().m_upMove = true;
        particleEffect.GetComponent<TranslateMove>().m_rightMove = false;

    }




    /**
     *  GAURAV's SECTION END
    */
    //------------------------------------------
    /**
    *  CLAYTON's SECTION START
    */





    /**
     *  CLAYTONS's SECTION END
    */
    //------------------------------------------
    /**
    *  JAAMI's SECTION START
    */





    /**
     *  JAAMI's SECTION END
    */
    //------------------------------------------
}
