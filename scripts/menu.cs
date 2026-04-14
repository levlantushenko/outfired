using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    #region input system init
    _InputSystem input;
    public Vector2 axis;
    public float submit;
    public float cancel;
    
    private void Awake()
    {
        Time.timeScale = 1.0f;
        input = new _InputSystem();
        input.Enable();

        input.menu.Joystick.performed += ctx => axis = ctx.ReadValue<Vector2>();
        input.menu.Joystick.canceled += ctx => axis = Vector2.zero;

        input.menu.Submit.performed += ctx => submit = ctx.ReadValue<float>();
        input.menu.Submit.canceled += ctx => submit = 0;

        input.menu.Cancel.performed += ctx => cancel = ctx.ReadValue<float>();
        input.menu.Cancel.canceled += ctx => cancel = 0;
    }
    #endregion
    public GameObject[] acts;
    public int curAct = 0;
    int totalActs = 1;
    int prevAct = 0;
    public float scrollSpd;

    Animator anim;

    private IEnumerator Start()
    {
        if (Application.isMobilePlatform)
            PlayerPrefs.SetInt("mobile", 0);
        else
            PlayerPrefs.SetInt("pc", 0);

        yield return new WaitForEndOfFrame();

        anim = GetComponent<Animator>();
        anim.enabled = true;
        anim.Play("start");
    }
    
    public void Play(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    
    public void Enable(GameObject obj)
    {

        for(int i = 0; i < acts.Length; i++)
        {
            if (acts[i] == obj)
                acts[i].transform.GetChild(0).gameObject.SetActive(true);
            else acts[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void PlayAnim() => anim.SetTrigger("play");
    public void MenuAnim() => anim.SetTrigger("menu");
    public void AmmunitionAnim() => anim.SetTrigger("ammunition");
    public void Clear() => PlayerPrefs.DeleteAll();
}
