using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paralax : MonoBehaviour
{
    public GameObject player;
    public float distance;

    void findPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("player is missing!");
            if (FindAnyObjectByType<player_main>(FindObjectsInactive.Include) != null)
                player = FindAnyObjectByType<player_main>(FindObjectsInactive.Include).gameObject;
            else
            {
                Debug.Log("Searching for unit");
                Unit[] list = FindObjectsByType<Unit>(FindObjectsSortMode.None);
                for (int i = 0; i < list.Length; i++)
                {
                    player = list[i].gameObject;
                    if (!player.GetComponent<Unit>().isControlled)
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
    void Update()
    {
        if(player == null) findPlayer();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        transform.position -= new Vector3(rb.velocity.x * Time.deltaTime / distance, rb.velocity.y * Time.deltaTime / distance);
    }
}
