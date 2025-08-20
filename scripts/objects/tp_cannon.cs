using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tp_cannon : MonoBehaviour
{
    #region input system init
    _InputSystem input;
    public Joystick joy;
    public Vector2 axis;
    public float jump;
    public float dash;
    private void Awake()
    {
        input = new _InputSystem();
        input.Enable();
        input.normal.Move.performed += ctx => axis = ctx.ReadValue<Vector2>();

        input.normal.Jump.performed += ctx => jump = ctx.ReadValue<float>();
        input.normal.Jump.canceled += ctx => jump = 0;

        input.normal.Dash.performed += ctx => dash = ctx.ReadValue<float>();
        input.normal.Dash.canceled += ctx => dash = 0;
    }
    #endregion
    public Vector2 force;
    public float dist;
    Transform obj;
    public GameObject eff;

    void Update()
    {
        if(obj == null)
            obj = FindAnyObjectByType<player_main>().transform;
        if (obj == null) return;
        if (Vector2.Distance(transform.position, obj.position) < dist)
        {
            if (jump != 0)
            {
                Step1();
            }
        }
    }
    void Step1()
    {
        obj.position = transform.position;
        obj.gameObject.SetActive(false);
        Invoke("Step2", 1);
    }
    void Step2()
    {
        obj.gameObject.SetActive(true);
        obj.GetComponent<Rigidbody2D>().velocity = force;
        Instantiate(eff, transform.position, transform.rotation);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dist);
    }
}
