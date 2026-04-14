using System;
using System.Collections;
using System.Collections.Generic;
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
    Transform groundCheck;
    public Transform sc;
    public float coyotT;

    Rigidbody2D rb;
    Dash dash;
    float g;
    float jump = 0;
    void Start()
    {
        groundCheck = GetComponent<Jump>().groundCheck;
        rb = GetComponent<Rigidbody2D>();
        dash = GetComponent<Dash>();
        g = rb.gravityScale;
    }

    float JumpWallDir;
    void Update()
    {
        Collider2D left = Physics2D.OverlapBox((Vector2)transform.position - offset, scale, 0f, lay);
        Collider2D right = Physics2D.OverlapBox((Vector2)transform.position + offset, scale, 0f, lay);

        if (left != null && right == null && !left.gameObject.CompareTag("slash"))
        {
            JumpWallDir = 1;
            if (
            InputSystem.devices.Count > 0 && Keyboard.current.zKey.wasPressedThisFrame && !dash.dashImitate)
                sc.localScale = new Vector3(1, 1, 1);
        }
        else if (right != null && left == null && !right.gameObject.CompareTag("slash"))
        {
            JumpWallDir = -1;
            if (InputSystem.devices.Count > 0 && Keyboard.current.zKey.wasPressedThisFrame && !dash.dashImitate)
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallFallSpd, Mathf.Infinity));
            sc.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            JumpWallDir = 0;
        }
        if (Gamepad.all.Count > 0 && Gamepad.current.buttonWest.wasPressedThisFrame ||
            InputSystem.devices.Count > 0 && Keyboard.current.zKey.wasPressedThisFrame)
        {
            jump = 1;
            Invoke("jumpCancel", coyotT);
        }
            if (!Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, lay) &&
            jump == 1 && JumpWallDir != 0)
                StartCoroutine(Jump());

    }
    void jumpCancel() => jump = 0;
    IEnumerator Jump()
    {
        dash.StopAllCoroutines();
        GetComponent<Jump>()?.StopAllCoroutines();
        rb.gravityScale = g;
        if(!dash.dashImitate)
            rb.velocity = new Vector2(force.x * JumpWallDir, force.y);
        else
            rb.velocity = new Vector2(force.x * JumpWallDir * 3, force.y);
        JumpWallDir = 0;
        dash.isDashing = true;
        yield return new WaitForSeconds(muteT);
        dash.isDashing = false;
        dash.dashImitate = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position - offset, scale);
        Gizmos.DrawWireCube((Vector2)transform.position + offset, scale);
    }
}
