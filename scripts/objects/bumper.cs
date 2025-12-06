using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bumper : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] float t;
    [SerializeField] float stunT = 1;
    public AudioClip clip;

    private void Start()
    {
        startSc = transform.localScale;
    }
    Vector2 startSc;
   

    // Update is called once per frame
    void Update()
    {
        if ((Vector2)transform.localScale != startSc)
            transform.localScale = Vector2.MoveTowards(transform.localScale, startSc, t * Time.deltaTime);
    }
    player_main pl;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!rech) return;
        if(collision.gameObject.name.Contains("pl") && collision.gameObject.CompareTag("slash"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.velocity = (Vector2)(collision.transform.position - transform.position).normalized * force + rb.velocity * -1;
            transform.localScale *= 1.5f;
            rech = false;
            Invoke("recharge", 0.5f);
            pl = FindAnyObjectByType<player_main>().GetComponent<player_main>();
            pl.isDashing = true;
            pl.isDashAble = true;
            Invoke("plDashReset", stunT);
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
        }    
    }
    bool rech = true;
    public void recharge() => rech = true;
    public void plDashReset() => pl.isDashing = false;
}
