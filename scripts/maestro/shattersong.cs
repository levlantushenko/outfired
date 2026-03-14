using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shattersong : MonoBehaviour
{
    public GameObject str;
    public GameObject warn;
    public float delay;
    public float cooldown;
    public float warnT;
    public float attT;
    public float speed;

    Transform pl;

    private IEnumerator Start()
    {
        pl = GameObject.Find("player").transform;
        yield return new WaitForSeconds(delay);
        StartCoroutine(strike());
    }
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    IEnumerator strike()
    {
        str.transform.position = new Vector2(str.transform.position.x, pl.position.y);
        warn.transform.position = new Vector2(str.transform.position.x, pl.position.y);
        warn.SetActive(true);

        yield return new WaitForSeconds(warnT);
        warn.SetActive(false);
        str.SetActive(true);

        yield return new WaitForSeconds(attT);
        str.SetActive(false);

        yield return new WaitForSeconds(cooldown);
        StartCoroutine(strike());
    }
}
