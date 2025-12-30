using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
/// <summary>
/// allows to move horizontally
/// </summary>
public class Walk : MonoBehaviour
{
    [Serialize] public float max;
    public float acceleration;
    public Transform groundCheck;
    public LayerMask lay;

    Rigidbody2D rb;

    bool isDashing = false;
    bool dashAble;
    Animator anim;
    Dash dash;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashAble = GetComponent<Dash>()? true : false;
        anim = GetComponent<Animator>();
        dash = GetComponent<Dash>();
    }
    float hor;
    
    private void Update()
    {
        bool isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, lay);

        if (dashAble)
            isDashing = dash.isDashing;
        if (!isDashing) { 
            hor = GetAxis(Keyboard.current.rightArrowKey, Keyboard.current.leftArrowKey);
            rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, max * hor, acceleration * Time.deltaTime), rb.velocity.y);
            if (hor != 0) transform.localScale = new Vector2(hor, 1);
            if(isGrounded)
                anim.SetBool("run", hor != 0);
            else
                anim.SetBool("run", false);
        }
        else
        {
            anim.SetBool("run", false);
        }
    }
    public float GetAxis(KeyControl positive, KeyControl negative)
    {
        if (positive.isPressed)
            return 1;
        else if (negative.isPressed)
            return -1;
        else return 0;
    }
}
