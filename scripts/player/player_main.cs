using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_main : MonoBehaviour
{
    public Transform groundCheck;
    public LayerMask lay;
    public float force;
    public float speed;
    public Transform sc;
    public float dashSpd;
    public float dashDur;
    public float dashCd;
    bool isDashAble = true;
    bool isDashing;
    
    void Update()
    {
        if (!isDashing)
            Control.Move(gameObject, groundCheck, lay, force, speed, sc);
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashAble)
        {
            Control.Dash(gameObject, dashSpd);
            isDashAble = false;
            isDashing = true;
            Invoke("StopDash", dashDur);
            Invoke("DashReset", dashCd);
        }
    }
    void StopDash() => Control.DashStop(gameObject);
    void DashReset(){
        isDashAble = true;
        isDashing = false;
    }
}
