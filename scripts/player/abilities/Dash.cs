using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
/// <summary>
/// allows to dash in 8 dircetions
/// </summary>
public class Dash : MonoBehaviour
{
    public float speed;
    public float duration;
    public LayerMask lay;
    public Transform groundCheck;
    public TrailRenderer trail;
    public Color hasDash;
    public Color noDash;
    Rigidbody2D rb;
    Animator anim;
    float g;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        g = rb.gravityScale;
    }

    [DoNotSerialize] public bool isDashing;
    [DoNotSerialize] public bool isDashable = true;
    float ver;
    float hor;
    void Update()
    {
        if (!Application.isMobilePlatform && Gamepad.all.Count == 0)
        {
            hor = GetAxis(Keyboard.current.rightArrowKey, Keyboard.current.leftArrowKey);
            ver = GetAxis(Keyboard.current.upArrowKey, Keyboard.current.downArrowKey);
        }
        else
        {
            hor = Mathf.Abs(Gamepad.current.leftStick.value.x);
            ver = Mathf.Abs(Gamepad.current.leftStick.value.y);
        }
        

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && isDashable)
            StartCoroutine(_Dash());

        if (Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, lay))
        {
            isDashable = true;
        }
        if (isDashable) 
            trail.startColor = hasDash;
        else trail.startColor = noDash;

    }

    IEnumerator _Dash()
    {
        anim.SetBool("dash", true);
        GetComponent<Jump>()?.StopAllCoroutines();
        isDashing = true;
        isDashable = false;
        if (rb.velocity.x < speed)
            rb.velocity = new Vector2(hor * speed, ver * speed);
        else
            rb.velocity = new Vector2(hor * rb.velocity.x * 1.1f, ver * speed);
        rb.gravityScale = 0;

        yield return new WaitForSeconds(duration);

        anim.SetBool("dash", false);
        isDashing = false;
        rb.gravityScale = g;
    }

    public float GetAxis(KeyControl positive, KeyControl negative)
    {
        if (positive.isPressed)
            return 1;
        else if (negative.isPressed)
            return -1;
        else return 0;
    }
    public float normal(float val)
    {
        if (val != 0)
            return val / Math.Abs(val);
        else return 0;
    }
}
