using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class forklift : MonoBehaviour
{
    public float speed;
    public string scene;
    public float dist;
    public Transform ridePos;
    bool ride = false;
    float lerpSpeed = 0;
    void Update()
    {
        Transform obj = gameObject.transform;
        if (FindAnyObjectByType<player_main>() != null)
            obj = FindAnyObjectByType<player_main>().transform;
        else
        {
            unit[] units = FindObjectsByType<unit>(FindObjectsSortMode.None);
            for(int i = 0; i < units.Count(); i++)
            {
                if (units[i].isControlled)
                {
                    obj = units[i].transform;
                }
            }
        }
            
        if (Vector2.Distance(transform.position, obj.position) < dist)
        {
            if (Application.isMobilePlatform && mobile.isInteracting ||
                !Application.isMobilePlatform && Input.GetKeyDown(KeyCode.Z))
            {
                ride = true;
                Debug.Log("enjoy the ride!");
            }
        }
        if (ride)
        {
            lerpSpeed = Mathf.MoveTowards(lerpSpeed, speed, speed/5);
            transform.Translate(Vector2.up * lerpSpeed * Time.deltaTime);
            obj.position = ridePos.position;
        }
    }
}
