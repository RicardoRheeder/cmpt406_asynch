using System.Collections;
using UnityEngine;


public class SpecialEffect : MonoBehaviour
{
    private UnitType unitType;

    private GameObject gm;
    private GameObject gm2;

    public Transform particleEffect;
    public Transform startPoint;
    private float distance;

    private Vector3 sourceWorldPosition;
    private Vector3 targetWorldPosition;

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

    private AudioManager audioManager;

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

    public void PlayAttackEffect(Vector3 sourceWorldPos, Vector3 targetWorldPos, UnitType unitType, AudioManager audioManager = null)
    {
        this.unitType = unitType;
        distance = Vector3.Distance(sourceWorldPos, targetWorldPos);
//        print(distance);
        this.sourceWorldPosition = sourceWorldPos;
        this.targetWorldPosition = targetWorldPos;

        if (audioManager != null)
            this.audioManager = audioManager;
        

        switch (unitType)
        {
            //------------------------------------------
            /**
            *  TODO: RICARDO's SECTION START
            */
            case UnitType.compensator:
                StartCoroutine(VECompensatorAttack());
                break;

            case UnitType.foundation:
                StartCoroutine(VEFoundationAttack());
                break;

            case UnitType.piercing_tungsten:
               
                StartCoroutine(VETungstenAttack());
                break;

            /**
             *  RICARDO's SECTION END
            */
            //------------------------------------------
            /**
            *  TODO: GAURAV's SECTION START
            */

            case UnitType.trooper:
                StartCoroutine(VETrooperAttack());
                break;
            case UnitType.recon:
                StartCoroutine(VEReconAttack());
                break;
            case UnitType.support_sandman:
                StartCoroutine(VESandmanAttack());
                break;

            /**
             *  GAURAV's SECTION END
            */
            //------------------------------------------
            /**
            *  TODO: CLAYTON's SECTION START
            */

            case UnitType.pewpew:
                StartCoroutine(VEPewPewAttack());
                break;
            case UnitType.steamer:
                StartCoroutine(VESteamerAttack());
                break;
            case UnitType.light_adren:
                StartCoroutine(VEAdrenAttack());
                break;
            case UnitType.heavy_albarn:
                StartCoroutine(VEAlbarnAttack());
                break;

            /**
             *  CLAYTONS's SECTION END
            */
            //------------------------------------------
            /**
            *  TODO: JAAMI's SECTION START
            */

            case UnitType.claymore:
                StartCoroutine(VEClaymoreAttackv2());
                break;
            case UnitType.powerSurge:
                StartCoroutine(VEPowerSurgeAttack());
                break;
            case UnitType.midas:
                StartCoroutine(VEMidasAttack());
                break;

            /**
             *  JAAMI's SECTION END
            */
            //------------------------------------------

        }
    }


    //------------------------------------------
    /**
    *  TODO: RICARDO's SECTION START
    */
    IEnumerator VECompensatorAttack()
    {
        Coroutine attackSound = StartCoroutine(playAttackSound(4.2f));

        // WAIT ANY DESIRED AMOUNT OF TIME
        yield return new WaitForSeconds(2f);

        // CREATE AND SET EVERYTHING
        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        // PERFORM WHAT MY VECompensatorAttack() WOULD HAVE DONE
        StartCoroutine(findMakeObject());

        yield return new WaitForSeconds(7f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }


    IEnumerator VEFoundationAttack() {
        Coroutine attackSound = StartCoroutine(playAttackSound(6f));

        yield return new WaitForSeconds(3.5f);

        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        //TODO: Clean up script
        ChangeScaleGameObject(0.5f);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);
        gm.transform.localPosition = new Vector3(startPoint.localPosition.x + 0.60f, startPoint.localPosition.y, startPoint.localPosition.z);


        gm2 = Instantiate(particleEffect, this.transform).gameObject;
        gm2.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);
        gm.transform.localPosition = new Vector3(startPoint.localPosition.x + 0.60f, startPoint.localPosition.y, startPoint.localPosition.z);

        StartCoroutine(setArrowDirection());

        StartCoroutine(findMakeObject());


