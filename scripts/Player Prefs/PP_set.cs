using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PP_set : MonoBehaviour
{
    public bool ability;
    public string _name;
    public int value = 0;
    /// <summary>
    /// should the object with the script react on the collision?
    /// </summary>
    public bool selfReacting = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && selfReacting)
        {
            if (ability)
            {
                if (!PlayerPrefs.HasKey("abilities"))
                    PlayerPrefs.SetString("abilities", "");
                if (!PlayerPrefs.GetString("abilities").Contains(_name))
                {
                    PlayerPrefs.SetString("abilities", PlayerPrefs.GetString("abilities") + _name);
                    player_main pl = FindAnyObjectByType<player_main>();
                    pl.checkPP();
                    Destroy(gameObject);
                }
            }
            else
                PlayerPrefs.SetInt(_name, 0);
        }
    }
    public void set()
    {
        if (ability)
        {
            if (!PlayerPrefs.HasKey("abilities"))
                PlayerPrefs.SetString("abilities", "");
            if (!PlayerPrefs.GetString("abilities").Contains(_name))
            {
                PlayerPrefs.SetString("abilities", PlayerPrefs.GetString("abilities") + _name);
                player_main pl = FindAnyObjectByType<player_main>();
                pl.checkPP();
                Destroy(gameObject);
            }
        }
        else
            PlayerPrefs.SetInt(_name, value);
    }
}
