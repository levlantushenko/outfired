using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_main : MonoBehaviour
{
    [Header("movement")]
    public Transform groundCheck;
    public LayerMask lay;
    public float force;
    public float speed;
    public Transform sc;
    [Header("dash")]
    public float dashSpd;
    public float dashDur;
    public float dashCd;
    bool isDashAble = true;
    bool isDashing;
    public float dist;
    CinemachineConfiner2D conf;
    Joystick joy;
    [Header("death")]
    public GameObject deathEff;
    public float deathT;
    private void Start()
    {
        FindAnyObjectByType<CinemachineVirtualCamera>().LookAt = transform;
        conf = FindAnyObjectByType<CinemachineConfiner2D>();
        joy = FindAnyObjectByType<Joystick>();  
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            Jump();
        if (!isDashing)
        {
            if(!Application.isMobilePlatform)
                Control.Move(gameObject, speed, sc, false);
            else
                Control.Move(gameObject, speed, sc, joy, false);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashAble)
        {
            Control.Dash(gameObject, dashSpd);
            isDashAble = false;
            isDashing = true;
            Invoke("StopDash", dashDur);
            Invoke("DashReset", dashCd);
        }
        if (Input.GetKeyDown(KeyCode.A))
            GetControl();
    }
    public void GetControl() => Control.getControl(transform, dist, conf.GetComponent<CinemachineVirtualCamera>());
    public void Jump() => Control.Jump(gameObject, groundCheck, lay, force);
    public void Dash()
    {
        if (!isDashAble) return;
        Control.Dash(gameObject, dashSpd, joy);
        isDashAble = false;
        isDashing = true;
        Invoke("StopDash", dashDur);
        Invoke("DashReset", dashCd);
    }
    void StopDash() { 
        Control.DashStop(gameObject);
        isDashing=false;
    }
    void DashReset() => isDashAble = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 3)
            Control.CameraControl(collision, conf);
        else if(collision.gameObject.tag == "slash")
        {
            Instantiate(deathEff, transform.position, transform.rotation);
            Invoke("SceneReset", deathT);
            gameObject.SetActive(false);
        }
    }
    void SceneReset() => GameObject.FindGameObjectWithTag("deathScr").GetComponent<Animator>().SetTrigger("die");
}
