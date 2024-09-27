using UnityEngine;

public class Player : MonoBehaviour
{
    public enum eMode { idle, move };

    [Header("Inscribed")]
    public float speed = 1;
    public float maxLives = 3;

    [Header("Dynamic")]
    public float facing = 0;
    public eMode mode = eMode.idle;
    public int dirHeld = -1;
    [Range(0f, 3f)]
    public float lives = 3;

    private float audioInterval = .3f;
    private float timeToPlayAudio = 0f;

    private Vector3 posRound = Vector3.zero;

    private Rigidbody rb;
    private Animator anim;
    private GameObject eHObject;
    private EventHandler eH;
    private AudioHandler aH;

    private Vector3[] directions = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1)};

    private KeyCode[] keys = new KeyCode[] {KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.UpArrow,
                                            KeyCode.D, KeyCode.S, KeyCode.A, KeyCode.W};

    public Vector3 startingPos = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        eHObject = GameObject.FindGameObjectWithTag("GameController");
        eH = eHObject.GetComponent<EventHandler>();

        aH = FindObjectOfType<AudioHandler>();

        lives = maxLives;
        startingPos = transform.position;
    }

    private void FixedUpdate()
    {
        posRound = transform.position;
        if (facing == 0 || facing == 2) posRound.z = Mathf.RoundToInt(posRound.z);
        if (facing == 1 || facing == 3) posRound.x = Mathf.RoundToInt(posRound.x);
        transform.position = posRound;

        if (mode == eMode.idle || mode == eMode.move)
        {
            dirHeld = -1;
            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKey(keys[i])) dirHeld = i % 4;
            }

            if (dirHeld == -1)
                mode = eMode.idle;
            else
            {
                facing = dirHeld;
                mode = eMode.move;
            }
        }

        Vector3 vel = Vector3.zero;
        switch (mode)
        {
            case eMode.idle:
                anim.Play("idle");
                timeToPlayAudio = Time.time;
                break;
            case eMode.move:
                vel = directions[dirHeld];
                transform.rotation = Quaternion.Euler(0, facing * 90 + 90, 0); 
                anim.Play("walk");
                anim.speed = .75f;

                if (Time.time >= timeToPlayAudio)
                {
                    aH.PlayMultiClipSound("PlayerWalk");
                    timeToPlayAudio = Time.time + audioInterval;
                }
                break;
        }

        rb.velocity = vel * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Gem")
        {
            eH.gemsCollected++;
            aH.Play("CollectGem");
            Destroy(other.gameObject);
        }

        if (other.tag == "Exit")
        {
            eH.CalculateScore();
        }
    }

}
