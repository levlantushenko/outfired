using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class rocket : MonoBehaviour
{
    public float startVel;
    public float speed;
    public float spinTime;
    public float spinSpeed;
    bool isSpinning = true;
    public Vector3 offset;
    public GameObject eff;
    public int[] lay;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(spinTime);
        Transform pl = FindAnyObjectByType<player_main>().transform;
        Debug.Log(pl.name);
        Vector2 dir = pl.position - transform.position;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z);
        //transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z);
        isSpinning = false;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (isSpinning)
            transform.Translate(Vector2.right * startVel * Time.deltaTime);
        else
            transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.layer + " : " + collision.name);
        if(collision.gameObject.layer == 3 || collision.gameObject.name.Contains("striker")) return;
        
        Instantiate(eff, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
