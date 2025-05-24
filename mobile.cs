using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobile : MonoBehaviour
{
    public GameObject player;
    private void Awake()
    {
        if(!Application.isMobilePlatform)
            gameObject.SetActive(false);
    }
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
    public void Jump(){
        if (player.TryGetComponent<player_main>(out player_main pl))
            pl.Jump();
        else
            player.GetComponent<unit>().Jump();
    }
    public void Dash()
    {
        if (player.TryGetComponent<player_main>(out player_main pl))
            pl.Jump();
        else
            player.GetComponent<unit>().Dash();
    }
    public void GetControl()
    {
        if (player.TryGetComponent<player_main>(out player_main pl))
            pl.GetControl();
    }
    public void Attack()
    {
        if (!player.TryGetComponent<player_main>(out player_main pl))
            player.GetComponent<unit>().Attack();
    }
}
