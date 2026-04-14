using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
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
    public float jump;
    bool coyotChecked;
    float hor = 0;
    void Update()
    {
        if (Gamepad.all.Count > 0)
            hor = Gamepad.current.leftStick.value.x;
        else
            hor = GetAxis(Keyboard.current.rightArrowKey, Keyboard.current.leftArrowKey);
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
    public float GetAxis(KeyControl positive, KeyControl negative)
    {
        if (positive.isPressed)
            return 1;
        else if (negative.isPressed)
            return -1;
        else return 0;
    }
    IEnumerator DashJump()
    {
        StopCoroutine(_Jump());
        rb.gravityScale = g;
        rb.velocity = new Vector2(dash.speed * 1.2f * hor, force / 1.5f);

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
        rb.velocity = new Vector2(rb.velocity.x * 1.01f, force);

        while (rb.velocity.y > 0f)
        {
            yield return new WaitForEndOfFrame();
        }
            

        isGrounded = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        if(stillT != 0)
            yield return new WaitForSeconds(stillT);

        rb.gravityScale = g;
    }
    
    
}
