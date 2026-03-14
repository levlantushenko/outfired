using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogue_trigger : dialogue
{
    public string[] lines;
    public Sprite[] icons;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            StartCoroutine(Say(lines, icons));
        }
    }
    public void Tell() => StartCoroutine(Say(lines, icons));

}
