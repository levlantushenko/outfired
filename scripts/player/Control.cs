using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Control : MonoBehaviour
{
    /// <summary>
    /// moves player with speed
    /// </summary>
    /// <param name="gm"></param>
    /// which gameobject to move
    /// <param name="speed"></param>
    /// speed of the movement
    /// <param name="SC"></param>
    /// transform that will invert scale
    /// <param name="isInverted"></param>
    /// invert SC on start?
    public static void Move(GameObject gm, float speed, Transform SC, bool isInverted)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        if (Input.GetAxis("Horizontal") != 0)
        {
            if (Mathf.Abs(rb.velocity.x) < speed)
                rb.AddForce(Vector2.right * speed * 2 * Input.GetAxis("Horizontal"), ForceMode2D.Force);
        }
        else
            rb.velocity /= new Vector2(1.2f, 1);
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
    /// <summary>
    /// moves player with speed (mobile ver)
    /// </summary>
    /// <param name="gm"></param>
    /// which gameobject to move
    /// <param name="speed"></param>
    /// speed of the movement
    /// <param name="SC"></param>
    /// transform that will invert scale
    /// <param name="joy"></param>
    /// joystick used for movement
    /// <param name="isInverted"></param>
    /// invert SC on start?
    public static void Move(GameObject gm, float speed, Transform SC, Joystick joy, bool isInverted)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        if (joy.Horizontal != 0)
        {
            if (rb.velocity.x < speed)
                rb.AddForce(Vector2.right * joy.Horizontal * speed * 2 * Input.GetAxis("Horizontal"), ForceMode2D.Force);
        }
        else
            rb.velocity /= new Vector2(1.2f, 1);
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
    /// <summary>
    /// player jump
    /// </summary>
    /// <param name="gm"></param>
    /// player gameobject
    /// <param name="groundCheck"></param>
    /// ground check point
    /// <param name="lay"></param>
    /// layer checked or ignored by groundCheck
    /// <param name="force"></param>
    /// jump force
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
    /// <summary>
    /// player attack method
    /// </summary>
    /// <param name="tr"></param>
    /// player transform
    /// <param name="att"></param>
    /// projectile gameobject
    /// <param name="attPos"></param>
    /// where to span projectile
    /// <param name="isRanged"></param>
    /// is attack ranged?
    /// <param name="SC"></param>
    /// SC of the movement
    public static void Attack(Transform tr, GameObject att, Transform attPos, bool isRanged, Transform SC)
    {
        if (!isRanged)
        {
            GameObject _slash = Instantiate(att, attPos.position, attPos.rotation);
            _slash.transform.localScale = new Vector3(SC.localScale.x / Mathf.Abs(SC.localScale.x) * _slash.transform.localScale.x,
                _slash.transform.localScale.y, 1);
            _slash.transform.parent = tr;
            Destroy(_slash, 0.3f);
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

    /// <summary>
    /// recieve control of the opponent
    /// </summary>
    /// <param name="tr"></param>
    /// player transform
    /// <param name="dist"></param>
    /// finds opponents in distance
    /// <param name="cam"></param>
    /// world cinemachine camera
    public static void getControl(Transform tr, float dist, CinemachineVirtualCamera cam)
    {
        Debug.Log("control : void caused!");
        List<Unit> enemies = getWeakEnemies(tr, dist);
        if (enemies.Count == 0) return;
        float closestDist = dist;
        Unit enemy = null;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (Vector3.Distance(enemies[i].transform.position, tr.position) < closestDist)
            {
                closestDist = Vector3.Distance(enemies[i].transform.position, tr.position);
                enemy = enemies[i];
            }
        }
        enemy.Invoke("BombTime", 0.5f);
        enemy.isControlled = true;
        cam.Follow = enemy.transform;
        Destroy(tr.gameObject);
        Debug.Log("we took control!");
    }
    //getting all enemies in range
    public static List<Unit> getWeakEnemies(Transform tr, float dist)
    {
        List<Unit> enemies = new List<Unit>();
        Unit[] enemiesRaw = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        Debug.Log("enemies on Map : " + enemiesRaw.Count());
        foreach (Unit enemy in enemiesRaw)
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
    /// <summary>
    /// Dashes player in direction
    /// </summary>
    /// <param name="gm"></param>
    /// player gameobject
    /// <param name="dashSpd"></param>
    /// dash speed
    public static void Dash(GameObject gm, float dashSpd)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        Vector2 bakedDashSpd = new Vector2(dashSpd * Input.GetAxis("Horizontal"), dashSpd * Input.GetAxis("Vertical"));
        rb.velocity = bakedDashSpd;

    }
    /// <summary>
    /// Dashes player in direction (mobile)
    /// </summary>
    /// <param name="gm"></param>
    /// player gameobject
    /// <param name="dashSpd"></param>
    /// dash speed
    /// <param name="joy"></param>
    /// joystick used to find dash direction
    public static void Dash(GameObject gm, float dashSpd, Joystick joy)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        Vector2 bakedDashSpd = new Vector2(dashSpd * joy.Horizontal, dashSpd * joy.Vertical);
        rb.velocity = bakedDashSpd;
    }
    public static bool DashStop(GameObject gm)
    {
        Rigidbody2D rb = gm.GetComponent<Rigidbody2D>();
        rb.velocity = rb.velocity / 3;
        return false;
    }
    /// <summary>
    /// camera followment through rooms
    /// </summary>
    /// <param name="collision"></param>
    /// in which polygon is player staying?
    /// <param name="cam"></param>
    /// camera
    public static void CameraControl(Collider2D collision, CinemachineConfiner2D cam)
    {
        if (collision.gameObject.layer == 3)
        {
            Debug.Log("now starring : " + collision.name);
            cam.m_BoundingShape2D = collision.gameObject.GetComponent<PolygonCollider2D>();
        }
    }
    /// <summary>
    /// explodes instillated enemy
    /// </summary>
    /// <param name="origin"></param>
    /// player object
    /// <param name="tr"></param>
    /// enemy transform
    /// <param name="force"></param>
    /// explosion force
    /// <param name="expl"></param>
    /// explosion effect
    public static void Explode(GameObject origin, Transform tr, float force, GameObject expl)
    {
        Vector2 bakedForce = new Vector2(Input.GetAxis("Horizontal") * force, Input.GetAxis("Vertical") * force);
        GameObject obj = Instantiate(origin, tr.position, tr.rotation);
        GameObject _expl = Instantiate(expl, tr.position, tr.rotation);
        Destroy(tr.gameObject);
        obj.GetComponent<Rigidbody2D>().velocity = bakedForce;
    }
}
