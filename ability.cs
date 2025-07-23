using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ability : MonoBehaviour
{
    public string _name;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
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
    }
}
