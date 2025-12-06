using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public float hp;
    public GameObject hitEff;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "slash")
        {
            GameObject eff = Instantiate(hitEff, transform.position, transform.rotation);
            float effDir = Control.normal(transform.position.x - collision.transform.position.x);
            eff.transform.localScale = Vector3.one;
            eff.transform.eulerAngles = new Vector3(0, 90 * effDir, 0);
            Destroy(eff, 1f);

            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            hp -= collision.gameObject.GetComponent<slash>().damage;
            achievments.pacifist = false;
        }
        if (collision.gameObject.tag == "explosion")
        {
            hp -= 5;
            PlayerPrefs.SetInt(name + " hp", (int)hp);
            achievments.pacifist = false;
        }
    }
}
