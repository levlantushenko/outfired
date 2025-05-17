using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Control : MonoBehaviour
{
    //test void comment
    public static void Move(GameObject gm, Transform groundCheck, LayerMask lay, float force, float speed, Transform SC)
    {
        bool jump;
        float space;
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        #region space int
        if (Input.GetKeyDown(KeyCode.Z))
        {
            space = 1;
        }else
            space = 0;
        #endregion
        if (Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, lay))
        {
            if (space == 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, force);
                jump = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Z) && rb.velocity.y > 0)
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
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SC.localScale = new Vector3(1, 1, 1);
        }
    }
    public static void Attack(Transform tr, GameObject att, Transform attPos, bool isRanged, Transform SC)
    {
        if (!isRanged)
        {
            GameObject _slash = Instantiate(att, attPos.position, attPos.rotation);
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
    public static void getControl(Transform tr, float dist)
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            List<enemy> enemies = Control.getWeakEnemy(tr, dist);
            if (enemies.Count == 0) return;
            float closestDist = dist;
            GameObject enemy = null;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (Vector3.Distance(enemies[i].transform.position, tr.position) < closestDist)
                {
                    closestDist = Vector3.Distance(enemies[i].transform.position, tr.position);
                    enemy = enemies[i].gameObject;
                }
            }
            tr.position = enemy.transform.position;
        }
    }
    //getting all enemies in range
    public static List<enemy> getWeakEnemy(Transform tr, float dist)
    {
        List<enemy> enemies = new List<enemy>();
        enemy[] enemiesRaw = FindObjectsByType<enemy>(FindObjectsSortMode.None);
        foreach (enemy enemy in enemiesRaw)
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
    public static bool DashStop(GameObject gm)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        return false;
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
