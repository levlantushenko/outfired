using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crunching : MonoBehaviour
{
    public float regenT;
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.1f);
        Invoke("Regenerate", regenT);
    }
    void Regenerate()
    {
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
