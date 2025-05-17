using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public float hp;
    public float controlHp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "slash") return;
        Debug.Log("triggered!");
        Transform pl = FindAnyObjectByType<player>().transform;
        hp -= FindAnyObjectByType<player>().damage;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(
            posDifference(transform.position.x, pl.position.x) * 5, 
            posDifference(transform.position.y, pl.position.y) * 5), ForceMode2D.Impulse);
    }
    float posDifference(float a, float b)
    {
        return (a - b) / Mathf.Abs(a - b);
    }

}
