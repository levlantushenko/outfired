using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class clock : MonoBehaviour
{
    public float t;
    public Color final;
    public GameObject eff;
    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = final;
            Instantiate(eff, transform.position, transform.rotation);

            yield return new WaitForSeconds(t);

            collision.gameObject.transform.position = transform.position;
            yield return new WaitForEndOfFrame();
            collision.gameObject.transform.position = transform.position;

            Instantiate(eff, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }
}
