using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room : MonoBehaviour
{
    public GameObject _room;

    private void Start()
    {
        _room.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            _room.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "P  layer")
            Invoke("Deactivate", 0.3f);
    }
    void Deactivate() => _room.SetActive(false);
}
