using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class mover : MonoBehaviour
{
    [Header("variations")]
    [Space()]
    public bool dashReq;
    public bool inversed;
    public bool lerp;
    [Space()]
    [Header("main params")]
    [Space()]
    float bakedSpeed = 0;
    public float speed;
    public Transform stops;
    public int cur;
    public float lerpT;
    public GameObject arrow;
    public float waitT;
    
    [DoNotSerialize] public bool isMoving = false;
    [DoNotSerialize] public dir fin;
    private IEnumerator OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag != "Player") yield break;
        dir direction;
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        collision.gameObject.transform.parent = transform;
        
        if (inversed)
            direction = inverseDir(GetDirection(collision.gameObject));
        else
            direction = GetDirection(collision.gameObject);
        if (dashReq)
        {
            if (collision.gameObject.GetComponent<Dash>().isDashing)
            {
                isMoving = false;
                collision.gameObject.GetComponent<Dash>().isDashing = false;
                collision.gameObject.GetComponent<Dash>().dashImitate = false;
                collision.gameObject.GetComponent<Dash>().isDashable = true;
                isMoving = false;
                arrow.GetComponent<SpriteRenderer>().color = Color.cyan;
                
                yield return new WaitForSeconds(waitT);
                isMoving = true;
                fin = direction;
            }
        }
        else
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.cyan;
            yield return new WaitForSeconds(waitT);
            isMoving = true; 
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;
        collision.gameObject.transform.parent = transform;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;
        collision.gameObject.transform.parent = null;
    }
    public enum dir
    {
        none,
        top,
        down, left, right
    }
    dir GetDirection(GameObject coll)
    {
        Vector2 pos = (coll.transform.position - transform.position).normalized;
        pos.x = Mathf.Round(pos.x); 
        pos.y = Mathf.Round(pos.y);

        if (pos.x > 0)
            return dir.right;
        else if (pos.x < 0)
            return dir.left;
        else if(pos.y > 0)
            return dir.top;
        else
            return dir.down;
    }

    bool touched = false;
    private void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (!isMoving) return;
        bakedSpeed = speed;
        switch (fin)
        {
            case dir.top:
                transform.Translate(Vector2.up * bakedSpeed * Time.deltaTime);
                arrow.transform.rotation = Quaternion.Euler(0, 0, 90 - transform.rotation.z);
                break;
            case dir.right:
                transform.Translate(Vector2.right * bakedSpeed * Time.deltaTime);
                arrow.transform.rotation = Quaternion.Euler(0, 0, 0 - transform.rotation.z);
                break;
            case dir.down:
                transform.Translate(Vector2.down * bakedSpeed * Time.deltaTime);
                arrow.transform.rotation = Quaternion.Euler(0, 0, -90 - transform.rotation.z);
                break;
            case dir.left:
                transform.Translate(Vector2.left * bakedSpeed * Time.deltaTime);
                arrow.transform.rotation = Quaternion.Euler(0, 0, 180 - transform.rotation.z);
                break;
        }
        
    }
    dir inverseDir(dir _dir)
    {
        switch (_dir)
        {
            case dir.left:
                return dir.right;
            case dir.right:
                return dir.left;
            case dir.top:
                return dir.down;
            default:
                return dir.top;
        }
    }
    
}
