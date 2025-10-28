using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingDoor : MonoBehaviour
{
    public float doorWaitT;
    public float speed;
    public Transform point;
    public Transform door;
    bool pressed;
    Collider2D doorColl;
    Collider2D pl;
    private void Start()
    {
        doorColl = door.GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.gameObject.CompareTag("Player") || pressed) return;
        pl = collision;
        StartCoroutine(move());
    }
    bool check;
    bool moved;
    private void Update()
    {
        if(!pressed) return;
        if(!moved)
            door.position = Vector2.Lerp(door.position, point.position, speed * Time.deltaTime);
        if(Vector2.Distance(door.position, point.position) <= speed / 16 && !moved)
        {
            moved = true;
            door.position = point.position;
        }
            
        if (check) return;
        if (doorColl.IsTouching(pl))
        {
            Debug.Log("Adding force");
            pl.transform.parent = door;
            Control.WallJumpEvent += AddForce;
        }
        else
        {
            Control.WallJumpEvent -= AddForce;
            pl.transform.parent = null;
        }
    }
    void AddForce()
    {
        Rigidbody2D rb = pl.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.right * Control.normal(rb.velocity.x) * speed * 1.5f);
    }
    IEnumerator move()
    {
        GetComponent<SpriteRenderer>().color = Color.green; 
        yield return new WaitForSeconds(doorWaitT);
        pressed = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(door.position, point.position); 
        Gizmos.DrawWireCube(point.position, door.localScale);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, door.position);
    }
}
