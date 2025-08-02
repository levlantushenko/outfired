using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;
        GetComponent<PlayableDirector>()?.Play();
        
    }
}
