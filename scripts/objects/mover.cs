using System.Collections;
using System.Collections.Generic;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for(int i = 0; i < transform.childCount; i++)
        {
            if (i != 0)
                Gizmos.DrawLine(transform.parent.GetChild(1).GetChild(i-1).position, transform.parent.GetChild(1).GetChild(i).position);
            else
                Gizmos.DrawLine(transform.position, transform.parent.GetChild(1).GetChild(i).position);
        }
    }
    bool isMoving = false;
    dir fin;
    private IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") yield break;
            dir direction;
        if (inversed)
            direction = inverseDir(GetDirection(collision.gameObject));
        else
            direction = GetDirection(collision.gameObject);
        collision.gameObject.transform.parent = transform;
        if (dashReq)
        {
            if (collision.gameObject.GetComponent<player_main>().isDashing)
            {
                Debug.Log("moving");
                isMoving = true;
                fin = direction;
                arrow.GetComponent<SpriteRenderer>().color = Color.cyan;
                switch (direction)
                {
                    case dir.top:
                        arrow.transform.eulerAngles = new Vector3(0, 0, 90);
                        break;
                    case dir.right:
                        arrow.transform.eulerAngles = new Vector3(0, 0, 0);
                        break;
                    case dir.down:
                        arrow.transform.eulerAngles = new Vector3(0, 0, -90);
                        break;
                    case dir.left:
                        arrow.transform.eulerAngles = new Vector3(0, 0, 180);
                        break;
                }
            }
        }
        else
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.cyan;
            switch (direction)
            {
                case dir.top:
                    arrow.transform.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case dir.right:
                    arrow.transform.eulerAngles = new Vector3(0, 0, 0);
                    break;
                case dir.down:
                    arrow.transform.eulerAngles = new Vector3(0, 0, -90);
                    break;
                case dir.left:
                    arrow.transform.eulerAngles = new Vector3(0, 0, 180);
                    break;
            }
            yield return new WaitForSeconds(0.5f);
            isMoving = true;
            collision.gameObject.transform.parent = transform;
        }
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.gray;
            isMoving = false;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;
        collision.gameObject.transform.parent = null;
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(speed * Control.normal(rb.velocity.x), 0));
    }
    enum dir
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

    
    private void Update()
    {
        if (isMoving)
        {
            bakedSpeed = Mathf.Lerp(bakedSpeed, speed, lerpT);
            switch (fin)
            {
                case dir.top:
                    transform.Translate(Vector2.up * bakedSpeed * Time.deltaTime);
                    break;
                case dir.right:
                    transform.Translate(Vector2.right * bakedSpeed * Time.deltaTime);
                    break;
                case dir.down:
                    transform.Translate(Vector2.down * bakedSpeed * Time.deltaTime);
                    break;
                case dir.left:
                    transform.Translate(Vector2.left * bakedSpeed * Time.deltaTime);
                    break;
            }
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
