using System;
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
    public float groundDeceleration;
    public float airDeceleration;
    public Transform groundCheck;
    public LayerMask lay;
    float checkLength;
    Rigidbody2D rb;
    public Transform sc;
    bool isDashing = false;
    bool dashAble;
    Dash dash;
    private void Start()
    {
        checkLength = GetComponent<Jump>().checkLength;
        rb = GetComponent<Rigidbody2D>();
        dashAble = GetComponent<Dash>()? true : false;
        dash = GetComponent<Dash>();
    }
    float hor;
    
    private void Update()
    {
        bool isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, lay);

        if (dashAble)
            isDashing = dash.isDashing;
        if (!isDashing) {
            if (!Application.isMobilePlatform && Gamepad.all.Count == 0)
                hor = GetAxis(Keyboard.current.rightArrowKey, Keyboard.current.leftArrowKey);
            else
                hor = Gamepad.current.leftStick.value.x;

            float newXspeed = 0;
            if(rb.velocity.x < max)
                newXspeed = Mathf.MoveTowards(rb.velocity.x, max * hor, acceleration * Time.deltaTime);
            else if(isGrounded)
                newXspeed = Mathf.MoveTowards(rb.velocity.x, max * hor, groundDeceleration * Time.deltaTime);
            else if(!isGrounded)
                newXspeed = Mathf.MoveTowards(rb.velocity.x, max * hor, airDeceleration * Time.deltaTime);

            if (normal(rb.velocity.x) != normal(hor) || isGrounded)
                rb.velocity = new Vector2(newXspeed, rb.velocity.y);

            else if(Mathf.Abs(rb.velocity.x) < max)
                rb.velocity = new Vector2(newXspeed, rb.velocity.y);

            if (hor > 0) 
                sc.localScale = new Vector2(1, 1);
            else if (hor < 0)
                sc.localScale = new Vector2(-1, 1);
            
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
    public float normal(float val)
    {
        if(val!=0)
            return val / Math.Abs(val);
        else return 0;
    }
}
