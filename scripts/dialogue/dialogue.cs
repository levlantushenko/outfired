using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class dialogue : MonoBehaviour
{
    _InputSystem inp;
    public float input;
    
    public TextMeshProUGUI textMesh;
    public Image img;
    GameObject parent;
    
    void Awake()
    {
        inp = new _InputSystem();
        inp.Enable();
        inp.normal.talk.performed += ctx => input = ctx.ReadValue<float>();
        parent = textMesh.transform.parent.parent.gameObject;
        parent.SetActive(false);
    }
    

    public IEnumerator Say(string[] _text, Sprite[] _icons)
    {
        parent.SetActive(true);
        for(int i = 0; i < _text.Length; i++)
        {
            input = 0;
            img.sprite = _icons[i];
            textMesh.text = _text[i];
            //for (int ii = 0; ii < _text[i].Length; ii++)
            //{
            //    textMesh.text += _text[i].ToCharArray().GetValue(ii);
            //    yield return new WaitForSeconds(0.02f);
            //    if (input != 0)
            //        break; continue;
            //}
            while(input == 0)
                yield return new WaitForEndOfFrame();
        }
        parent.SetActive(false);
        gameObject.SetActive(false);
    }
}
