using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
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
            transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, point.y + diveVerOffset, 5f * Time.deltaTime));
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

        yield return new WaitForSeconds(2f);
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
            pointX = points[points.Length-1].transform.position.x;
            transform.localScale = new Vector2(1.5f, 3);
        }

        yield return new WaitForSeconds(1.3334f);
        //ramming
        anim.speed = 0;

        while(transform.position.x != pointX)
        {
            transform.position = new Vector2(
                Mathf.MoveTowards(transform.position.x, pointX, slashSpd * Time.deltaTime), transform.position.y);

            yield return new WaitForNextFrameUnit();
        }
        //ending attack

        anim.speed = 1;

        yield return new WaitForSeconds(1);
        isAttacking = false;
    }

    IEnumerator jump()
    {
        Transform point = points[Random.Range(0, points.Length)];
        while (!point.gameObject.activeInHierarchy)
            point = points[Random.Range(0, points.Length)];
        Vector2 airPoint = new Vector2(point.position.x, air.position.y);
        anim.SetTrigger("jump");
        //dive
        while (Mathf.Abs(transform.position.y - airPoint.y) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, airPoint, flySpeed * Time.deltaTime);
            yield return new WaitForNextFrameUnit();
        }
        yield return new WaitForSeconds(diveDelay);
        switch (Random.Range(0, 2))
        {
            case 0:
                StartCoroutine(dive());
                yield return new WaitForSeconds(0.5f);
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
                yield return new WaitForSeconds(1f);
                fallEff.SetActive(false);

                isAttacking = false;
                break;
        }
    }
}
