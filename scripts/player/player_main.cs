using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class player_main : MonoBehaviour
{
    #region input system init
    _InputSystem input;
    public Joystick joy;
    public Vector2 axis;
    public float jump;
    public float dash;
    private void Awake()
    { 
        input = new _InputSystem();
        input.Enable();

        input.normal.Move.performed += ctx => axis = ctx.ReadValue<Vector2>();
        input.normal.Move.canceled += ctx => axis = Vector2.zero;

        input.normal.Jump.performed += ctx => jump = ctx.ReadValue<float>();
        input.normal.Jump.canceled += ctx => jump = 0;

        input.normal.Dash.performed += ctx => dash = ctx.ReadValue<float>();
        input.normal.Dash.canceled += ctx => dash = 0;
    }
    #endregion

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
    [Header("death")]
    public float hp;
    public float knockback;
    public GameObject deathEff;
    public float deathT;


    private void Start()
    {
        joy = FindAnyObjectByType<Joystick>();
        FindAnyObjectByType<CinemachineVirtualCamera>().Follow = transform;
        conf = FindAnyObjectByType<CinemachineConfiner2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        Control.Move(gameObject, speed, sc, false, axis.x);
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
        if (jump != 0)
        {
           Jump();
        }


        if (dash != 0 && isDashAble)
        {
            Control.Dash(gameObject, dashSpd, axis);
            isDashAble = false;
            isDashing = true;
            Invoke("StopDash", dashDur);
            Invoke("DashReset", dashCd);
        }
        if (Input.GetKeyDown(KeyCode.A))
            GetControl();

        // inputReset must be in the end of Update
        
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
            Invoke("WallJumpReset", 0.1f);
        }
    }
    public void WallJump() => Control.WallJump(gameObject, JumpWallDir, wallJumpForce);
    
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
