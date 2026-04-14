using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class achievments : MonoBehaviour
{
    public GameObject canvas;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Player") return;
        canvas.SetActive(true);
        Time.timeScale = 0;
        
        
    }
}
