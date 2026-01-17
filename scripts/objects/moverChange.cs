using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moverChange : MonoBehaviour
{
    public enum MChange
    {
        Destroy,
        Redirect,
        Boost,
        Stop
    }
    public MChange type;
    public float colorLerpT;
    [Header("--- Destroy ---")]
    [Space(5)]
    public float delay;
    [Space(5)]
    [Header("--- Redirect ---")]
    [Space(5)]
    public mover.dir redirTo;
    [Space(5)]
    [Header("--- Boost ---")]
    [Space(5)]
    public float boost;
    [Space(5)]
    [Header("--- Stop ---")]
    [Space(5)]
    public Color stopCol;

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("mover"))
        {
            mover _mover = collision.gameObject.GetComponent<mover>();
            SpriteRenderer spr = collision.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            Color startCol = spr.color;
            switch (type)
            {
                case MChange.Destroy:
                    _mover.speed = 0;
                    yield return new WaitForSeconds(delay);
                    Destroy(_mover.gameObject);
                    break;
                case MChange.Redirect:
                    _mover.fin = redirTo;
                    break;
                case MChange.Stop:
                    _mover.speed = 0;
                    spr.color = stopCol;
                    break;
                case MChange.Boost:
                    _mover.speed += boost;
                    break;
            }
        }
    }
    
}
