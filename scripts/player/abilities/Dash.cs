
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Localization.SmartFormat.Extensions;
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
    float g;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        g = rb.gravityScale;
    }
    public float imitateT;
    [DoNotSerialize] public bool isDashing;
    [DoNotSerialize] public bool dashImitate;
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
        else if (Gamepad.all.Count != 0 && !Application.isMobilePlatform)
        {
            hor = Mathf.Round(Gamepad.current.leftStick.value.x * 2) / 2;
            ver = Mathf.Round(Gamepad.current.leftStick.value.y * 2) / 2;
        }
        if (Application.isMobilePlatform)
        {
            hor = Mathf.Round(Gamepad.current.leftStick.value.x * 2) / 2;
            ver = Mathf.Round(Gamepad.current.leftStick.value.y * 2) / 2;
        }
        if (hor == 0 && ver == 0)
            hor = 1;
        if(Gamepad.all.Count != 0)
            if(Gamepad.current.rightTrigger.wasPressedThisFrame && isDashable)
                StartCoroutine(_Dash());

        if (InputSystem.devices.Count != 0)
            if (Keyboard.current.leftShiftKey.wasPressedThisFrame && isDashable)
                StartCoroutine(_Dash());

        if (Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, lay) && !dashImitate)
        {
            isDashable = true;
        }
        if (isDashable) 
            trail.startColor = hasDash;
        else trail.startColor = noDash;

    }

    IEnumerator _Dash()
    {
        GetComponent<Jump>()?.StopAllCoroutines();
        dashImitate = true;
        isDashing = true;
        isDashable = false;
        if (Mathf.Abs(rb.velocity.x) <= speed / 1.5f)
            rb.velocity = new Vector2(hor * speed, ver * speed);
        else
        {
            rb.velocity = new Vector2(hor * Mathf.Abs(rb.velocity.x) * 1.2f, ver * speed);
            isDashable = true;
        }
        rb.gravityScale = 0;

        yield return new WaitForSeconds(duration);

        isDashing = false;
        rb.gravityScale = g;
        yield return new WaitForSeconds(imitateT - duration);
        dashImitate = false;
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
            return val / Mathf.Abs(val);
        else return 0;
    }
}
