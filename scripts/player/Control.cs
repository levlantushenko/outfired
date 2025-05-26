using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Control : MonoBehaviour
{
    public static void Move(GameObject gm, float speed, Transform SC, bool isInverted)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.velocity = new Vector2(speed * Input.GetAxis("Horizontal"), rb.velocity.y);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!isInverted) SC.localScale = new Vector3(-1, 1, 1);
            else SC.localScale = new Vector3(1, 1, 1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!isInverted) SC.localScale = new Vector3(1, 1, 1);
            else SC.localScale = new Vector3(-1, 1, 1);
        }
    }
    public static void Move(GameObject gm, float speed, Transform SC, Joystick joy, bool isInverted)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        if (joy.Horizontal != 0)
        {
            rb.velocity = new Vector2(speed * joy.Horizontal / Mathf.Abs(joy.Horizontal), rb.velocity.y);
        }else
            rb.velocity = new Vector2(0, rb.velocity.y);
        if (joy.Horizontal < 0)
        {
            if(!isInverted) SC.localScale = new Vector3(-1, 1, 1);
            else SC.localScale = new Vector3(1, 1, 1);
        }
        if (joy.Horizontal > 0)
        {
            if (!isInverted) SC.localScale = new Vector3(1, 1, 1);
            else SC.localScale = new Vector3(-1, 1, 1);
        }
    }
    public static void Jump(GameObject gm, Transform groundCheck, LayerMask lay, float force)
    {
        bool jump;
        float space;
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        #region space int
        #endregion
        if (Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, lay))
        {

            rb.velocity = new Vector2(rb.velocity.x, force);
            jump = true;
            
        }

        if (Input.GetKeyUp(KeyCode.Z) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.3f);
            jump = false;
        }
    }
    public static void Attack(Transform tr, GameObject att, Transform attPos, bool isRanged, Transform SC)
    {
        if (!isRanged)
        {
            GameObject _slash = Instantiate(att, attPos.position, attPos.rotation);
            _slash.transform.localScale = new Vector3(SC.localScale.x / Mathf.Abs(SC.localScale.x) * _slash.transform.localScale.x,
                _slash.transform.localScale.y, 1);
            _slash.transform.parent = tr;
            Destroy(_slash, 0.1f);
        }
        else
        {
            GameObject _bullet = Instantiate(att, attPos.position, attPos.rotation);
            _bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(5 * SC.localScale.x / Mathf.Abs(SC.localScale.y),
                0);
            Destroy(_bullet, 3f);
        }
    }
    [Header("Control")]
    GameObject controled = null;
    bool isControlling = false;
    public float controlDist = 4;
    public float encounterSpeed;
    public static void getControl(Transform tr, float dist, CinemachineVirtualCamera cam)
    {
        Debug.Log("control : void caused!");
        List<unit> enemies = getWeakEnemies(tr, dist);
        if (enemies.Count == 0) return;
        float closestDist = dist;
        unit enemy = null;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (Vector3.Distance(enemies[i].transform.position, tr.position) < closestDist)
            {
                closestDist = Vector3.Distance(enemies[i].transform.position, tr.position);
                enemy = enemies[i];
            }
        }
        if (enemy.GetComponent<unit>().hp <= 0)
            enemy.GetComponent<unit>().hp = 1;
        enemy.isControlled = true;
        cam.Follow = enemy.transform;
        Destroy(tr.gameObject);
        Debug.Log("we took control!");
    }
    //getting all enemies in range
    public static List<unit> getWeakEnemies(Transform tr, float dist)
    {
        List<unit> enemies = new List<unit>();
        unit[] enemiesRaw = FindObjectsByType<unit>(FindObjectsSortMode.None);
        Debug.Log("enemies on Map : " + enemiesRaw.Count());
        foreach (unit enemy in enemiesRaw)
        {
            Transform t = enemy.transform;
            if (enemy.hp <= enemy.controlHp && Vector2.Distance(t.position, tr.position) < dist)
                enemies.Add(enemy);
        }
        if (enemies.Count != 0)
            Debug.Log("enemies found!");
        else
            Debug.Log("enemies not found!");
        return enemies;
    }
    public static void Dash(GameObject gm, float dashSpd)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        Vector2 bakedDashSpd = new Vector2(dashSpd * Input.GetAxis("Horizontal"), dashSpd * Input.GetAxis("Vertical"));
        rb.velocity = bakedDashSpd;

    }
    public static void Dash(GameObject gm, float dashSpd, Joystick joy)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        Vector2 bakedDashSpd = new Vector2(dashSpd * joy.Horizontal, dashSpd * joy.Vertical);
        rb.velocity = bakedDashSpd;
    }
    public static bool DashStop(GameObject gm)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        return false;
    }
    public static void CameraControl(Collider2D collision, CinemachineConfiner2D cam)
    {
        if (collision.gameObject.layer == 3)
        {
            Debug.Log("now starring : " + collision.name);
            cam.m_BoundingShape2D = collision.gameObject.GetComponent<PolygonCollider2D>();
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == 3)
    //    {
    //        Debug.Log("now starring : " + collision.name);
    //        cameraLimit = collision.gameObject.GetComponent<PolygonCollider2D>();
    //    }
    //}
}
