using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region input system init
    _InputSystem input;
    public Vector2 axis;
    public float jump;
    public float dash;
    public float attack;
    public float control;
    private void Start()
    {
        input = new _InputSystem();
        input.Enable();

        input.normal.Move.performed += ctx => axis = ctx.ReadValue<Vector2>();
        input.normal.Move.canceled += ctx => axis = Vector2.zero;

        input.normal.Jump.performed += ctx => jump = ctx.ReadValue<float>();
        input.normal.Jump.canceled += ctx => jump = 0;

        input.normal.Dash.performed += ctx => dash = ctx.ReadValue<float>();
        input.normal.Dash.canceled += ctx => dash = 0;

        input.normal.Attack.performed += ctx => attack = ctx.ReadValue<float>();
        input.normal.Attack.canceled += ctx => attack = 0;

        input.normal.Control.performed += ctx => control = ctx.ReadValue<float>();
        input.normal.Control.canceled += ctx => control = 0;

        originSc = transform.localScale;
        anim = GetComponent<Animator>();
        conf = FindAnyObjectByType<CinemachineConfiner2D>();
        pixPerHp = hpBar.transform.localScale.x / hp;
        startScale = transform.localScale;
    }
    #endregion
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
    public float dashSpd;
    public float dashDur;
    public GameObject origin;
    public float explForce;
    public GameObject expl;
    Vector2 originSc;
    [Header("HP display")]
    public float pixPerHp;
    public GameObject hpBar;
    public Vector2 startScale;
    
    private void Update()
    {
        hpBar.transform.localScale = new Vector3(hp * pixPerHp, hpBar.transform.localScale.y);
        if (hp <= controlHp) hpBar.GetComponent<SpriteRenderer>().color = Color.yellow;
        if (isControlled && hp <= 0)
        {
            hp = 1;
            Debug.Log("unit rebirthed!");
            anim.SetBool("isAlive", true);
            GetComponent<Collider2D>().enabled = true;
            GetComponent<Rigidbody2D>().simulated = true;
        }
        if(hp<=0)
        {
            anim.SetBool("isAlive", false);
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
                Unit[] units = FindObjectsByType<Unit>(FindObjectsSortMode.None);
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
                transform.localScale = new Vector2(startScale.x * posDifference(transform.position.x, enemy.position.x), 1);
                transform.Translate(Vector3.left * transform.localScale.x * speed * Time.deltaTime);
            }
            else
                anim.SetBool("chase", false);
                if (Vector2.Distance(transform.position, enemy.position) < attDist)
                    anim.SetTrigger("attack");
        }
        if (!isControlled) return;
        Animate();
        Control.Move(gameObject, speed, sc, true, axis.x, originSc);
        if(jump != 0)
            Jump();
        if (attack != 0)
            Attack();
        if (dash != 0)
        {
            Dash();
            Invoke("stopDash", dashDur);
        }
        if (control != 0 && isControlled)
            Explode();
        attack = 0;
        control = 0;
    }
    public void Animate()
    {
        if (axis.x != 0) anim.SetBool("chase", true);
        else anim.SetBool("chase", false);
    }
    
    float posDifference(float a, float b)
    {
        return (a - b) / Mathf.Abs(a - b);
    }
    bool explodable = false;
    public void BombTime() => explodable = true;
    public void Explode() { 
        if (explodable) Control.Explode(origin, transform, explForce, expl);
    }
    public void Jump() => Control.Jump(gameObject, groundCheck, lay, force);
    public void Attack() => Control.Attack(transform, slash, attPos, false, transform);
    public void Dash() => Control.Dash(gameObject, dashSpd, axis);
    public void StopDash() => Control.DashStop(gameObject);


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
        if (collision.gameObject.tag == "slash")
        {
            hp -= 1;
        }
        if(collision.gameObject.tag == "explosion")
        {
            hp -= 5;
        }
    }
}
