using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tp_cannon : MonoBehaviour
{
    public Vector2 force;
    public float dist;
    Transform obj;
    public GameObject eff;

    void Update()
    {
        if(obj == null)
            obj = FindAnyObjectByType<player_main>().transform;
        if (obj == null) return;
        if (Vector2.Distance(transform.position, obj.position) < dist)
        {
            if (Application.isMobilePlatform && mobile.isInteracting ||
                !Application.isMobilePlatform && Input.GetKeyDown(KeyCode.Z))
            {
                Step1();
            }
        }
    }
    void Step1()
    {
        obj.position = transform.position;
        obj.gameObject.SetActive(false);
        Invoke("Step2", 1);
    }
    void Step2()
    {
        obj.gameObject.SetActive(true);
        obj.GetComponent<Rigidbody2D>().velocity = force;
        Instantiate(eff, transform.position, transform.rotation);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dist);
    }
}
