using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class diver : MonoBehaviour
{
    Vector3 pl;
    Animator anim;
    [Header("--- main ---")]
    [Space(2)]
    public float hp;
    public Transform[] points;
    public GameObject hitEff;
    public float stunT;
    public float[] stunPoints;
    public float rageHp;
    public float rageImprovement;
    [Space(2)]
    [Header("--- dive attack ---")]
    [Space(2)]
    public GameObject diveEff;
    public GameObject splashEff;
    public float diveSpeed;
    public float afterWait;
    public string diveAnim;
    bool isDiving = true;
    float defHeight;
    [Space(2)]
    [Header("--- slash attack ---")]
    [Space(2)]
    public GameObject slashObj;
    public float slashSpd;
    [Space(2)]
    [Header("--- jump attack ---")]
    [Space(2)]
    public Transform air;
    public GameObject fallEff;
    public float flySpeed;
    public float diveDelay;
    public float platRegenT;
    public float fallSpd;
    public float jumpDelay;

    Collider2D fallEffColl;
    void Start()
    {
        fallEffColl = fallEff.GetComponentInChildren<Collider2D>();
        anim = GetComponent<Animator>();
        pl = FindAnyObjectByType<player_main>().transform.position;
        defHeight = transform.position.y;
    }
    bool isAttacking = false;
    // Update is called once per frame
    void Update()
    {
        splashEff.transform.position = new Vector2(transform.position.x, splashEff.transform.position.y);
        if (isAttacking) return;
        int attack = Random.Range(0, 3);
        switch (attack)
        {
            case 0:
                StartCoroutine(dive());
                break;
            case 1:
                StartCoroutine(slash());
                break;
            case 2:
                StartCoroutine(jump());
                break;
        }
        isAttacking = true;
    }
    float diveVerOffset = -7f;
    float baseAnimSpd = 1;
    float ramDelay = 1.3334f;
    float splashDelay = 2;
    //while (<condition>) {
    //  <body>
    //  yield return new WaitForNextFrameUnit();
    //}
    //-- use this structure to create a pocket update, that will work while condition is complied
    IEnumerator dive()
    {
        if (pl.x < transform.position.x)
        {
            transform.localScale = new Vector2(-1.5f, 3);
        }
        else
        {
            transform.localScale = new Vector2(1.5f, 3);
        }
        anim.SetTrigger("dive");
        isDiving = true;
        Transform pointTr = points[Random.Range(0, points.Length)];
        while(!pointTr.gameObject.activeInHierarchy)
            pointTr = points[Random.Range(0, points.Length)];
        Vector2 point = pointTr.position;
        //dive
        while(Mathf.Abs(transform.position.y - (point.y + diveVerOffset)) > 0.1f)
        {
            transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, point.y + diveVerOffset, 5 * Time.deltaTime));
            yield return new WaitForNextFrameUnit();
        }
        yield return new WaitForSeconds(0.2f);
        //swimming
        diveEff.SetActive(true);

        while (transform.position.x != point.x)
        {
            yield return new WaitForNextFrameUnit();
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(point.x, transform.position.y), diveSpeed * Time.deltaTime);
        }

        yield return new WaitForSeconds(splashDelay);
        //splashing
        diveEff.SetActive(false);
        splashEff.SetActive(true);
        anim.SetTrigger("splash");
        while (Mathf.Abs(transform.position.y - defHeight) > 0.1f)
        {
            transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, defHeight, 5f * Time.deltaTime));
            yield return new WaitForNextFrameUnit();
        }
        //ending splash
        splashEff.SetActive(false);
        if (pl.x < transform.position.x)
        {
            transform.localScale = new Vector2(-1.5f, 3);
        }
        else
        {
            transform.localScale = new Vector2(1.5f, 3);
        }
        yield return new WaitForSeconds(afterWait);

        isAttacking = false;
    }

    IEnumerator slash()
    {
        //interrupting if any of the platforms is disabled
        for(int i = 0; i<points.Length; i++)
        {
            if (points[i].gameObject.activeSelf)
            {
                isAttacking = false;
                StopCoroutine(slash());
                break;
            }
        }
        //starting attack

        anim.SetTrigger("slash");
        float pointX;

        if (pl.x < transform.position.x)
        {
            pointX = points[0].transform.position.x;
            transform.localScale = new Vector2(-1.5f, 3);
        }
        else
        {
            pointX = points[^1].transform.position.x;
            transform.localScale = new Vector2(1.5f, 3);
        }

        yield return new WaitForSeconds(ramDelay);
        //ramming
        anim.speed = 0;

        while(transform.position.x != pointX)
        {
            transform.position = new Vector2(Mathf.MoveTowards(transform.position.x, pointX, slashSpd * Time.deltaTime),
                transform.position.y);
            Debug.Log(Time.deltaTime);
            yield return new WaitForNextFrameUnit();
        }
        //ending attack

        anim.speed = baseAnimSpd;

        yield return new WaitForSeconds(afterWait);
        isAttacking = false;
    }

    IEnumerator jump()
    {
        Transform point = points[Random.Range(0, points.Length)];
        while (!point.gameObject.activeInHierarchy)
            point = points[Random.Range(0, points.Length)];
        Vector2 airPoint = new Vector2(point.position.x, air.position.y);
        if (airPoint.x < transform.position.x)
        {
            transform.localScale = new Vector2(-1.5f, 3);
        }
        else
        {
            transform.localScale = new Vector2(1.5f, 3);
        }
        anim.SetTrigger("jump");
        yield return new WaitForSeconds(jumpDelay);
        //jump
        while (Mathf.Abs(transform.position.y - airPoint.y) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, airPoint, flySpeed * Time.deltaTime);
            yield return new WaitForNextFrameUnit();
        }
        //fall
        yield return new WaitForSeconds(diveDelay);
        switch (Random.Range(0, 2))
        {
            case 0:
                StartCoroutine(dive());
                yield return new WaitForSeconds(0.3f);
                point.gameObject.SetActive(false);
                yield return new WaitForSeconds(platRegenT);
                point.gameObject.SetActive(true);
                break;
            case 1:
                while (transform.position.y != defHeight)
                {
                    transform.position = new Vector2(transform.position.x, Mathf.MoveTowards(transform.position.y, defHeight, fallSpd * Time.deltaTime));
                    yield return new WaitForNextFrameUnit();
                }
                fallEff.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                fallEffColl.enabled = false;
                yield return new WaitForSeconds(0.9f);
                fallEff.SetActive(false);
                fallEffColl.enabled = true;
                yield return new WaitForSeconds(0.5f);
                isAttacking = false;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "slash" || collision.gameObject.name.Contains("diver")) return;
        Debug.Log("collide");
        GameObject eff = Instantiate(hitEff, transform.position, Quaternion.Euler(0, -90, 0));
        float effDir = Control.normal(transform.position.x - collision.transform.position.x);
        eff.transform.localScale = Vector3.one;
        eff.transform.eulerAngles = new Vector3(0, 90 * effDir, 0);
        Destroy(eff, 1f);

        hp--;
        if(hp == rageHp)
        {
            Debug.Log("rage");
            StopAllCoroutines();
            StartCoroutine(rage());
        } else if (stunPoints.Contains(hp))
        {
            Debug.Log("stun");
            StopAllCoroutines();
            StartCoroutine(stun());
        }else if (hp <= 0)
        {
            StopAllCoroutines();
            anim.SetTrigger("death");
            GetComponent<diver>().enabled = false;
        }
    }
    IEnumerator rage()
    {
        diveEff.SetActive(false);
        fallEff.SetActive(false);
        hitEff.SetActive(false);
        splashEff.SetActive(false);

        isAttacking = true;
        anim.SetTrigger("rage");

        while (Mathf.Abs(transform.position.y - defHeight) > 0.1)
        {
            transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, defHeight, diveSpeed));
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(3);
        //rage mode


        jumpDelay /= rageImprovement;
        splashDelay /= rageImprovement;
        diveSpeed *= rageImprovement;
        ramDelay /= rageImprovement;
        baseAnimSpd = rageImprovement;
        anim.speed = rageImprovement;
        afterWait /= rageImprovement;
        diveDelay /= rageImprovement;
        platRegenT *= rageImprovement;
        isAttacking = false;
    }
    IEnumerator stun()
    {
        diveEff.SetActive(false);
        fallEff.SetActive(false);
        hitEff.SetActive(false);
        splashEff.SetActive(false);

        isAttacking = true;
        anim.SetTrigger("stun");

        while(Mathf.Abs(transform.position.y - defHeight) > 0.1)
        {
            transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, defHeight, diveSpeed));
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(stunT);
        StartCoroutine(dive());
    }
    
}
