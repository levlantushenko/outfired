using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quest : MonoBehaviour
{
    public Transform target;
    public Transform place;
    public static float reachDist = 2;
    Transform pl; 
    private void Awake()
    {
        pl = FindAnyObjectByType<player>().GetComponent<Transform>();
    }
    public enum questType
    {
        press,
        bring
    }
    public questType type;
    
    void Update()
    {
        if (Vector3.Distance(pl.position, target.position) < reachDist && type == questType.bring &&
            Input.GetKeyDown(KeyCode.E))
        {
            pl.GetComponent<player>().items.Add(target.GetComponent<item>());
            Destroy(target);
        }
        if (Vector3.Distance(pl.position, place.position) < reachDist &&
            pl.GetComponent<player>().items.Contains(target.GetComponent<item>()) && Input.GetKeyDown(KeyCode.E))
        {
            pl.GetComponent<player>().items.Remove(target.GetComponent<item>());
            place.GetComponent<Animator>().SetTrigger("opened");
        }

        if (Vector3.Distance(pl.position, target.position) < reachDist && type == questType.press && Input.GetKeyDown(KeyCode.E))
            place.GetComponent<Animator>().SetTrigger("opened");
    }
}
