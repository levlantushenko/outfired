using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class maestro : MonoBehaviour
{
    public float afterWait;
    public float hp;
    public GameObject hitEff;
    [Space(5)]

    [Header("---- Teleport ----")]

    [Space(5)]
    public GameObject tpEff;
    float curPoint;
    public Transform[] tpPoints;
    [Space(5)]

    [Header("---- Wind Attack ----")]

    [Space(5)]
    public GameObject horWind;
    public GameObject horWarn;
    public GameObject verWind;
    public GameObject verWarn;
    public float windDelay;
    public float windT;
    [Space(5)]

    [Header("---- String attack ----")]

    [Space(5)]
    public Transform[] strings;
    public Transform[] strWarns;
    public float strDelay;
    public float strT;
    [Space(5)]

    [Header("---- Phase 2 ----")]

    [Space(5)]
    public float phase2StartT;
    public GameObject wallBreack;
    public GameObject wall;
    public GameObject newCL;
    public GameObject oldCL;
    public GameObject chaser;



    Transform pl;
    bool started = false;
    bool isAttacking = false;
    int phase = 1;
    private void Start()
    {
        pl = GameObject.Find("player").transform;
    }
    void Update()
    {
        if(started && !isAttacking)
            switch (Random.Range(0, 3))
            {
                case 0:
                    StartCoroutine(Wind());
                    break;
                case 1:
                    StartCoroutine(Strings());
                    break;
                case 2:
                    StartCoroutine(Teleport());
                    break;
            }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("pl") || !collision.gameObject.CompareTag("slash")) return;
        if (collision.gameObject.tag == "slash")
        {
            GameObject eff = Instantiate(hitEff, transform.position, transform.rotation);
            float effDir = _Control.normal(transform.position.x - collision.transform.position.x);
            eff.transform.localScale = Vector3.one;
            eff.transform.eulerAngles = new Vector3(0, 90 * effDir, 0);
            Destroy(eff, 1f);

            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            hp -= collision.gameObject.GetComponent<slash>().damage;
            if(hp <= 0)
            {
                phase = 2;
                StopAllCoroutines();
                foreach(Transform strWarn in strWarns)
                    strWarn.gameObject.SetActive(false);
                foreach (Transform strWarn in strings)
                    strWarn.gameObject.SetActive(false);
                horWarn.SetActive(false);
                horWind.SetActive(false);
                verWarn.SetActive(false);
                verWind.SetActive(false);
                StartCoroutine(phase2Start());
            }

        }
        if (collision.gameObject.tag == "explosion")
        {
            hp -= 5;
        }
        started = true;
        hp--;
    }
    #region phase 1
    IEnumerator Wind()
    {
        isAttacking = true;

        horWarn.transform.position = new Vector2(horWarn.transform.position.x, pl.position.y);
        horWind.transform.position = new Vector2(horWind.transform.position.x, pl.position.y);
        verWarn.transform.position = new Vector2(pl.position.x, verWarn.transform.position.y);
        verWind.transform.position = new Vector2(pl.position.x, verWind.transform.position.y);

        horWarn.SetActive(true);
        verWarn.SetActive(true);

        yield return new WaitForSeconds(windDelay);

        horWind.SetActive(true);
        horWarn.SetActive(false);
        verWind.SetActive(true);
        verWarn.SetActive(false);

        yield return new WaitForSeconds(windT);

        horWind.SetActive(false);
        verWind.SetActive(false);

        yield return new WaitForSeconds(afterWait);

        isAttacking = false;
    }

    IEnumerator Strings()
    {
        isAttacking = true;

        for(int i = 0; i < strWarns.Length; i++)
        {
            strWarns[i].position = pl.position;
            strWarns[i].rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
            strWarns[i].gameObject.SetActive(true);

            strings[i].position = pl.position;
            strings[i].rotation = strWarns[i].rotation;
        }
        yield return new WaitForSeconds(strDelay);

        for(int i = 0; i < strings.Length; i++)
        {
            strWarns[i].gameObject.SetActive(false);
            strings[i].gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(strT);

        for (int i = 0; i < strings.Length; i++)
        {
            strings[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    IEnumerator Teleport()
    {
        isAttacking = true;
        Instantiate(tpEff, transform.position, transform.rotation);
        if(curPoint == 0)
        {
            curPoint++;
            transform.position = tpPoints[1].position;
        }
        else
        {
            curPoint--;
            transform.position = tpPoints[0].position;
        }
        Instantiate(tpEff, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
    #endregion

    IEnumerator phase2Start()
    {
        isAttacking = true;
        Instantiate(tpEff, transform.position, transform.rotation);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(phase2StartT);

        wall.SetActive(false);
        wallBreack.SetActive(true);
        chaser.SetActive(true);
        oldCL.SetActive(false);
        newCL.SetActive(true);
    }
}
