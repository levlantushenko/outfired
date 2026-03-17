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
    BoxCollider2D coll;
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        g = rb.gravityScale;
        dash = GetComponent<Dash>();
    }

    bool isGrounded = false;
    bool isJumping;
    float jump;
    bool coyotChecked;
    void Update()
    {
        if ((Gamepad.all.Count > 0 && Gamepad.current.buttonWest.wasPressedThisFrame) ||
            (InputSystem.devices.Count > 0 && Keyboard.current.zKey.wasPressedThisFrame))
        {
            CancelInvoke("pressSaveCancel");
            Invoke("pressSaveCancel", pressSaveT);
            jump = 1;
        }

        if (Physics2D.Raycast(groundCheck.position, Vector2.down, checkLength, lay))
        {

            isGrounded = true;
            StopCoroutine(_Jump());
            rb.gravityScale = g;
        }
        else if (!coyotChecked)
            Invoke("CoyotCheck", coyotT);

        if(dash != null && dash.dashImitate && isGrounded && jump == 1)
        {
            StartCoroutine(DashJump());
        }

        if (isGrounded && jump == 1)
        {
            StartCoroutine(_Jump());
        }
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallSpd, Mathf.Infinity));
        
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
        dash.isDashing = false;
        dash.dashImitate = false;
    }
    IEnumerator _Jump()
    {
        //Vector2 additive = jumpAffect != null ? jumpAffect.velocity : Vector2.zero;

        rb.velocity = new Vector2(rb.velocity.x * 1.05f, force);

        jumpAffect = null;

        while (rb.velocity.y > 0f)
            yield return new WaitForEndOfFrame();


        rb.gravityScale = 0;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        if(stillT != 0)
            yield return new WaitForSeconds(stillT);

        rb.gravityScale = g;
    }
    Rigidbody2D jumpAffect;
    //private void OnCollisionStay2D(Collision2D collision)
    //{
        
    //    jumpAffect = collision.rigidbody; 
    //    Debug.Log("---" + jumpAffect.name);
    //    return;
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.rigidbody == jumpAffect)
    //    {
    //        jumpAffect = null;
    //    }
    //}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position - new Vector3(0, checkLength, 0));
    }
}
