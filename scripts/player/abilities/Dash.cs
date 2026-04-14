
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
    public GameObject eff;
    public float speed;
    public float duration;
    public LayerMask lay;
    public Transform groundCheck;
    public TrailRenderer trail;
    public Color hasDash;
    public Color noDash;
    Rigidbody2D rb;
    AudioSource src;
    public AudioClip clip;
    float g;

    void Start()
    {
        src = GetComponent<AudioSource>();
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
        if (Gamepad.all.Count > 0)
        {
            hor = Mathf.Round(Gamepad.current.leftStick.value.x * 2) / 2;
            ver = Mathf.Round(Gamepad.current.leftStick.value.y * 2) / 2;
        }
        else if (InputSystem.devices.Count > 0)
        {
            hor = GetAxis(Keyboard.current.rightArrowKey, Keyboard.current.leftArrowKey);
            ver = GetAxis(Keyboard.current.upArrowKey, Keyboard.current.downArrowKey);
        }
        if (Application.isMobilePlatform)
        {
            hor = Mathf.Round(Gamepad.current.leftStick.value.x * 2) / 2;
            ver = Mathf.Round(Gamepad.current.leftStick.value.y * 2) / 2;
        }
        if (hor == 0 && ver == 0)
            hor = 1;
        Debug.Log(hor + "|" + ver);
        if(Gamepad.all.Count != 0)
            if (Gamepad.current.rightTrigger.wasPressedThisFrame && isDashable)
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
        if(GetComponent<Jump>() != null)
            GetComponent<Jump>().jump = 0;
        Debug.Log(hor + " _ " + ver);
        GetComponent<Jump>()?.StopAllCoroutines();
        dashImitate = true;
        isDashing = true;
        isDashable = false;
        src.clip = clip;
        src.pitch += UnityEngine.Random.Range(-0.1f, 0.1f);
        src.pitch = Mathf.Clamp(src.pitch, 1.1f, 1.3f);
        src.Play();
        if (Mathf.Abs(rb.velocity.x) <= speed * 0.8f)
            rb.velocity = new Vector2(hor * speed, ver * speed);
        else
        {
            rb.velocity = new Vector2(hor * Mathf.Abs(rb.velocity.x) * 1.2f, ver * speed);
        }
        float z = Mathf.Atan2(ver, hor) * Mathf.Rad2Deg;
        Instantiate(eff, transform.position, Quaternion.Euler(0, 0, z));

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
