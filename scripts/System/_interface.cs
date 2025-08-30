using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class _interface : MonoBehaviour
{
    
    public static float time = 0;
    public GameObject player = null;
    public Joystick joy;
    _InputSystem input;
    float pause;
    public GameObject[] panels;
    private void Awake()
    {
        if (!Application.isMobilePlatform)
            panels[0].SetActive(false);
        if (PlayerPrefs.HasKey("timer"))
            tmp.transform.parent.gameObject.SetActive(PlayerPrefs.GetInt("timer") == 0);
        input = new _InputSystem();
        input.Enable();
        input.normal.pause.performed += ctx => pause = ctx.ReadValue<float>();
        input.normal.pause.canceled += ctx => pause = 0;
    }
    private void Start()
    {
        time = PlayerPrefs.GetInt("time");
    }
    public Text tmp;
    void Update()
    {
        time += Time.deltaTime;
        #region timer
        if (((int)(time / 60)) < 1)
            tmp.text = "00";
        else if (((int)(time / 60)) < 10)
            tmp.text = "0" + ((int)(time / 60));
        else
            tmp.text = "" + ((int)(time/60));
        if ((int)time % 60 < 1)
            tmp.text += ".00";
        else if ((int)time % 60 < 10)
            tmp.text += ".0" + (int)time % 60;
        else
            tmp.text += "." + (int)time % 60;
        if (Mathf.Round((time - (int)time) * 100) < 1)
            tmp.text += ".00";
        else if (Mathf.Round((time - (int)time) * 100) < 10)
            tmp.text += ".0" + Mathf.Round((time - (int)time) * 100);
        else if (Mathf.Round((time - (int)time) * 100) < 100)
            tmp.text += "." + Mathf.Round((time - (int)time) * 100);
        else
            tmp.text += "." + Mathf.Round((time - (int)time) * 10);
        #endregion
        if (pause == 1)
        {
            if(Application.isMobilePlatform)
                panels[0].SetActive(!panels[0].activeInHierarchy);
            panels[1].SetActive(!panels[1].activeInHierarchy);
            if(!panels[1].activeInHierarchy)
                Time.timeScale = 1;
            else
            {
                Time.timeScale = 0;
                PlayerPrefs.SetInt("timer", tmp.transform.parent.gameObject.activeInHierarchy ? 1 : 0);
            }
        }
        pause = 0;
    }


    //void Update()
    //{
    //    if (player == null)
    //    {
    //        Debug.LogWarning("player is missing!");
    //        if(FindAnyObjectByType<player_main>(FindObjectsInactive.Include) != null)
    //            player = FindAnyObjectByType<player_main>(FindObjectsInactive.Include).gameObject;
    //        else
    //        {
    //            Debug.Log("Searching for unit");
    //            Unit[] list = FindObjectsByType<Unit>(FindObjectsSortMode.None);
    //            for(int i = 0; i < list.Length; i++)
    //            {
    //                player = list[i].gameObject;
    //                if (!player.GetComponent<Unit>().isControlled)
    //                    player = null;
    //                if (player != null)
    //                {
    //                    Debug.Log("player found! type : unit");
    //                    break;  
    //                }

    //            }
    //        }
    //    }
    //    if(player.TryGetComponent<Unit>(out Unit pl))
    //    {
    //        pl.Animate(joy, pl.GetComponent<Animator>());
    //    }
    //}
    //public void Jump(){
    //    if (player.TryGetComponent<player_main>(out player_main pl))
    //        pl.Jump();
    //    else
    //        player.GetComponent<Unit>().Jump();
    //}
    //public void Dash()
    //{
    //    if (player.TryGetComponent<player_main>(out player_main pl))
    //        pl.Dash();
    //    else
    //        player.GetComponent<Unit>().Dash();
    //}
    //public void GetControl()
    //{
    //    Debug.Log("void started");
    //    if (player.TryGetComponent<player_main>(out player_main pl))
    //        pl.GetControl();
    //    else
    //        player.GetComponent<Unit>().Explode();
    //}
    //public void Attack()
    //{
    //    if (!player.TryGetComponent<player_main>(out player_main pl))
    //        player.GetComponent<Unit>().Attack();
    //}
    //public static bool isInteracting = false;
    //public void Interact()
    //{
    //    isInteracting = true;
    //    Invoke("StopInteracting", 0.1f);
    //}
    //void StopInteracting() => isInteracting = false;
}
