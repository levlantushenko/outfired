using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class unit : MonoBehaviour
{
    public bool isControlled;
    [Header("enemy")]
    public float hp;
    public float controlHp;
    public float dist;
    public float attDist;
    bool spotted = false;
    Animator anim;
    [Header("movement")]
    public Transform groundCheck;
    public LayerMask lay;
    public float force;
    public float speed;
    public Transform sc;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Transform enemy = FindAnyObjectByType<player_main>().transform;
        if (Vector2.Distance(transform.position, enemy.position) < dist)
        {
            anim.SetBool("chase", true);
            transform.localScale = new Vector2(posDifference(transform.position.x, enemy.position.x), transform.localScale.y);
            transform.Translate(Vector3.left * transform.localScale.x * speed * Time.deltaTime);
        }
        else
            anim.SetBool("chase", false);
        if (Vector2.Distance(transform.position, enemy.position) < attDist)
            anim.SetTrigger("attack");
        

        if (!isControlled) return;
        Control.Move(gameObject, speed, sc);
        Control.Jump(gameObject, groundCheck, lay, force);
        
    }
    float posDifference(float a, float b)
    {
        return (a - b) / Mathf.Abs(a - b);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dist);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attDist);
    }
}
