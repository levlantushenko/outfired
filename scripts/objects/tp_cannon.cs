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
    AudioSource src;
    public float stun = 3;
    private void Start()
    {
        src = GetComponent<AudioSource>();
    }
    bool shooting = false;
    void Update()
    {
        if(obj == null)
            obj = FindAnyObjectByType<player_main>(FindObjectsInactive.Exclude).transform;
        if (obj == null) return;
        if (Vector2.Distance(transform.position, obj.position) < dist)
        {
            if (jump != 0 && !shooting)
            {
                shooting = true;
                StartCoroutine(Shoot());
            }
        }
    }
    IEnumerator Shoot()
    {
        obj.position = transform.position;
        obj.GetComponent<Dash>().isDashing = false;
        yield return new WaitForEndOfFrame();
        obj.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        src.Play();
        obj.GetComponent<Dash>().isDashing = true;
        obj.gameObject.SetActive(true);
        obj.GetComponent<Rigidbody2D>().velocity = force;
        obj.GetComponent<Dash>().isDashable = true;

        float g = obj.GetComponent<Rigidbody2D>().gravityScale;
        obj.GetComponent<Rigidbody2D>().gravityScale = 0;

        Instantiate(eff, transform.position, transform.rotation);
        shooting = false;
        obj.GetComponent<Animator>().SetBool("dash", true);

        //step 2
        yield return new WaitForSeconds(stun);

        obj.GetComponent<Animator>().SetBool("dash", false);
        obj.GetComponent<Rigidbody2D>().gravityScale = g;
        obj.GetComponent<Dash>().isDashing = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dist);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + force * stun);
    }
}
