using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_main : MonoBehaviour
{
    [Header("movement")]
    Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask lay;
    public float force;
    public float speed;
    public Transform sc;
    public Vector2 wallJumpForce;
    public float JumpWallDir;
    public Transform[] wallChecks;
    public Vector2 wallCheckBox;
    public float wallFallSpd;
    public bool isWallJumping = false;
    [Header("dash")]
    public float dashSpd;
    public float dashDur;
    public float dashCd;
    bool isDashAble = true;
    [DoNotSerialize] public bool isDashing;
    public float dist;
    CinemachineConfiner2D conf;
    Joystick joy;
    [Header("death")]
    public float hp;
    public float knockback;
    public GameObject deathEff;
    public float deathT;
    
    private void Start()
    {
        
        FindAnyObjectByType<CinemachineVirtualCamera>().Follow = transform;
        conf = FindAnyObjectByType<CinemachineConfiner2D>();
        joy = FindAnyObjectByType<FixedJoystick>();  
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        bool isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, lay);
            #region JumpDir capture
            if (!isWallJumping && !isGrounded)
        {
            if (Physics2D.OverlapBox(wallChecks[0].position, wallCheckBox, 0f, lay) != null)
            {
                Debug.Log("Is Touching " + Physics2D.OverlapBox(wallChecks[0].position, wallCheckBox, 0f, lay).name);
                JumpWallDir = 1;
                if (rb.velocity.y < wallFallSpd)
                    rb.velocity = new Vector2(rb.velocity.x, wallFallSpd);
            } else if(Physics2D.OverlapBox(wallChecks[1].position, wallCheckBox, 0f, lay) != null){
                JumpWallDir = -1;
                if (rb.velocity.y < wallFallSpd)
                    rb.velocity = new Vector2(rb.velocity.x, wallFallSpd);
                Debug.Log("Is Touching " + Physics2D.OverlapBox(wallChecks[1].position, wallCheckBox, 0f, lay).name);
            } 
            else
                JumpWallDir = 0;
        }
        #endregion
        if (FindAnyObjectByType<CinemachineVirtualCamera>().Follow != transform)
            FindAnyObjectByType<CinemachineVirtualCamera>().Follow = transform;
        if (Input.GetKeyDown(KeyCode.Z))
        {
           Jump();
        }
        if (!isWallJumping)
        {
            if (!Application.isMobilePlatform)
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
    public void WallJumpReset() => isWallJumping = false;
    public void GetControl()
    {
        Control.getControl(transform, dist, conf.GetComponent<CinemachineVirtualCamera>());
        Debug.Log("player : void caused!");
    }
    public void Jump()
    {
        if (JumpWallDir == 0)
            Control.Jump(gameObject, groundCheck, lay, force);
        else
        {
            WallJump();
            isWallJumping = true;
            Invoke("WallJumpReset", 0.5f);
        }
    }
    public void WallJump() => Control.WallJump(gameObject, JumpWallDir, wallJumpForce);
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
            hp -= 1;
            if (hp <= 0)
                Death();
            else
            {
                RaycastHit2D hit;
                rb.velocity = (collision.transform.position - transform.position).normalized * knockback;
            }
                
        }
    }
    void Death()
    {
        Instantiate(deathEff, transform.position, transform.rotation);
        Invoke("SceneReset", deathT);
        gameObject.SetActive(false);
    }

    void SceneReset() => GameObject.FindGameObjectWithTag("deathScr").GetComponent<Animator>().SetTrigger("die");
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(wallChecks[0].position, wallCheckBox);
        Gizmos.DrawWireCube(wallChecks[1].position, wallCheckBox);
    }
}
