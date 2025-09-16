using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingDoor : MonoBehaviour
{
    public float doorWaitT;
    public float flyT;
    public Transform point;
    public Transform door;
    bool pressed;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.gameObject.CompareTag("Player") || pressed) return;
        StartCoroutine(move());
    }
    private void Update()
    {
        if(!pressed) return;
        door.position = Vector2.Lerp(door.position, point.position, flyT * Time.deltaTime);

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
