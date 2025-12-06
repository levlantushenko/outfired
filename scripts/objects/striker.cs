using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class striker: MonoBehaviour
{
    public GameObject rocket;
    public float dist;
    public GameObject eff;

    public float cooldown;
    public float aimT;

    public Transform fireArea;
    public Vector2 areaScale;
    public ContactFilter2D filter;

    Transform shootPos;
    public float angleLimit;
    bool isAttacking = false;
    Transform pl;

    public float hp;
    public GameObject hitEff;

    void Start()
    {
        shootPos = transform.GetChild(0);
        pl = FindAnyObjectByType<player_main>().transform;
        StartCoroutine(EUpdate());
    }
    void Update()
    {

        if(Physics2D.OverlapBoxAll(fireArea.position, areaScale, 0).Contains(pl.gameObject.GetComponent<Collider2D>()) && !isAttacking)
            StartCoroutine(EUpdate());
    }
    // Update is called once per frame
    
    IEnumerator EUpdate()
    {
        isAttacking = true;
        eff.SetActive(true);
        Invoke("effDisable", 0.5f);
        Instantiate(rocket, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(cooldown);
        if(Physics2D.OverlapBoxAll(fireArea.position, areaScale, 0).Contains(pl.gameObject.GetComponent<Collider2D>()))
            StartCoroutine(EUpdate());
        else isAttacking = false;
    }
    void effDisable() => eff.SetActive(false);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "slash")
        {
            GameObject eff = Instantiate(hitEff, transform.position, transform.rotation);
            float effDir = Control.normal(transform.position.x - collision.transform.position.x);
            eff.transform.localScale = Vector3.one;
            eff.transform.eulerAngles = new Vector3(0, 90 * effDir, 0);
            Destroy(eff, 1f);

            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            hp -= collision.gameObject.GetComponent<slash>().damage;
            achievments.pacifist = false;
        }
        if (collision.gameObject.tag == "explosion")
        {
            hp -= 5;
            PlayerPrefs.SetInt(name + " hp", (int)hp);
            achievments.pacifist = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(fireArea.position, areaScale);
    }
}
