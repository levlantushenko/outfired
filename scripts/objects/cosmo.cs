using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cosmo : MonoBehaviour
{
    public float rech;
    public float aimT;
    public Color aim;
    public Color shoot;
    public LineRenderer line;
    Transform pl;
    IEnumerator Start()
    {

        pl = FindAnyObjectByType<player_main>().transform;
        line.SetPosition(0, Vector3.zero);
        yield return new WaitForSeconds(5);
        StartCoroutine(attack());
    }
    bool follow = false;
    private void Update()
    {
        if (follow)
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, (pl.position - transform.position).normalized, Mathf.Infinity, 0);
            Debug.Log(ray.point);
            line.SetPosition(1, ray.point - (Vector2)transform.position);
        }

    }
    // Update is called once per frame
    IEnumerator attack()
    {
        Debug.Log("catched " + pl.name);
        line.SetColors(aim, aim);
        follow = true;
        yield return new WaitForSeconds(aimT);
        follow = false;
        yield return new WaitForSeconds(1);
        line.SetColors(shoot, shoot);
        RaycastHit2D ray = Physics2D.Raycast(transform.position, line.GetPosition(1) - transform.position, Mathf.Infinity, 0);
        if (ray.collider.CompareTag("Player"))
            ray.collider.GetComponent<player_main>().hp -= 1;
        yield return new WaitForSeconds(0.1f);
        line.SetPosition(1, transform.position);
        yield return new WaitForSeconds(rech);
        StartCoroutine(attack());
    }
}
