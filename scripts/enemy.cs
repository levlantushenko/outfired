using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public float hp;
    public float controlHp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
    float posDifference(float a, float b)
    {
        return (a - b) / Mathf.Abs(a - b);
    }

}
