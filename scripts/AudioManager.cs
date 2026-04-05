using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [DoNotSerialize] public AudioSource src;
    private void Awake()
    {
        src = GameObject.Find("Music").GetComponent<AudioSource>();
        Debug.Log(src);
    } 
    public void Play(AudioClip clip)
    {
        src.clip = clip;
        src.Play();
    }
    public void LerpAudio(float fadeSpd, float waitT, AudioClip clip) => StartCoroutine(_LerpAudio(fadeSpd, waitT, clip));

    IEnumerator _LerpAudio(float fadeSpd, float waitT, AudioClip clip)
    {
        while(src.volume != 0 && fadeSpd != 0)
        {
            if (src.volume > fadeSpd * Time.deltaTime)
            {
                src.volume -= fadeSpd * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                src.volume = 0;
            }
        }
        if(fadeSpd != 0)
            yield return new WaitForSeconds(waitT);
        src.clip = clip;
        src.Play();
        while (src.volume != 1 && fadeSpd != 0)
        {
            if (src.volume < 1)
            {
                src.volume += fadeSpd * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                src.volume = 1;
            }
        }
    }

    public void RepeatAudio(float bottomT, float topT) => StartCoroutine(_RepeatAudio(bottomT, topT));
    IEnumerator _RepeatAudio(float bottomT, float topT)
    {
        for(; ; )
        {
            Debug.Log("loop");
            if(src.time >= topT || src.time < bottomT)
                src.time = bottomT;
            yield return new WaitForEndOfFrame();
        }
    }
}
