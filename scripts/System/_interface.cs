using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class _interface : MonoBehaviour
{
    
    public static float time = 0;
    public GameObject player = null;
    public Joystick joy;
    _InputSystem input;
    float pause;
    public GameObject[] panels;
    public Button menu;
    public Button retry;
    private void Awake()
    {
        PlayerPrefs.DeleteKey("died");
        menu.onClick.AddListener(clearSessionSaves);
        retry.onClick.AddListener( () =>
        {
            if (Application.isMobilePlatform)
                panels[0].SetActive(!panels[0].activeInHierarchy);

            panels[1].SetActive(!panels[1].activeInHierarchy);
            Time.timeScale = 1;

            GameObject.Find("player").GetComponent<player_main>().Death();
        });

        if (Application.isMobilePlatform)
            panels[0].SetActive(true);
        else
            panels[0].SetActive(false);

        if (PlayerPrefs.HasKey("timer"))
            tmp.transform.parent.gameObject.SetActive(PlayerPrefs.GetInt("timer") == 0);
    }
    private void Start()
    {
        time = PlayerPrefs.GetInt("time");
    }
    public Text tmp;
    void Update()
    {
        if (InputSystem.devices.Count > 0 && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Application.isMobilePlatform)
                panels[0].SetActive(!panels[0].activeInHierarchy);
            panels[1].SetActive(!panels[1].activeInHierarchy);
            if (!panels[1].activeInHierarchy)
                Time.timeScale = 1;
            else
            {
                Time.timeScale = 0;
                PlayerPrefs.SetInt("timer", tmp.transform.parent.gameObject.activeInHierarchy ? 1 : 0);
            }
        }
            
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
        
        
    }
    void clearSessionSaves()
    {
        PlayerPrefs.DeleteKey("x");
        PlayerPrefs.DeleteKey("died");
        PlayerPrefs.DeleteKey("time");
    }
    
}
