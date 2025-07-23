using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
/// <summary>
/// User Inputs data base
/// </summary>
public class InputRead : MonoBehaviour
{
    _InputSystem input;
    public static Vector2 axis;
    public static float jump;
    public static float dash;
    public static float attack;
    public static float control;
    private void Awake()
    {
        input = new _InputSystem();
        input.Enable();

        input.normal.Move.performed += ctx => axis = ctx.ReadValue<Vector2>();
        input.normal.Move.canceled += ctx => axis = Vector2.zero;

        input.normal.Jump.performed += ctx => jump = ctx.ReadValue<float>();
        input.normal.Jump.canceled += ctx => jump = 0;

        input.normal.Dash.performed += ctx => dash = ctx.ReadValue<float>();
        input.normal.Dash.canceled += ctx => dash = 0;

        input.normal.Attack.performed += ctx => attack = ctx.ReadValue<float>();
        input.normal.Attack.canceled += ctx => attack = 0;

        input.normal.Control.performed += ctx => control = ctx.ReadValue<float>();
        input.normal.Control.canceled += ctx => control = 0;
    }
   
}
