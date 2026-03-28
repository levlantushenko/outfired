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
    public GameObject[] selectable;
    public GameObject[] achievments;
    public GameObject[] acts;
    List<GameObject> actChildren;
    Transform actsP;
    public int curAct = 0;
    int totalActs = 1;
    int prevAct = 0;
    public float scrollSpd;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (Application.isMobilePlatform)
            PlayerPrefs.SetInt("mobile", 0);
        else
            PlayerPrefs.SetInt("pc", 0);
        // achievments
        PlayerPrefs.SetInt("complete", 0);
        PlayerPrefs.SetInt("mercy", 0);
        PlayerPrefs.SetInt("pacifist", 0);
        PlayerPrefs.SetInt("genocide", 0);
        PlayerPrefs.SetInt("perfect", 0);
        if (PlayerPrefs.HasKey("complete"))
            achievments[0].SetActive(true);
        else achievments[0].SetActive(false);

        if (PlayerPrefs.HasKey("mercy"))
            achievments[1].SetActive(true);
        else achievments[1].SetActive(false);

        if (PlayerPrefs.HasKey("pacifist"))
            achievments[2].SetActive(true);
        else achievments[2].SetActive(false);

        if (PlayerPrefs.HasKey("genocide"))
            achievments[3].SetActive(true);
        else achievments[3].SetActive(false);

        if (PlayerPrefs.HasKey("perfect"))
            achievments[4].SetActive(true);
        else achievments[4].SetActive(false);
        if (PlayerPrefs.HasKey("speedrun"))
            achievments[5].SetActive(true);
        else achievments[5].SetActive(false);
        if (PlayerPrefs.HasKey("master"))
            achievments[6].SetActive(true);
        else achievments[6].SetActive(false);
        if (PlayerPrefs.HasKey("diver"))
            achievments[7].SetActive(true);
        else achievments[7].SetActive(false);
        // acts
        acts[0].SetActive(true);
        if (PlayerPrefs.HasKey("cave"))
        {
            acts[1].SetActive(true);
            totalActs++;
        }
        if (PlayerPrefs.HasKey("surface"))
        {
            acts[2].SetActive(true);
            totalActs++;
        }
        actsP = acts[0].transform.parent;
        for(int i = 0; i< acts.Length; i++)
        {
            
            if (acts[i].activeSelf)
                totalActs++;
        }
        
    }
    
    public void Play(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
        foreach (var item in achievments)
            item.SetActive(false);
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
}
