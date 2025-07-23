using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barricade : MonoBehaviour
{
    public float hp;
    public float speed;
    public Animator anim;
    //phase 1 attacks
    [Header("laser beam")]
    public float laserTime;
    public GameObject laser;
    [Header("rocket attack")]
    public GameObject rocket;
    public Transform rocketLauncher;
    public float rocketAmount;
    public float dgrDelta;
    public float rocketRech;
    public float startDgr;
    //phase 2 attacks
    [Header("head move")]
    public Transform[] points;
    public float headSpeed;
    public float stayTime;
    Vector2 startScale;
    private void Start()
    {
        startScale = transform.localScale;
    }

    bool isAttacking = false;
    float posDifference(float a, float b)
    {
        return (a - b) / Mathf.Abs(a - b);
    }
    void Update()
    {
        Transform pl = FindAnyObjectByType<player_main>().transform;
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("laserBeam"))
        {
            transform.localScale = new Vector2(startScale.x * posDifference(transform.position.x, pl.position.x), 1);
            transform.Translate(Vector3.left * transform.localScale.x * speed * Time.deltaTime);
        }
        if (!isAttacking)
        {
            switch(Mathf.Round(Random.Range(0, 2)))
            {
                case 0:
                    StartCoroutine(RocketAttack());
                    break;
                case 1:
                    StartCoroutine(LaserAttack());
                    break;
            }

        }
    }
    public IEnumerator RocketAttack()
    {
        isAttacking = true;
        float degrees = startDgr - dgrDelta * rocketAmount / 2;
        for(int i = 0; i <= rocketAmount; i++)
        {
            Instantiate(rocket, rocketLauncher.position, Quaternion.Euler(0, 0, degrees));
            degrees += dgrDelta;
            yield return new WaitForSeconds(rocketRech);
        }
        yield return new WaitForSeconds(3);
        isAttacking =false;
    }
    public IEnumerator LaserAttack()
    {
        isAttacking = true;
        anim.SetTrigger("laser");
        yield return new WaitForSeconds(5);
        isAttacking = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
    }
}
