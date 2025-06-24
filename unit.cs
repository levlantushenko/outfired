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
    public float X;
    public float Y;
    //private void Awake()
    //{
    //    input = new _InputSystem();
    //    input.Enable();
    //    input.normal.X.performed += ctx =>
    //    {
    //        X = ctx.ReadValue<float>();
    //    };
    //    input.normal.Y.performed += ctx =>
    //    {
    //        Y = ctx.ReadValue<float>();
    //    };
    //}
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
    Joystick joy = null;
    public float dashSpd;
    public float dashDur;
    public GameObject origin;
    public float explForce;
    public GameObject expl;

    private void Start()
    {
        if(Application.isMobilePlatform)
            joy = FindAnyObjectByType<Joystick>();
        anim = GetComponent<Animator>();
        conf = FindAnyObjectByType<CinemachineConfiner2D>();

    }
    private void Update()
    {
        if(isControlled && hp <= 0)
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
                transform.localScale *= new Vector2(posDifference(transform.position.x, enemy.position.x), 1);
                transform.Translate(Vector3.left * transform.localScale.x * speed * Time.deltaTime);
            }
            else
                anim.SetBool("chase", false);
                if (Vector2.Distance(transform.position, enemy.position) < attDist)
                    anim.SetTrigger("attack");
        }
        if (!isControlled) return;
        Animate();
        if (!Application.isMobilePlatform)
            Control.Move(gameObject, speed, sc, true, X);
        else
            Control.Move(gameObject, speed, sc, joy, true);
        if(Input.GetKeyDown(KeyCode.Z))
            Jump();
        if (Input.GetKeyDown(KeyCode.X))
            Attack();
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
            Invoke("stopDash", dashDur);
        }
        if (Input.GetKeyDown(KeyCode.A) && isControlled)
            Explode();
    }
    public void Animate()
    {
        if (Application.isMobilePlatform) return;
        if (Input.GetAxis("Horizontal") != 0) anim.SetBool("chase", true);
        else anim.SetBool("chase", false);
    }
    public void Animate(Joystick joy, Animator anim)
    {
        if (joy.Horizontal != 0) anim.SetBool("chase", true);
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
    public void Dash() => Control.Dash(gameObject, dashSpd, new Vector2(X, Y));
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
            hp -= 1;
        if(collision.gameObject.tag == "explosion")
        {
            hp -= 5;
        }
    }
}
