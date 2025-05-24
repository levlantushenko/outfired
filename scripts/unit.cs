using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class unit : MonoBehaviour
{
    public enum enTypes
    {
        Miner
    }
    public enTypes type;
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
    public CinemachineConfiner2D conf;
    public GameObject slash;
    public Transform attPos;

    private void Start()
    {
        anim = GetComponent<Animator>();
        conf = FindAnyObjectByType<CinemachineConfiner2D>();
    }
    private void Update()
    {
        if(hp<=0)
        {
            anim.SetTrigger("die");
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;
            return;
        }
        if (!isControlled)
        {
            Transform enemy = null;
            if (FindAnyObjectByType<player_main>() != null)
                enemy = FindAnyObjectByType<player_main>().transform;
            else
            {
                unit[] units = FindObjectsByType<unit>(FindObjectsSortMode.None);
                for (int i = 0; i < units.Count(); i++)
                {
                    if (units[i].isControlled && units[i].type != type)
                    {
                        enemy = units[i].transform;
                    }
                }
            }
                
            if (enemy == null) return;
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
        }
        if (!isControlled) return;
        Animate();
        Control.Move(gameObject, speed, sc, true);
        Control.Jump(gameObject, groundCheck, lay, force);
        if(Input.GetKeyDown(KeyCode.X))
            Control.Attack(transform, slash, attPos, false,  transform);
        
    }
    public void Animate()
    {
        if (Input.GetAxis("Horizontal") != 0) anim.SetBool("chase", true);
        else anim.SetBool("chase", false);

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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isControlled && collision.gameObject.layer == 3)
            Control.CameraControl(collision, conf);
        else if (collision.gameObject.tag == "slash")
            hp -= 1;
    }
}
