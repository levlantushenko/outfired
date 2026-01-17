using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// allows to jump and dashJump
/// </summary>
public class Jump : MonoBehaviour
{
    public float force, stillT, maxFallSpd, checkLength;
    public LayerMask lay;
    float g;
    public Transform groundCheck;
    public float dashJumpLength;
    Rigidbody2D rb;
    [Space(5)]
    [Header("--- comfort ---")]
    [Space(5)]
    public float pressSaveT;
    public float coyotT;
    Dash dash;
    Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        g = rb.gravityScale;
        dash = GetComponent<Dash>();
        anim = GetComponent<Animator>();
    }

    bool isGrounded = false;
    bool isJumping;
    float jump;
    bool coyotChecked;
    void Update()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            Debug.Log("jump");
            CancelInvoke("pressSaveCancel");
            jump = 1;
        }
        else if (Keyboard.current.zKey.wasReleasedThisFrame) Invoke("pressSaveCancel", pressSaveT);

        if (Physics2D.Raycast(groundCheck.position, Vector2.down, checkLength, lay))
        {

            isGrounded = true;
            StopCoroutine(_Jump());
            rb.gravityScale = g;
        }
        else if (!coyotChecked)
            Invoke("CoyotCheck", coyotT);

        if(dash != null && dash.isDashing && isGrounded && jump == 1)
        {
            StartCoroutine(DashJump());
        }

        if (isGrounded && jump == 1)
        {
            StartCoroutine(_Jump());
        }
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallSpd, Mathf.Infinity));
        if (!dash.isDashing)
        {
            if(rb.velocity.y > 0.5f)
            {
                anim.SetBool("rise", true);
                anim.SetBool("fall", false);
            }
            else if(rb.velocity.y < -0.5f)
            {
                anim.SetBool("rise", false);
                anim.SetBool("fall", true);
            }
            else
            {
                anim.SetBool("rise", false);
                anim.SetBool("fall", false);
            }
        }
        else
        {
            anim.SetBool("rise", false);
            anim.SetBool("fall", false);
        }
    }
    void pressSaveCancel() => jump = 0;
    void CoyotCheck() => isGrounded = false;
    IEnumerator DashJump()
    {
        StopCoroutine(_Jump());
        rb.gravityScale = g;
        rb.velocity = new Vector2(rb.velocity.x * 1.1f, force / 1.5f);
        dash.isDashable = true;
        dash.StopAllCoroutines();
        isGrounded = false;
        jump = 0;
        yield return new WaitForSeconds(dashJumpLength);
        anim.SetBool("dash", false);
        dash.isDashing = false;
    }
    IEnumerator _Jump()
    {
        jump = 0;
        rb.velocity = new Vector2(rb.velocity.x, force);
        isGrounded = false;
        while(rb.velocity.y >= 0f)
            yield return new WaitForEndOfFrame();
        Debug.Log("step 2");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        yield return new WaitForSeconds(stillT);
        rb.gravityScale = g;
        yield break;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position - new Vector3(0, checkLength, 0));
    }
}
