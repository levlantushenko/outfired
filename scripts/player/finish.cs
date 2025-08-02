using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class achievments : MonoBehaviour
{
    void Start()
    {
        foreach(var ach in _achievments)
            ach.SetActive(false);
        canvas.SetActive(false);
    }
    public GameObject canvas;
    public GameObject[] _achievments;
    public static bool pacifist = true;
    public static bool mercy = false;
    public static bool genocide = true;
    public static bool perfect = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Player") return;
        PlayerPrefs.SetInt("complete", 0);

        barricade br = FindAnyObjectByType<barricade>();
        if (br.isAngry && br.hp > 0)
            mercy = true;
        Unit[] unit = FindObjectsOfType<Unit>();
        for(int i = unit.Length-1; i != 0; i--)
            if (unit[i].hp > 0)
            {
                genocide = false;
                break;
            }
        if(br.hp > 0)
            genocide = false;
        

        if (!PlayerPrefs.HasKey("died"))
            perfect = true;

        canvas.SetActive(true);
        if (mercy)
        {
            _achievments[0].SetActive(true);
            PlayerPrefs.SetInt("mercy", 0);
        }
        if (pacifist)
        {
            _achievments[1].SetActive(true);
            PlayerPrefs.SetInt("pacifist", 0);
        }
        if (genocide)
        {
            _achievments[2].SetActive(true);
            PlayerPrefs.SetInt("genocide", 0);
        }
        if (perfect)
        {
            _achievments[3].SetActive(true);
            PlayerPrefs.SetInt("perfect", 0);
        }
        Time.timeScale = 0;
    }
}
