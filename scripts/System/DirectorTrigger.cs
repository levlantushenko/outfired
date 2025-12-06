using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorTrigger : MonoBehaviour
{
    #region input system init
    _InputSystem input;
    public float jump;

    private void Awake()
    {
        input = new _InputSystem();
        input.Enable();
        input.normal.Jump.performed += ctx => jump = ctx.ReadValue<float>();
        input.normal.Jump.canceled += ctx => jump = 0;

    }
    #endregion
    public bool stopable;
    public bool trigered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;
        GetComponent<PlayableDirector>()?.Play();
        trigered = true;
        
    }
    private void Update()
    {
        if (jump != 0 && stopable && trigered)
        {
            GetComponent<PlayableDirector>()?.Stop();
            GetComponent<Collider2D>().enabled = false;
            gameObject.SetActive(false);
            transform.GetChild(1)?.gameObject.SetActive(false);
        }
    }
}
