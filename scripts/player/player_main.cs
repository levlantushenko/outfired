 using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class player_main : MonoBehaviour
{
    #region input system init
    _InputSystem input;
    public Vector2 axis;
    public float jump;
    public float dash;
    public float attack;
    public float control;
    private void Awake()
    { 
        input = new _InputSystem();
        input.Enable();
        #region standart input read
        input.normal.Move.performed += ctx => axis = ctx.ReadValue<Vector2>();
        input.normal.Move.canceled += ctx => axis = Vector2.zero;

        input.normal.Jump.performed += ctx => jump = ctx.ReadValue<float>();
        input.normal.Jump.canceled += ctx => jump = 0;

        input.normal.Dash.performed += ctx => dash = ctx.ReadValue<float>();
        input.normal.Dash.canceled += ctx => dash = 0;

        input.normal.Attack.performed += ctx => attack = ctx.ReadValue<float>();
        input.normal.Attack.canceled += ctx => attack = 0;

        input.normal.Control.performed += ctx => control = ctx.ReadValue<float>();
        input.normal.Control.canceled += ctx => control = 0;
        #endregion

    }
    #endregion
    [Header("Sounds")]
    [Space]
    public AudioClip dashSd;
    public AudioClip attackSd;
    AudioSource sound;
    [Space]
    [Header("----- movement -----")]
    [Space]
    public float slowSpeed;
    public float fallLimit;
    Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask lay;
    public float force;
    public float speed;
    public Transform sc;
    public float coyotT;
    [Space]
    [Header("----- wall jump -----")]
    [Space]
    public Vector2 wallJumpForce;
    public float JumpWallDir;
    public Transform[] wallChecks;
    public Vector2 wallCheckBox;
    public float wallFallSpd;
    public bool isWallJumping = false;
    [Space]
    [Header("----- dash -----")]
    [Space]
    public float dashSpd;
    public float dashDur;
    public float dashCd;
    [DoNotSerialize] public bool isDashAble = true;
    [DoNotSerialize] public bool isDashing;
    public float dist;
    public GameObject highSpeedEff;
    CinemachineConfiner2D conf;
    public TrailRenderer trail;
    public Gradient[] trailCols;
    [Space]
    [Header("----- death -----")]
    [Space]
    public float hp;
    public float knockback;
    public GameObject deathEff;
    public float deathT;
    [Space]
    [Header("----- attack -----")]
    [Space]
    public GameObject slash;
    public Transform attPos;
    [Space]
    [Header("----- abilities -----")]
    [Space]
    public bool sword;
    public bool recharged;
    public float rechT;

    private void Start()
    {
        
        sound = GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("x") && PlayerPrefs.HasKey("Dead"))
        {
            PlayerPrefs.DeleteKey("Dead");
            transform.position = new Vector3(PlayerPrefs.GetInt("x"), PlayerPrefs.GetInt("y"));
            PlayableDirector[] directors = FindObjectsOfType<PlayableDirector>();
            foreach(var obj in directors)
                if(obj.playOnAwake)
                    obj.gameObject.SetActive(false);
        }
        FindAnyObjectByType<CinemachineVirtualCamera>().Follow = transform;
        conf = FindAnyObjectByType<CinemachineConfiner2D>();
        rb = GetComponent<Rigidbody2D>();
        
        startG = rb.gravityScale;
        checkPP();
    }
    float startG;
    bool isGrounded;
    void Update()
    {
        if(isDashAble)
            trail.colorGradient = trailCols[0];
        else
            trail.colorGradient = trailCols[1];
        if (!isDashing)
        {
            Control.Move(gameObject, speed, sc, false, axis.x, new Vector2(1, 1));
            rb.gravityScale = startG;
        }else
            rb.gravityScale = 0;

        if (Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, lay))
        {
            isGrounded = true;
            isDashAble = true;
        }
        //if (!isDashing && Mathf.Abs(rb.velocity.x) > Mathf.Abs(speed * 1.5f))
        //    rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, Control.normal(speed), slowSpeed), rb.velocity.y);
        else if (isGrounded && !coyotChecked)
        {
            StartCoroutine(CoyotTime());
        }

        #region JumpDir capture
            if (!isWallJumping && !isGrounded)
        {
            Collider2D left = Physics2D.OverlapBox(wallChecks[0].position, wallCheckBox, 0f, lay);
            Collider2D right = Physics2D.OverlapBox(wallChecks[1].position, wallCheckBox, 0f, lay);
            if (left != null && right == null && !left.gameObject.CompareTag("slash"))
            {
                JumpWallDir = 1;
                if (rb.velocity.y < wallFallSpd && !isDashing && axis.y >= -0.5)
                    rb.velocity = new Vector2(rb.velocity.x, wallFallSpd);
            } else if(right != null && left == null && !right.gameObject.CompareTag("slash"))
            {
                JumpWallDir = -1;
                if (rb.velocity.y < wallFallSpd && !isDashing && axis.y >= -0.5)
                    rb.velocity = new Vector2(rb.velocity.x, wallFallSpd);
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
        if (control != 0)
        {
            InputRead.control = 0;
            GetControl();
        }
        if (sword && attack != 0 && recharged)
        {
            sound.clip = attackSd;
            sound.Play();
            Control.Attack(transform, slash, attPos, false, sc);
            StartCoroutine(SlashRech());
        }
        if (Mathf.Round(axis.y) < 0)
            rb.AddForce(Vector2.down * rb.gravityScale * 2, ForceMode2D.Force);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, fallLimit, Mathf.Infinity));
        float vel = GetComponent<Rigidbody2D>().velocity.magnitude;
        float speedMult = 1;
        if (vel > dashSpd * speedMult || vel < dashSpd * speedMult * -1)
        {
            fast = true;
            if (!speedChecked)
                StartCoroutine(HighSpeed());
        }else
            fast = false;
        // inputReset must be in the end of Update
        control = 0;
        attack = 0;
        dash = 0;
    }
    bool fast;
    bool speedChecked = false;
    bool coyotChecked;
    IEnumerator CoyotTime()
    {
        coyotChecked = true;
        yield return new WaitForSeconds(coyotT);
        isGrounded = false;
        coyotChecked = false;
    }
    IEnumerator HighSpeed()
    {
        speedChecked = true;
        GameObject eff = Instantiate(highSpeedEff, transform.position, transform.rotation);
        Vector2 dir = rb.velocity;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        eff.transform.rotation = Quaternion.Euler(0, 0, z);
        yield return new WaitForSeconds(0.17f);
        if (fast)
            StartCoroutine(HighSpeed());
        else
            speedChecked = false;
    }
    IEnumerator SlashRech()
    {
        recharged = false;
        yield return new WaitForSeconds(rechT);
        recharged = true;
    }
    public void checkPP()
    {
        string abs = "";
        if (PlayerPrefs.HasKey("abilities"))
            abs = PlayerPrefs.GetString("abilities");
        if (abs.Contains("sword"))
            sword = true;
    }
    
    public void WallJumpReset() => isWallJumping = false;
    public void GetControl()
    {
        Control.getControl(transform, dist, conf.GetComponent<CinemachineVirtualCamera>());

    }
    public void Jump()
    {
        if (JumpWallDir == 0) {
            if (isGrounded)
            {
                StopCoroutine(CoyotTime()); 
                isGrounded = false;
                if (isDashing)
                {
                    Control.Jump(gameObject, groundCheck, lay, force / 2);
                    rb.velocity *= new Vector2(1.1f, 1);
                }
                else
                    Control.Jump(gameObject, groundCheck, lay, force);
            }
        
        }
        else
        {
            Debug.Log("Wall jump");
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
        {
            Control.CameraControl(collision, conf);
            PlayerPrefs.SetInt("x", (int)collision.transform.GetChild(0).position.x);
            PlayerPrefs.SetInt("y", (int)collision.transform.GetChild(0).position.y);
        }
        else if(collision.gameObject.tag == "slash")
        {
            hp -= 1;
            if (hp <= 0)
                Death();
            else
            {
                rb.velocity = (collision.transform.position - transform.position).normalized * knockback;
            }
                
        }else if(collision.gameObject.tag == "refill" && !isDashAble)
        {
            isDashAble=true;
            collision.gameObject.SetActive(false);
        }
    }
    void Death()
    {
        PlayerPrefs.SetInt("died", 0);
        Instantiate(deathEff, transform.position, transform.rotation);
        PlayerPrefs.SetInt("time", (int)Time.timeAsDouble);
        PlayerPrefs.SetInt("Dead", 0);
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
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("x");
        PlayerPrefs.DeleteKey("died");
        PlayerPrefs.DeleteKey("time");
    }
}
