using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class dialogue : MonoBehaviour
{
    _InputSystem inp;
    public float input;
    
    public TextMeshProUGUI textMesh;
    GameObject parent;
    private void Awake()
    {
        inp = new _InputSystem();
        inp.Enable();
        inp.normal.talk.performed += ctx => input = ctx.ReadValue<float>();
    }
    void Start()
    {
        
        parent = textMesh.transform.parent.gameObject;
    }
    private void LateUpdate()
    {
        
    }

    public IEnumerator Say(string[] _text)
    {
        parent.SetActive(true);
        for(int i = 0; i < _text.Length; i++)
        {
            input = 0;
            textMesh.text = _text[i];
            while(input == 0)
                yield return new WaitForEndOfFrame();
        }
        parent.SetActive(false);
    }
}
