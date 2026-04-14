using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class striker: enemy
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
        collException = Instantiate(rocket, shootPos.position, transform.rotation);
        collException.GetComponent<rocket>().exception = gameObject;
        yield return new WaitForSeconds(cooldown);
        if(Physics2D.OverlapBoxAll(fireArea.position, areaScale, 0).Contains(pl.gameObject.GetComponent<Collider2D>()))
            StartCoroutine(EUpdate());
        else isAttacking = false;
    }
    void effDisable() => eff.SetActive(false);

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(fireArea.position, areaScale);
    }
}
