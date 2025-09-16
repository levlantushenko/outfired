using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PP_check : MonoBehaviour
{
    public string name;
    public int val;
    public string trigger;
    bool act = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;
        if(PlayerPrefs.GetInt(name) == val && act)
        {
            GetComponent<Animator>().SetBool(trigger, true);
            act = false;
        }
    }
    public void Check(string cur)
    {
        if (!PlayerPrefs.HasKey(cur)) return;
        if(PlayerPrefs.GetInt(cur) == val)
        {
            GetComponent<Animator>().SetBool(trigger, true);
            act = false;
        }
        else
        {
            GetComponent<Animator>().SetBool(trigger, false);
        }

    }
    public void ResetBool() => GetComponent<Animator>().SetBool(trigger, false);
}
