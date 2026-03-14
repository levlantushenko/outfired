using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class tp_cannon : MonoBehaviour
{
    #region input system init
    _InputSystem input;
    public Joystick joy;
    public Vector2 axis;
    public float jump;
    public float dash;
    
    #endregion
    public Vector2 force;
    public float dist;
    Transform obj;
    public GameObject eff;
    AudioSource src;
    public float stun = 3;
    private void Start()
    {
        src = GetComponent<AudioSource>();
    }
    bool shooting = false;
    void Update()
    {
        if(obj == null)
            obj = FindAnyObjectByType<player_main>(FindObjectsInactive.Exclude).transform;
        if (obj == null) return;
        if (Vector2.Distance(transform.position, obj.position) < dist)
        {
            if(!shooting && (Keyboard.current.zKey.isPressed || Gamepad.current.buttonWest.isPressed))
            {
                shooting = true;
                StartCoroutine(Shoot());
            }
        }
    }
    IEnumerator Shoot()
    {
        Dash dash = obj.GetComponent<Dash>();
        obj.position = transform.position;
        dash.isDashing = false;
        dash.dashImitate = false;
        yield return new WaitForEndOfFrame();
        obj.position = transform.position;
        obj.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        src.Play();
        dash.isDashing = true;
        dash.dashImitate = false;
        obj.gameObject.SetActive(true);
        obj.GetComponent<Rigidbody2D>().velocity = force;
        dash.isDashable = true;

        float g = obj.GetComponent<Rigidbody2D>().gravityScale;
        obj.GetComponent<Rigidbody2D>().gravityScale = 0;

        Instantiate(eff, transform.position, transform.rotation);
        shooting = false;

        //step 2
        yield return new WaitForSeconds(stun);

        obj.GetComponent<Rigidbody2D>().gravityScale = g;
        dash.isDashing = false;
        yield return new WaitForSeconds(dash.imitateT);
        dash.dashImitate = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dist);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + force * stun);
    }
}
