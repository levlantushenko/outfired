using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class player_main : MonoBehaviour
{
    private void Awake()
    { 
        checkPP();
    }

    [Header("----- death -----")]
    [Space]
    public float hp;
    public float knockback;
    public GameObject deathEff;
    public float deathT;
    [Space]
    [Header("----- attack -----")]
    [Space]
    [Tooltip("all player slashes container" +
        "\n 0 - broadsword" +
        "\n 1 - rapiere" +
        "\n 2 - claws")
    ]
    public GameObject[] slashes;
    
    Animator anim;
    Rigidbody2D rb;
    CinemachineConfiner2D conf;
    float dashSpd;
    public GameObject highSpeedEff;
    public float dist;
    [DoNotSerialize] public Dash _dash;
    

    private void Start()
    {
        _dash = GetComponent<Dash>();
        rb = GetComponent<Rigidbody2D>();
        dashSpd = GetComponent<Dash>().speed;
        anim = GetComponent<Animator>();
        if (PlayerPrefs.HasKey("x") && PlayerPrefs.HasKey("Dead"))
        {
            PlayerPrefs.DeleteKey("Dead");
            transform.position = new Vector3(PlayerPrefs.GetInt("x"), PlayerPrefs.GetInt("y"));
            PlayableDirector[] directors = FindObjectsOfType<PlayableDirector>();
            foreach(var obj in directors)
                if(obj.playOnAwake)
                    obj.gameObject.SetActive(false);
        }
        FindAnyObjectByType<CinemachineVirtualCamera>().Follow = transform;
        conf = FindAnyObjectByType<CinemachineConfiner2D>();

    }

    void Update()
    {
        if (FindAnyObjectByType<CinemachineVirtualCamera>().Follow != transform)
            FindAnyObjectByType<CinemachineVirtualCamera>().Follow = transform;
        
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            GetControl();
        }
        
        float vel = GetComponent<Rigidbody2D>().velocity.magnitude;
        float speedMult = 1;
        if (vel > dashSpd * speedMult || vel < dashSpd * speedMult * -1)
        {
            fast = true;
            if (!speedChecked)
                StartCoroutine(HighSpeed());
        }else
            fast = false;
    }
    
    
    bool fast;
    bool speedChecked = false;
    

    IEnumerator HighSpeed()
    {
        speedChecked = true;
        GameObject eff = Instantiate(highSpeedEff, transform.position, transform.rotation);
        Vector2 dir = rb.velocity;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        eff.transform.rotation = Quaternion.Euler(0, 0, z);
        yield return new WaitForSeconds(0.17f);
        if (fast)
            StartCoroutine(HighSpeed());
        else
            speedChecked = false;
    }
    
    public void checkPP()
    {
        int sword = -1;
        if (PlayerPrefs.HasKey("sword"))
            sword = PlayerPrefs.GetInt("sword");
        if(sword != -1)
            GetComponent<Attack>().attObj = slashes[sword];
    }
    public void GetControl()
    {
        _Control.getControl(transform, dist, conf.GetComponent<CinemachineVirtualCamera>());
    }
    
    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _Control.CameraControl(collision, conf);
            PlayerPrefs.SetInt("x", (int)collision.transform.GetChild(0).position.x);
            PlayerPrefs.SetInt("y", (int)collision.transform.GetChild(0).position.y);
        }
        else if(collision.gameObject.tag == "slash" && !collision.gameObject.name.Contains("pl"))
        {
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            hp -= collision.gameObject.GetComponent<slash>().damage;
            if (hp <= 0)
                Death();
            else
                rb.velocity = (collision.transform.position - transform.position).normalized * knockback;
            yield return new WaitForSeconds(1f);
            collision.gameObject.GetComponent<Collider2D>().enabled = true;

        }
        
        yield return null;
    }
    public void Death()
    {
        PlayerPrefs.SetInt("died", 0);
        Instantiate(deathEff, transform.position, transform.rotation);
        PlayerPrefs.SetInt("time", (int)Time.timeAsDouble);
        PlayerPrefs.SetInt("Dead", 0);
        Invoke("SceneReset", deathT);
        gameObject.SetActive(false);
    }

    void SceneReset() => GameObject.FindGameObjectWithTag("deathScr").GetComponent<Animator>().SetTrigger("die");

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dist);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("x");
        PlayerPrefs.DeleteKey("died");
        PlayerPrefs.DeleteKey("time");
    }
}
