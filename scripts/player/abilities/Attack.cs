using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public GameObject attObj;
    public Transform attPos;
    float rech = 1;

    void Start()
    {
        rech = attObj.GetComponent<slash>().attSpeed;
    }
    bool charged = true;
    float att;
    // Update is called once per frame
    void Update()
    {
        if (Gamepad.all.Count > 0 && Gamepad.current.leftTrigger.wasPressedThisFrame ||
            InputSystem.devices.Count > 0 && Keyboard.current.xKey.wasPressedThisFrame)
            att = 1;
        if(att==1 && charged)
            StartCoroutine(attack());
            
    }
    IEnumerator attack()
    {
        att = 0;
        Transform cur = Instantiate(attObj, attPos).transform;
        cur.position = attPos.position;
        cur.name = $"pl {cur.name}";
        charged = false;
        yield return new WaitForSeconds(rech);
        charged=true;
    }
}
