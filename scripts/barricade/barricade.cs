using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class barricade : MonoBehaviour
{
    public float hp;
    public float lowHp;
    public float startHp;
    public bool isAngry = false;
    public float speed;
    public Animator anim;
    //phase 1 attacks
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
    [Header("HP display")]
    public float pixPerHp;
    public GameObject hpBar;
    private void Start()
    {
        startHp = hp;
        startScale = transform.localScale;
        pixPerHp = hpBar.transform.localScale.x / hp;
        Invoke("end", 180);
    }

    bool isAttacking = false;
    float posDifference(float a, float b)
    {
        return (a - b) / Mathf.Abs(a - b);
    }
    bool dead = false;
    void Update()
    {
        if (dead) return;
        if (hp <= lowHp) hpBar.GetComponent<Image>().color = Color.yellow;
        if (!isAngry && hp <= startHp / 2)
            Anger();
        hpBar.transform.localScale = new Vector3(hp * pixPerHp, hpBar.transform.localScale.y);
        Transform pl = FindAnyObjectByType<player_main>().transform;
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("laserBeam"))
        {
            transform.localScale = new Vector2(startScale.x * posDifference(transform.position.x, pl.position.x), 1);
            transform.Translate(Vector3.left * transform.localScale.x * speed * Time.deltaTime);
        }
        if (!isAttacking)
        {
            switch (Mathf.Round(Random.Range(0, 2)))
            {
                case 0:
                    StartCoroutine(RocketAttack());
                    break;
                case 1:
                    StartCoroutine(LaserAttack());
                    break;
            }

        }
        if (hp <= 0)
        {
            dead = true;
            anim.SetTrigger("die");
            CancelInvoke("end");
            end();
        }
    }
    void end() => GameObject.Find("roof fall").GetComponent<PlayableDirector>()?.Play();

    public void Anger()
    {
        
        rocketAmount *= 2;
        rocketRech /= 2;
        laserTime = 3;
        rocketSpeed *= 2;
        isAngry = true;
    }
    float rocketTime = 3;
    public float rocketSpeed;
    public IEnumerator RocketAttack()
    {
        isAttacking = true;
        float degrees = startDgr - dgrDelta * rocketAmount / 2;
        for(int i = 0; i <= rocketAmount; i++)
        {
            rocket _rocket = Instantiate(rocket, rocketLauncher.position, Quaternion.Euler(0, 0, degrees)).GetComponent<rocket>();
            _rocket.speed = rocketSpeed;
            degrees += dgrDelta;
            yield return new WaitForSeconds(rocketRech);
        }
        yield return new WaitForSeconds(rocketTime);
        isAttacking =false;
    }
    float laserTime = 5;
    public IEnumerator LaserAttack()
    {
        isAttacking = true;
        anim.SetTrigger("laser");
        yield return new WaitForSeconds(laserTime);
        isAttacking = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "slash" && !collision.name.Contains("rocket"))
        {
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            hp -= 1;
            StartCoroutine(Hit());
            achievments.pacifist = false;
        }
    }

    IEnumerator Hit()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
    }
}
