using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class player : MonoBehaviour
{
    [Header("movement")]
    public float speed;
    public float force;
    public bool jump;
    public float jumpT;
    public float rememberTime;
    public int space;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask lay;
    public Transform SC;
    [Space(5)]
    [Header("attack")]
    public GameObject slash;
    public Transform attPos;
    public Transform shootP;
    public GameObject bullet;
    public float damage;
    [Space(5)]
    public float bulSpeed;
    [Header("camera control")]
    public CinemachineConfiner2D cam;
    public PolygonCollider2D cameraLimit;
    [Space(5)]
    [Header("inventory")]
    public List<item> items;
    public Canvas canv;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDashing) Move();
        Attack();
        cam.m_BoundingShape2D = cameraLimit;
        getControl();
        Dash();
    }
    private void Move()
    {
        #region space int
        if (Input.GetKeyDown(KeyCode.Z))
        {
            space = 1;
            Invoke("spaceClear", rememberTime);
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            Invoke("spaceClear", rememberTime);
        }
        #endregion
        if (Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, lay))
        {
            anim.SetBool("jump", false);
            if (space == 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, force);
                jump = true;
                anim.SetBool("jump", true);
            }
        }
        if (jump == true) anim.SetBool("jump", true);
        else anim.SetBool("jump", false);

        if (Input.GetKeyUp(KeyCode.Z) && jump == true && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.3f);
            jump = false;
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.velocity = new Vector2(speed * Input.GetAxis("Horizontal"), rb.velocity.y);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SC.localScale = new Vector3(-1, 1, 1);
            anim.SetInteger("run", -1);

        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SC.localScale = new Vector3(1, 1, 1);
            anim.SetInteger("run", 1);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            anim.SetInteger("run", 0);
    }
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameObject _slash = Instantiate(slash, attPos.position, attPos.rotation);
            _slash.transform.parent = transform;
            Destroy(_slash, 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject _bullet = Instantiate(bullet, attPos.position, shootP.rotation);
            _bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(bulSpeed * SC.localScale.x / Mathf.Abs(SC.localScale.y),
                0);
            Destroy(_bullet, 3f);
        }
    }
    [Header("Control")]
    GameObject controled = null;
    bool isControlling = false;
    public float controlDist = 4;
    public float encounterSpeed;
    void getControl()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            List<enemy> enemies = getWeakEnemy();
            if (enemies.Count == 0) return;
            float closestDist = controlDist;
            GameObject enemy = null;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (Vector3.Distance(enemies[i].transform.position, transform.position) < closestDist)
                {
                    closestDist = Vector3.Distance(enemies[i].transform.position, transform.position);
                    enemy = enemies[i].gameObject;
                }
            }
            transform.position = enemy.transform.position;
            player newPl = enemy.AddComponent<player>();
            newPl = gameObject.GetComponent<player>();
        }
    }
    List<enemy> getWeakEnemy()
    {
        List<enemy> enemies = new List<enemy>();
        enemy[] enemiesRaw = FindObjectsByType<enemy>(FindObjectsSortMode.None);
        foreach (enemy enemy in enemiesRaw)
        {
            Transform t = enemy.transform;
            if (enemy.hp <= enemy.controlHp && Vector2.Distance(t.position, transform.position) < controlDist)
                enemies.Add(enemy);
        }
        if (enemies.Count != 0)
            Debug.Log("enemies found!");
        else
            Debug.Log("enemies not found!");
        return enemies;
    }
    public void spaceClear()
    {
        space = 0;
    }
    public void spaceSet()
    {
        space = 1;
    }
    [Header("Dash")]
    public float dashSpd;
    public float dashDur;
    public float dashCD;
    public bool isDashAble = true;
    bool isDashing = false;
    public Vector2 bakedDashSpd;
    void Dash()
    {
        bakedDashSpd = new Vector2(dashSpd * Input.GetAxis("Horizontal"), dashSpd * Input.GetAxis("Vertical"));
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashAble)
        {
            rb.velocity = bakedDashSpd;
            isDashAble = false;
            isDashing = true;
            Invoke("DashStop", dashDur);
            Invoke("DashReset", dashCD);
        }
    }
    void DashStop()
    {
        rb.velocity = Vector2.zero;
        isDashing = false;
    }
    void DashReset() => isDashAble = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            Debug.Log("now starring : " + collision.name);
            cameraLimit = collision.gameObject.GetComponent<PolygonCollider2D>();
        }
    }
}
