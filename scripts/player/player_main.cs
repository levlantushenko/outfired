using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

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
        //input.normal.Attack.canceled += ctx => attack = 0;

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
    Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask lay;

    public float speed;
    public float acceleration;
    public float finalSpd;

    public Transform sc;
    public float coyotT;
    [Space]
    [Header("----- jump -----")]
    [Space]
    public float force;
    public float g;
    public float stillT;
    public float fallLimit;
    public float speedY;
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
    [Tooltip("all player slashes container" +
        "\n 0 - broadsword" +
        "\n 1 - rapiere" +
        "\n 2 - claws")
    ]
    public GameObject[] slashes;
    [Tooltip("current slash")]
    public GameObject slash;
    public Transform attPos;
    [Space]
    [Header("----- abilities -----")]
    [Space]
    public bool sword;
    public bool recharged;
    public float rechT;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
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
    bool isJumping;
    public int jumpStep = 3;

    void Update()
    {
        Collider2D left = Physics2D.OverlapBox(wallChecks[0].position, wallCheckBox, 0f, lay); ;
        Collider2D right = Physics2D.OverlapBox(wallChecks[1].position, wallCheckBox, 0f, lay); ;
        #region JumpDir capture
            if (!isWallJumping && !isGrounded)
        {
            if (left != null && right == null && !left.gameObject.CompareTag("slash"))
            {
                JumpWallDir = 1;
                if (rb.velocity.y < wallFallSpd && !isDashing && axis.y >= -0.5)
                    rb.velocity = new Vector2(rb.velocity.x, wallFallSpd);

                sc.localScale = new Vector3(1, 1, 1);
            } else if(right != null && left == null && !right.gameObject.CompareTag("slash"))
            {
                JumpWallDir = -1;
                if (rb.velocity.y < wallFallSpd && !isDashing && axis.y >= -0.5)
                    rb.velocity = new Vector2(rb.velocity.x, wallFallSpd);
                sc.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                JumpWallDir = 0;
            }
        }
        #endregion

        #region animations

        if (isDashing) anim.SetTrigger("dash");
        if (left != null || right != null)
        {
            if (!isGrounded) anim.SetTrigger("climb");
        }
        if (axis.x != 0 && isGrounded) anim.SetTrigger("run");
        if (rb.velocity.y > 2 && !isGrounded) anim.SetTrigger("rise");
        if (rb.velocity.y < 2 && !isGrounded) anim.SetTrigger("fall");
        if (isGrounded && axis.x == 0) anim.SetTrigger("idle");
        #endregion

        if(axis.x != 0) sc.localScale = new Vector2(axis.x, 1);
        if (isDashAble)
            trail.colorGradient = trailCols[0];
        else
            trail.colorGradient = trailCols[1];
        if (!isDashing)
        {
            finalSpd = Mathf.MoveTowards(rb.velocity.x, speed * axis.x, acceleration * Time.deltaTime);
            rb.gravityScale = startG;
            rb.velocity = new Vector2(finalSpd, rb.velocity.y);
        }else
            rb.gravityScale = 0;

        if (Physics2D.Raycast(groundCheck.position, Vector2.down, 0.01f, lay) && jumpStep != 0)
        { 
            jumpStep = 0;
            isGrounded = true;
            isDashAble = true;
        }
        #region jump


        #endregion

         if (!isGrounded && !coyotChecked)
        {
            StartCoroutine(CoyotTime());
        }

        
        if (FindAnyObjectByType<CinemachineVirtualCamera>().Follow != transform)
            FindAnyObjectByType<CinemachineVirtualCamera>().Follow = transform;
        if (jump != 0)
        {
           JumpStart();
        }
        if (JumpWallDir != 0) isDashAble = true;

        if (dash != 0 && isDashAble)
        {
            _Control.Dash(gameObject, dashSpd, axis);
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
            _Control.Attack(transform, slash, attPos, false, sc);
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
        if(jumpStep != 2 && jumpStep != 0 && !isGrounded)
            speedY = Mathf.MoveTowards(speedY, fallLimit, g * Time.deltaTime);
        rb.velocity = new Vector2(rb.velocity.x, speedY);
        control = 0;
        dash = 0;
    }
    public void JumpStart()
    {
        if (jump != 0 && isGrounded)
        {
            speedY = force;
            jumpStep = 1;
            isGrounded = false;
        }
        if(jumpStep == 1)
            speedY = Mathf.MoveTowards(speedY, 0, g * Time.deltaTime);

        if (speedY == 0 && jumpStep == 1)
        {
            jumpStep = 2;
            Invoke("jumpStep3", stillT);
        }
    }
    
    void jumpStep3() => jumpStep = 0;
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
        attack = 0;
        yield return new WaitForSeconds(rechT);
        recharged = true;
    }
    public void checkPP()
    {
        int sword = -1;
        if (PlayerPrefs.HasKey("sword"))
            sword = PlayerPrefs.GetInt("sword");
        if(sword != -1)
            slash = slashes[sword];
        Debug.Log(sword);
        rechT = slash.GetComponent<slash>().attSpeed;
    }
    
    public void WallJumpReset() => isWallJumping = false;
    public void GetControl()
    {
        _Control.getControl(transform, dist, conf.GetComponent<CinemachineVirtualCamera>());

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
                    _Control.Jump(gameObject, groundCheck, lay, force / 2);
                    rb.velocity *= new Vector2(1.1f, 1);
                }
                else
                    _Control.Jump(gameObject, groundCheck, lay, force);
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
    public void WallJump() => _Control.WallJump(gameObject, JumpWallDir, wallJumpForce);
    
    void StopDash() { 
        _Control.DashStop(gameObject);
        isDashing=false;
    }
    void DashReset() => isDashAble = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumpStep = 0;
    }
    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _Control.CameraControl(collision, conf);
            PlayerPrefs.SetInt("x", (int)collision.transform.GetChild(0).position.x);
            PlayerPrefs.SetInt("y", (int)collision.transform.GetChild(0).position.y);
        }
        else if(collision.gameObject.tag == "slash" && !collision.gameObject.name.Contains("pl"))
        {
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            hp -= collision.gameObject.GetComponent<slash>().damage;
            if (hp <= 0)
                Death();
            else
            {
                rb.velocity = (collision.transform.position - transform.position).normalized * knockback;
                Time.timeScale = 0;
                yield return new WaitForSecondsRealtime(0.1f);
                Time.timeScale = 1;
                isDashing = true;
                yield return new WaitForSeconds(0.1f);
                isDashing = false;
            }
            yield return new WaitForSeconds(1f);
            collision.gameObject.GetComponent<Collider2D>().enabled = true;

        }
        else if(collision.gameObject.tag == "refill" && !isDashAble)
        {
            isDashAble=true;
            collision.gameObject.SetActive(false);
        }
        yield return null;
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
