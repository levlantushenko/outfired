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
    public GameObject weakEff;
    public GameObject instEff;
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

    public float crystSpotDist;
    public GameObject crystCollect;
    public float crystKnockback;
    Transform crystal;


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
        conf.transform.position = (Vector2)transform.position;
        crystal = GameObject.Find("Crystal").transform;

    }
    Transform curEn = null;
    Transform foundEn = null;
    void Update()
    {
        Transform foundEn = null;
        if(_Control.getWeakEnemy(transform, dist) != null)
            foundEn = _Control.getWeakEnemy(transform, dist).transform;

        if (curEn != null && Vector2.Distance(curEn.position, transform.position) > dist || foundEn != curEn && curEn != null)
        {
            Destroy(curEn.Find(weakEff.name).gameObject);
            curEn = null;
        }
        if(foundEn != null && foundEn != curEn)
        {
            GameObject eff = Instantiate(weakEff, foundEn.position, foundEn.rotation);
            eff.transform.parent = foundEn;
            eff.transform.localScale = Vector2.one;
            eff.name = weakEff.name;
            curEn = foundEn;
        }

        if (Vector2.Distance(transform.position, crystal.position) > crystSpotDist)
            FindAnyObjectByType<CinemachineVirtualCamera>().Follow = transform;
        else
            FindAnyObjectByType<CinemachineVirtualCamera>().Follow = crystal;

        RaycastHit2D obstacle = Physics2D.Raycast(transform.position, rb.velocity.normalized, rb.velocity.magnitude);
        if (Physics2D.RaycastAll(transform.position, rb.velocity.normalized, rb.velocity.magnitude) != null)
        {
            StartCoroutine(HighSpeedReact(obstacle));
        }


        if ((Gamepad.all.Count > 0 && Gamepad.current.buttonNorth.wasPressedThisFrame) ||
            (InputSystem.devices.Count > 0 && Keyboard.current.aKey.wasPressedThisFrame))
            GetControl();

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
    
    IEnumerator HighSpeedReact(RaycastHit2D obst)
    {
        yield return new WaitForEndOfFrame();
        transform.position = obst.point;
    }
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
        _Control.getControl(transform, dist, conf.GetComponent<CinemachineVirtualCamera>(), instEff);
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
        else if(collision.gameObject == crystal.gameObject)
        {
            if (!_dash.dashImitate)
            {
                rb.velocity = crystKnockback * (transform.position - crystal.position);
            }
            else
            {
                crystal.gameObject.SetActive(false);
                Instantiate(crystCollect, crystal.position, Quaternion.Euler(0, 0, 0));
                PlayerPrefs.SetInt(crystal.GetChild(0).name, 0);
            }
        }
        yield return null;
    }
    public void Death()
    {
        hp = 0;
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, crystSpotDist);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("x");
        PlayerPrefs.DeleteKey("died");
        PlayerPrefs.DeleteKey("time");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
