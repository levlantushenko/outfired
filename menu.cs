using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject[] achs;

    private void Start()
    {
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
