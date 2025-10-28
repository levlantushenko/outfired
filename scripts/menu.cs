using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject[] achs;

    private void Start()
    {
        PlayerPrefs.SetInt("complete", 0);
        PlayerPrefs.SetInt("mercy", 0);
        PlayerPrefs.SetInt("pacifist", 0);
        PlayerPrefs.SetInt("genocide", 0);
        PlayerPrefs.SetInt("perfect", 0);
        if (PlayerPrefs.HasKey("complete"))
            achs[0].SetActive(true);
        else achs[0].SetActive(false);

        if (PlayerPrefs.HasKey("mercy"))
            achs[1].SetActive(true);
        else achs[1].SetActive(false);

        if (PlayerPrefs.HasKey("pacifist"))
            achs[2].SetActive(true);
        else achs[2].SetActive(false);

        if (PlayerPrefs.HasKey("genocide"))
            achs[3].SetActive(true);
        else achs[3].SetActive(false);

        if (PlayerPrefs.HasKey("perfect"))
            achs[4].SetActive(true);
        else achs[4].SetActive(false);
        if (PlayerPrefs.HasKey("speedrun"))
            achs[5].SetActive(true);
        else achs[5].SetActive(false);
        if (PlayerPrefs.HasKey("master"))
            achs[6].SetActive(true);
        else achs[6].SetActive(false);
    }
    public void Play()
    {
        SceneManager.LoadScene("tutorial");
    }
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
        foreach (var item in achs)
            item.SetActive(false);
    }
}