        yield return new WaitForSeconds(5f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    IEnumerator VETungstenAttack()
    {
        Coroutine attackSound = StartCoroutine(playAttackSound(0.7f));

        yield return new WaitForSeconds(4f);


        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        ChangeColor(Color.red);
        ChangeScale(3);
        StartCoroutine(findMakeObject());

        yield return new WaitForSeconds(5f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;

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

        Destroy(gm, 2f);
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

    IEnumerator playAttackSound(float secondsToWait = 0.1f)
    {
        yield return new WaitForSeconds(secondsToWait);
        if (audioManager != null)
            audioManager.Play(unitType, SoundType.Attack);
        yield return null;
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
        yield return new WaitForSeconds(0.5f);
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

    IEnumerator findMakeObjectSandman()
    {
        GameObject[] movePoints = GameObject.FindGameObjectsWithTag("MissileMovePoint");
        while (movePoints.Length != 3)
        {
//            print("Length: " + movePoints.Length);
            movePoints = GameObject.FindGameObjectsWithTag("MissileMovePoint");
            for (int i = 0; i < movePoints.Length-1; i++)
            {
                movePoints[i].transform.localPosition = new Vector3(0, 0, 1000);
            }

            yield return new WaitForSeconds(0.1f);
            if (movePoints.Length != 0)
            {
                movePoints[movePoints.Length - 1].transform.localPosition = new Vector3(0, 0, 1000);
            }
        }
        yield return null;
    }

    IEnumerator VETrooperAttack() {
        Coroutine attackSound = StartCoroutine(playAttackSound(4.5f));

        yield return new WaitForSeconds(4.5f);

        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        ChangeColor(new Color(0f, 255f, 0f));

        StartCoroutine(setArrowDirection());

        yield return new WaitForSeconds(4f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    IEnumerator VEReconAttack(){
        Coroutine attackSound = StartCoroutine(playAttackSound(0.1f));

        yield return new WaitForSeconds(0.1f);

        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        ChangeColor(new Color(66f, 244f, 244f));

        StartCoroutine(setArrowDirection());

        yield return new WaitForSeconds(3f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    IEnumerator VESandmanAttack()
    {
        Coroutine attackSound = StartCoroutine(playAttackSound(0.1f));

        yield return new WaitForSeconds(0.1f);
        if (audioManager != null)
            audioManager.Play(unitType, SoundType.Attack);

        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(targetWorldPosition.x, targetWorldPosition.y, targetWorldPosition.z);

        StartCoroutine(findMakeObjectSandman());

        yield return new WaitForSeconds(4f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    /**
     *  GAURAV's SECTION END
    */
    //------------------------------------------
    /**
    *  CLAYTON's SECTION START
    */

    IEnumerator moveShot(GameObject particle)
    {
        yield return new WaitForSeconds(0.01f);

        while (particle != null)
        {
            particle.transform.position = Vector3.MoveTowards(particle.transform.position, targetWorldPosition, 1f);
            yield return new WaitForSeconds(0.01f);
        }



        yield return null;
    }

    IEnumerator VEPewPewAttack()
    {
        Coroutine attackSound = StartCoroutine(playAttackSound(4f));

        yield return new WaitForSeconds(4f);
      
        for (int i = 0; i < 8; i++)
        {
            PewPewSingleShot();
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(4f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    void PewPewSingleShot()
    {
        GameObject gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        gm.GetComponent<TranslateMove>().m_fowardMove = false;
        gm.GetComponent<TranslateMove>().m_upMove = false;
        gm.GetComponent<TranslateMove>().m_rightMove = false;

        StartCoroutine(moveShot(gm));

        Destroy(gm, 2f);
    }

    IEnumerator VESteamerAttack()
    {
        Coroutine attackSound = StartCoroutine(playAttackSound(0.1f));

        yield return new WaitForSeconds(0.1f);

        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        yield return new WaitForSeconds(3f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    IEnumerator VEAdrenAttack()
    {
        Coroutine attackSound = StartCoroutine(playAttackSound(0.1f));

        yield return new WaitForSeconds(0.1f);

        GameObject gm2 = Instantiate(particleEffect.gameObject) as GameObject;
        gm2.transform.position = new Vector3(targetWorldPosition.x, targetWorldPosition.y, targetWorldPosition.z);
        gm2.transform.eulerAngles = new Vector3(0, 180, 0);


        yield return new WaitForSeconds(3f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    IEnumerator VEAlbarnAttack()
    {
        Coroutine attackSound = StartCoroutine(playAttackSound(0.1f));

        yield return new WaitForSeconds(0.1f);

        GameObject gm2 = Instantiate(particleEffect.gameObject) as GameObject;
        gm2.transform.position = new Vector3(targetWorldPosition.x, targetWorldPosition.y, targetWorldPosition.z);
        gm2.transform.eulerAngles = new Vector3(0, 180, 0);

        yield return new WaitForSeconds(5f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    /**
     *  CLAYTONS's SECTION END
    */
    //------------------------------------------
    /**
    *  JAAMI's SECTION START
    */
    IEnumerator VEClaymoreAttack()
    {
        yield return new WaitForSeconds(1f);
        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);
        gm.GetComponent<DelayObjectMake>().m_startDelay = 2f;
        ChangeColor(Color.red);
        GameObject FireBreathHole = null;
        while (FireBreathHole == null)
        {
            FireBreathHole = GameObject.Find("Effect_38_FireBreathHole(Clone)");

            yield return new WaitForSeconds(0.1f);
        }
        foreach (Transform child in FireBreathHole.transform)
        {
            child.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }

        StartCoroutine(findMakeObjectPowerClaymore());

        Destroy(gm, 5f);
        yield return null;
    }

    IEnumerator VEClaymoreAttackv2() {
        Coroutine attackSound = StartCoroutine(playAttackSound(3.5f));

        yield return new WaitForSeconds(3.5f);

        gm = Instantiate(effect47Explosion, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(targetWorldPosition.x, targetWorldPosition.y, targetWorldPosition.z);
        ChangeColor(Color.blue);
        ChangeScale(3f);

        yield return new WaitForSeconds(3f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    IEnumerator VEPowerSurgeAttack()
    {
        Coroutine attackSound = StartCoroutine(playAttackSound(3.5f));

        yield return new WaitForSeconds(3.5f);

        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        
        GameObject FireBreathHole = null;
        while (FireBreathHole == null)
        {
            FireBreathHole = GameObject.Find("Effect_38_FireBreathHole(Clone)");
           
            yield return new WaitForSeconds(0.1f);
        }
        foreach (Transform child in FireBreathHole.transform)
        {
            child.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }

        StartCoroutine(findMakeObjectPowerSurge());

        yield return new WaitForSeconds(3f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }

    IEnumerator VEMidasAttack()
    {
        Coroutine attackSound = StartCoroutine(playAttackSound(1f));

        yield return new WaitForSeconds(1f);

        gm = Instantiate(particleEffect, this.transform).gameObject;
        gm.transform.SetParent(this.transform);
        gm.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);

        StartCoroutine(findMakeObjectMidas());

        yield return new WaitForSeconds(3f);
        StopCoroutine(attackSound);
        Destroy(gm);
        yield return null;
    }
    
    IEnumerator findMakeObjectMidas()
    {
        GameObject MakeObject = null;
        while (MakeObject == null)
        {
            MakeObject = GameObject.Find("MakeObjectMidas");
            yield return new WaitForSeconds(0.1f);
        }
        MakeObject.transform.localPosition = new Vector3(0, 0, distance);
        yield return null;
    }
    IEnumerator findMakeObjectPowerClaymore()
    {
        GameObject MakeObject = null;
        while (MakeObject == null)
        {
            MakeObject = GameObject.Find("MakeObject");
            yield return new WaitForSeconds(0.1f);
        }
        MakeObject.transform.localPosition = new Vector3(0, 0, distance);
        GameObject FireBreath = GameObject.Find("Effect_38_FireBreath(Clone)");
        FireBreath.transform.position = MakeObject.transform.position;
        GameObject FireBreathField = null;
        while (FireBreathField == null)
        {
            FireBreathField = GameObject.Find("Effect_38_FireBreathField(Clone)");
            yield return new WaitForSeconds(0.1f);
        }
        FireBreathField.transform.localPosition = new Vector3(0, 0, 0);
        FireBreathField.transform.localEulerAngles = new Vector3(90, 0, 0);

        //Destroy(gm, 2f);
        yield return null;
    }
    IEnumerator findMakeObjectPowerSurge()
    {
        GameObject MakeObject = null;
        while (MakeObject == null)
        {
            MakeObject = GameObject.Find("MakeObject");
            yield return new WaitForSeconds(0.1f);
        }
        MakeObject.transform.localPosition = new Vector3(0, 0, distance);
        GameObject FireBreath = GameObject.Find("Effect_38_FireBreath(Clone)");
        if (FireBreath != null)
        {
            FireBreath.transform.position = MakeObject.transform.position;
            GameObject FireBreathField = null;
            while (FireBreathField == null) {
                FireBreathField = GameObject.Find("Effect_38_FireBreathField(Clone)");
                yield return new WaitForSeconds(0.1f);
            }
            FireBreathField.transform.localPosition = new Vector3(0, 0, 0);
            FireBreathField.transform.localEulerAngles = new Vector3(90, 0, 0);
        }
       

        yield return null;
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
            yield return new WaitForSeconds(0.01f);
        }
        MakeObject.transform.localPosition = new Vector3(0, 0, distance);
        yield return null;
    }
}
