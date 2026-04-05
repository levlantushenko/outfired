using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChange : MonoBehaviour
{
    public int scene;
    public void change() => SceneManager.LoadScene(scene);
    
    
}
