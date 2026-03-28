using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class hp_interface : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI text;
    public float iconFillSpd;
    public Color attackedCol;

    player_main pl;
    float maxHP;
    private void Start()
    {
        pl = GameObject.Find("player").GetComponent<player_main>();
        maxHP = pl.hp;
    }

    private void Update()
    {
        if (pl != null)
        {
            icon.fillAmount = Mathf.MoveTowards(icon.fillAmount, pl.hp / maxHP, iconFillSpd * Time.deltaTime);
            text.text = "" + pl.hp;
            if (pl.hp <= 1)
                text.color = attackedCol;
        }
        else
        {
            pl = GameObject.Find("player").GetComponent<player_main>();
        }
    }
}
