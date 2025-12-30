using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogue_trigger : dialogue
{
    public string[] lines;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            StartCoroutine(Say(lines));
        }
    }
    public void Tell() => StartCoroutine(Say(lines));

}
