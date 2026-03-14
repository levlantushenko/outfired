using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class PP_check : MonoBehaviour
{
    public string name;
    public int val;
    public string trigger;
    bool act = true;
    public bool activate = false;
    public bool start;
    public bool invert = false;
    private void Awake()
    {
        if (!start) return;
        if (!PlayerPrefs.HasKey(name))
        {
            if(activate) gameObject.SetActive(invert);
            return;
        }
        if (!activate)
        {
            if (PlayerPrefs.GetInt(name) == val)
            {
                GetComponent<Animator>().SetBool(trigger, true);
                act = false;
            }
            else
            {
                GetComponent<Animator>().SetBool(trigger, false);
            }
        }
        else
            gameObject.SetActive((PlayerPrefs.GetInt(name) == val) == !invert);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activate)
        {
            if (collision.gameObject.tag != "Player") return;
            if(PlayerPrefs.GetInt(name) == val && act)
            {
                GetComponent<Animator>().SetBool(trigger, true);
                act = false;
            }
        }
    }
    public void Check(string cur)
    {
        if (!PlayerPrefs.HasKey(cur)) return;
        if (!activate)
        {
            if (PlayerPrefs.GetInt(cur) == val)
            {
                GetComponent<Animator>().SetBool(trigger, true);
                act = false;
            }
            else
            {
                GetComponent<Animator>().SetBool(trigger, false);
            }
        }
        else
            gameObject.SetActive(PlayerPrefs.GetInt(cur) == val);

    }
    public void ResetBool() => GetComponent<Animator>().SetBool(trigger, false);
}
