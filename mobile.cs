using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobile : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<player_main>().gameObject;
            if(player == null)
            {
                while(player != null)
                {
                    player = FindAnyObjectByType<unit>().gameObject;
                    if (!player.GetComponent<unit>().isControlled)
                        player = null;
                }
            }
        }
    }
    public void Jump() => player.GetComponent<player_main>().Jump();
    public void Dash() => player.GetComponent<player_main>().Dash();
}
