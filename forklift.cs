using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class forklift : MonoBehaviour
{
    public float speed;
    public string scene;
    public float dist;
    public Transform ridePos;
    public Transform[] stopPoints;
    bool ride = false;
    float lerpSpeed = 0;
    public bool isOnTop = false;
    void Update()
    {

        transform.Translate(Vector2.up * lerpSpeed * Time.deltaTime);
        Transform obj = gameObject.transform;
        if (FindAnyObjectByType<player_main>() != null)
            obj = FindAnyObjectByType<player_main>().transform;
        else
            return;
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
