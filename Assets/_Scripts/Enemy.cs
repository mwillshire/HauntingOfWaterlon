using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum eType { zombie }
    public enum eMode { idle, patrol, chase, search, retreat }

    [Header("Inscribed")]
    public GameObject[] patrolPath;
    public float speed;
    public eType type;

    [Header("Dynamic")]
    public eMode mode = eMode.patrol;
    [SerializeField]
    private int patrolIndex = 0;

    private Vector3[] directions = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1) };

    protected int facing = 3;
    private int indexIncrement = 1;
    private Vector3 posRound = Vector3.zero; //rounded position
    private Vector3 playerPos = Vector3.zero;
    private float posRoomForError = .35f; //When checking to see if enemy is on top of last player pos, it will never be exact (.35)
    private int lastFacing = -1; //The last direction the enemy was facing

    //Used to determine when 1 unit in any direction is traveled.
    private Vector3 ogLocation = Vector3.zero;
    private float targetDistance = 1;
    private bool hasMoved = false;

    private float timeInterval = 1f;
    private float timeToRotate = 0;

    protected Rigidbody rb;
    protected Animator anim;
    protected FieldOfView fOV;
    protected GameObject eHObject;
    protected AudioHandler aH;

    private int startFacing = 0;

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        fOV = GetComponent<FieldOfView>();
        eHObject = GameObject.FindGameObjectWithTag("GameController");
        aH = FindObjectOfType<AudioHandler>();
    }

    protected void FixedUpdate()
    {

        //Keeps the Enemy aligned.
        posRound = transform.position;
        if (facing == 0 || facing == 2) posRound.z = Mathf.RoundToInt(posRound.z);
        if (facing == 1 || facing == 3) posRound.x = Mathf.RoundToInt(posRound.x);
        transform.position = posRound;

        rb.velocity = Vector3.zero;
        switch (mode)
        {
            case eMode.idle:
                anim.Play("idle");
                anim.speed = 1;
                break;
            case eMode.patrol:

                transform.rotation = Quaternion.Euler(0, facing * 90 + 90, 0);

                anim.Play("walk");
                anim.speed = 1;

                if (fOV.canSeePlayer)
                {
                    mode = eMode.chase;
                    aH.Play("DetectedPlayer");
                    speed = 1f;
                }

                break;
            case eMode.chase:
                //set playerPos to player position if still can see player
                if (fOV.canSeePlayer)
                    playerPos = fOV.playerPos;

                MoveOneUnit();

                if (Mathf.Abs(transform.position.x - playerPos.x) <= posRoomForError && Mathf.Abs(transform.position.z - playerPos.z) <= posRoomForError)
                {
                    mode = eMode.search;
                    startFacing = facing;
                    return;
                }

                if (hasMoved == false) //if the enemy is moving in between whole unit coordinates (like 1.3 is x) then wont complete this case.
                {
                    DetermineMoveDirection(playerPos);

                    transform.rotation = Quaternion.Euler(0, facing * 90 + 90, 0);
                }

                anim.Play("walk");
                anim.speed = 1;
                break;
            case eMode.search:

                if(Time.time >= timeToRotate)
                {
                    if (facing != 3) facing++;
                    else facing = 0;

                    transform.rotation = Quaternion.Euler(0, facing * 90 + 90, 0);

                    timeToRotate = Time.time + timeInterval;
                }
                
                if (fOV.canSeePlayer)
                {
                    mode = eMode.chase;
                    return;
                }

                if (facing == startFacing)
                {
                    mode = eMode.retreat;
                    aH.Play("LostPlayer");
                    speed = .75f;
                    return;
                }

                anim.Play("idle");
                anim.speed = 1;

                break;
            case eMode.retreat:

                MoveOneUnit();

                if (fOV.canSeePlayer)
                {
                    mode = eMode.chase;
                    return;
                }

                if (Mathf.Abs(transform.position.x - patrolPath[patrolIndex].transform.position.x) <= posRoomForError && Mathf.Abs(transform.position.z - patrolPath[patrolIndex].transform.position.z) <= posRoomForError)
                {
                    mode = eMode.patrol;
                    facing = patrolPath[patrolIndex].GetComponent<PatrolPoint>().facing;

                    if (patrolIndex == patrolPath.Length - 1)
                    {
                        //indexIncrement *= -1;
                        patrolIndex = -1;
                    }
                    patrolIndex += indexIncrement;
                    return;
                }

                if (hasMoved == false)
                {
                    DetermineMoveDirection(patrolPath[patrolIndex].transform.position);

                    transform.rotation = Quaternion.Euler(0, facing * 90 + 90, 0);
                }

                anim.Play("walk");
                anim.speed = 1;
                break;
        }

        if (mode != eMode.search && mode != eMode.idle)
            rb.velocity = speed * directions[facing];
    }

    protected void DetermineMoveDirection(Vector3 targetPos)
    {
        lastFacing = facing;

        float xDiff = Mathf.Round(transform.position.x - targetPos.x);
        float zDiff = Mathf.Round(transform.position.z - targetPos.z);
        int[] priorityDir = new int[] {-1, -1, -1, -1};
        int tryFacing = -1;

        //Set the opposite direction of travel to least priority
        priorityDir[OppositeFacing(lastFacing)] = 0;

        //Set every other value to medium priority
        for (int i = 0; i < priorityDir.Length; i++)
            if (priorityDir[i] == -1) priorityDir[i] = 1;

        //Determine the directions the enemy wants to go and set to high priority
        if (xDiff < 0)
            priorityDir[0] = 2;
        if (zDiff > 0)
            priorityDir[1] = 2;
        if (xDiff > 0)
            priorityDir[2] = 2;
        if (zDiff < 0)
            priorityDir[3] = 2;

        int highestPriority = -1;
        //Find the most prioritized direction, or the first that comes up.
        for (int i = 0; i < priorityDir.Length; i++)
        {
            if (priorityDir[i] > highestPriority && !fOV.isBlocked[i])
            {
                highestPriority = priorityDir[i];
                tryFacing = i;
            }
        }

        facing = tryFacing;

        //Debug.Log("Moving to position: " + targetPos.x + ", " + targetPos.y + ", " + targetPos.z);
    }

    protected int OppositeFacing(int direction)
    {
        if (direction == 0)
            return 2;
        if (direction == 1)
            return 3;
        if (direction == 2)
            return 0;
        if (direction == 3)
            return 1;

        return -1;
    }

    //With this method, the enemy will only check which direction to go at every whole unit in the direction it is traveling. 
    //Without this, the enemy would get stuck immediately when chasing
    protected void MoveOneUnit()
    {
        if (hasMoved == false) ogLocation = transform.position;

        Vector3 distanceTraveled;
        distanceTraveled.x = Mathf.Abs(ogLocation.x - transform.position.x);
        distanceTraveled.z = Mathf.Abs(ogLocation.z - transform.position.z);

        if ((distanceTraveled.x > targetDistance - posRoomForError && distanceTraveled.x < targetDistance + posRoomForError) || (distanceTraveled.z > targetDistance - posRoomForError && distanceTraveled.z < targetDistance + posRoomForError))
        {
            hasMoved = false;
            return;
        }

        hasMoved = true;
    }

    protected void OnTriggerEnter(Collider other)
    {
        PatrolPoint pp = other.gameObject.GetComponent<PatrolPoint>();

        if (mode != eMode.patrol) return;

        if (other.name == patrolPath[patrolIndex].name)
        {
            if (patrolIndex == patrolPath.Length - 1)
            {
                //indexIncrement *= -1;
                patrolIndex = -1;
            }
            patrolIndex += indexIncrement;
            facing = pp.facing;
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player") return;

        Player player = collision.gameObject.GetComponent<Player>();

        player.lives -= 1;
        player.transform.position = player.startingPos;

        switch (type)
        {
            case eType.zombie:
                aH.Play("CollisionWithZombie");
                break;
        }

        if (player.lives == 0)
        {
            PlayerPrefs.SetInt("PlayerScore", 0);
            FindFirstObjectByType<HighScoreMultiLevel>().TryToSetNewHighScore(PlayerPrefs.GetInt("PlayerScore")); //Only doing this so the high score is calculated and shown correctly
            eHObject.GetComponent<SceneManagement>().LoadScene(6);
        }
    }

}
