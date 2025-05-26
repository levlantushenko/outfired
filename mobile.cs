using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobile : MonoBehaviour
{
    public GameObject player = null;
    private void Awake()
    {
        //if (!Application.isMobilePlatform)
        //    gameObject.SetActive(false);
    }
    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("player is missing!");
            if(FindAnyObjectByType<player_main>(FindObjectsInactive.Include) != null)
                player = FindAnyObjectByType<player_main>(FindObjectsInactive.Include).gameObject;
            else
            {
                Debug.Log("Searching for unit");
                unit[] list = FindObjectsByType<unit>(FindObjectsSortMode.None);
                for(int i = 0; i < list.Length; i++)
                {
                    player = list[i].gameObject;
                    if (!player.GetComponent<unit>().isControlled)
                        player = null;
                    if (player != null)
                    {
                        Debug.Log("player found! type : unit");
                        break;  
                    }

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
            pl.Dash();
        else
            player.GetComponent<unit>().Dash();
    }
    public void GetControl()
    {
        Debug.Log("void started");
        if (player.TryGetComponent<player_main>(out player_main pl))
            pl.GetControl();
        else
            Debug.LogWarning("err : no player found");
    }
    public void Attack()
    {
        if (!player.TryGetComponent<player_main>(out player_main pl))
            player.GetComponent<unit>().Attack();
    }
    public static bool isInteracting = false;
    public void Interact()
    {
        isInteracting = true;
        Invoke("StopInteracting", 0.1f);
    }
    void StopInteracting() => isInteracting = false;
}
