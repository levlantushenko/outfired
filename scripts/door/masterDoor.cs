using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class masterDoor : MonoBehaviour
{
    public static int current;
    
    bool check = false;
    public Animator door;
    public string key;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;
        if (PlayerPrefs.HasKey(key) && !check)
        {
            check = true;
            current++;
            GetComponent<SpriteRenderer>().color = Color.white;
            if (current == 6)
                door.SetTrigger("open");
        }
    }
}
