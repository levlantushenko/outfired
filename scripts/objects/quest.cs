using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quest : MonoBehaviour
{
    public Animator door;
    public Color newColor;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            door.SetTrigger("open");
            GetComponent<SpriteRenderer>().color = newColor;
        }
    }
}
