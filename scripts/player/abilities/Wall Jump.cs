using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// allows to jump off the walls
/// </summary>
public class WallJump : MonoBehaviour
{
    public Vector2 scale;
    public Vector2 offset;
    public float muteT;
    public LayerMask lay;
    public float wallFallSpd;
    public Vector2 force;

    Rigidbody2D rb;
    Dash dash;
    float g;
    Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dash = GetComponent<Dash>();
        g = rb.gravityScale;
        anim = GetComponent<Animator>();
    }

    float JumpWallDir;
    void Update()
    {
        Collider2D left = Physics2D.OverlapBox((Vector2)transform.position - offset, scale, 0f, lay);
        Collider2D right = Physics2D.OverlapBox((Vector2)transform.position + offset, scale, 0f, lay);

        if (left != null && right == null && !left.gameObject.CompareTag("slash"))
        {
            JumpWallDir = 1;
            if (!dash.isDashing && !Keyboard.current.downArrowKey.isPressed)
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallFallSpd, Mathf.Infinity));

            transform.localScale = new Vector3(1, 1, 1);
            anim.SetBool("climb", true);
        }
        else if (right != null && left == null && !right.gameObject.CompareTag("slash"))
        {
            JumpWallDir = -1;
            if (!dash.isDashing && !Keyboard.current.downArrowKey.isPressed)
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallFallSpd, Mathf.Infinity));
            transform.localScale = new Vector3(-1, 1, 1);
            anim.SetBool("climb", true);
        }
        else
        {
            JumpWallDir = 0;
            anim.SetBool("climb", false);
        }
        if (Keyboard.current.zKey.wasPressedThisFrame && JumpWallDir != 0)
            StartCoroutine(Jump());

    }
    IEnumerator Jump()
    {
        dash.StopAllCoroutines();
        GetComponent<Jump>()?.StopAllCoroutines();
        rb.gravityScale = g;
        if(rb.velocity.y < dash.speed)
            rb.velocity = new Vector2(force.x * JumpWallDir, force.y);
        else
            rb.velocity = new Vector2(force.x * JumpWallDir, rb.velocity.y + force.y / 2);
        JumpWallDir = 0;
        dash.isDashing = true;
        yield return new WaitForSeconds(muteT);
        dash.isDashing = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position - offset, scale);
        Gizmos.DrawWireCube((Vector2)transform.position + offset, scale);
    }
}
