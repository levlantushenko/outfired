using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barricade : MonoBehaviour
{
    public float hp;
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

    bool isAttacking = false;
    
    void Update()
    {
        if (!isAttacking)
            StartCoroutine(RocketAttack());
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
    }
}
