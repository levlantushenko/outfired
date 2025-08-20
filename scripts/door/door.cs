using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    _InputSystem input;
    public float jump;
    private void Awake()
    {
    input = new _InputSystem();
    input.Enable();
        #region standart input read
        input.normal.Jump.performed += ctx => jump = ctx.ReadValue<float>();
        input.normal.Jump.canceled += ctx => jump = 0;
        #endregion
        
    }
    public string scene;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;
        if(jump == 1)
            GetComponent<Animator>().SetTrigger("enter");
    }
}
