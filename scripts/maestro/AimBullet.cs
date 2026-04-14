using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBullet : MonoBehaviour
{
    public float speed;
    public float redirSpeed;
    public Transform pl;
    void Start()
    {
        pl = GameObject.Find("player").transform;
        transform.eulerAngles = new Vector3(0, 0, 
            Mathf.Atan2(pl.position.y - transform.position.y, pl.position.x - transform.position.x) * Mathf.Rad2Deg);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != pl.gameObject) return;
        GetComponent<Collider2D>().enabled = false;
    }
}
