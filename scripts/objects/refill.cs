using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class refill : MonoBehaviour
{
    public float dashes = 1;
    public float freezeT = 0.1f;
    public GameObject refillEff;
    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name != "player" && 
            !collision.gameObject.GetComponent<Dash>().isDashable) yield return null;

        collision.gameObject.GetComponent<Dash>().isDashable = true;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(freezeT);
        Time.timeScale = 1;
        Instantiate(refillEff, transform.position, Quaternion.identity);
        gameObject.SetActive(false);

    }
}
