using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class clock : MonoBehaviour
{
    public float t;
    public Color final;
    public GameObject eff;

    public TextMeshProUGUI localText;
    public Image icon;
    public TextMeshProUGUI text;
    float diff = 0;

    private IEnumerator Start()
    {
        diff = 1 / t;
        icon = GameObject.Find("globalClock").GetComponent<Image>();
        text = icon.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        localText.text = "" + t;
        yield return new WaitForEndOfFrame();
        icon.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(RunClock(collision.gameObject));
        }
    }

    private IEnumerator RunClock(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        localText.gameObject.SetActive(false);
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = final;
        Instantiate(eff, transform.position, transform.rotation);

        float left = t;
        icon.gameObject.SetActive(true);
        icon.fillAmount = 1;
        while (icon.fillAmount > 0)
        {
            icon.fillAmount -= diff * Time.deltaTime;
            left -= Time.deltaTime;
            text.text = "" + Mathf.Ceil(Mathf.Max(left, 0));
            yield return new WaitForEndOfFrame();
        }
        icon.gameObject.SetActive(false);
        icon.fillAmount = 1;

        while(!rb.gameObject.activeSelf)
            yield return new WaitForEndOfFrame();

        rb.position = transform.position;

        Instantiate(eff, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}
