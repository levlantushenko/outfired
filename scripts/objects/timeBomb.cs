using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class timeBomb : MonoBehaviour
{
    public float t;
    private void Start()
    {
        Destroy(gameObject, t);
    }
}
