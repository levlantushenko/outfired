using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class clock : MonoBehaviour
{
    public float t;
    public Color final;

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<SpriteRenderer>().color = final;
            yield return new WaitForSeconds(t);
            collision.gameObject.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }
}
