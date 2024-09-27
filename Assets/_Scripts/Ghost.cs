//This Enemy does not inherit the Enemy script as that script has nothing that
//this enemy uses. A ghost will just float out in a straight line, slow down, 
//then go back to the crypt.

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Ghost : MonoBehaviour
{
    [Header("Inscribed")]
    public float baseSpeed;
    public float capSpeed;
    public float aValue;
    public float bValue;
    public float cValue;
    public float dValue;

    [Header("Dynamic")]
    public Vector3 moveDirection = Vector3.zero;

    private float timeMoving; //The amount of time the ghost has moved for. Used in move equation to determine velocity
    private Vector3 startPos;

    private GameObject eHObject;
    private AudioHandler aH;
    private Rigidbody rb;
    private Spawner spawner;

    private void Awake()
    {
        eHObject = FindFirstObjectByType<EventHandler>().gameObject;
        aH = FindFirstObjectByType<AudioHandler>();
        rb = GetComponent<Rigidbody>();

        timeMoving = 0;
        startPos = gameObject.transform.position;
    }

    public void SetSpawner(Spawner spawner)
    {
        this.spawner = spawner;
    }
    public void FixedUpdate()
    {
        Vector3 tempVelocity = moveDirection;

        tempVelocity *= MovingEquation() * baseSpeed;

        if (Mathf.Abs(tempVelocity.x) > capSpeed) tempVelocity.x = capSpeed * (tempVelocity.x / Mathf.Abs(tempVelocity.x));
        if (Mathf.Abs(tempVelocity.z) > capSpeed) tempVelocity.z = capSpeed * (tempVelocity.z/Mathf.Abs(tempVelocity.z));

        //if (moveDirection.x < 0 || moveDirection.z < 0) tempVelocity *= -1;

        rb.velocity = tempVelocity;
        //Debug.Log("CurrentVelocity for ghost: " + gameObject.name + " is: x=" + rb.velocity.x + ", y=" + rb.velocity.y + ", z=" + rb.velocity.z);
        timeMoving += Time.deltaTime;

        if (rb.velocity.x > 0) transform.rotation = Quaternion.Euler(0, 90, 0);
        if (rb.velocity.x < 0) transform.rotation = Quaternion.Euler(0, 270, 0);
        if (rb.velocity.y > 0) transform.rotation = Quaternion.Euler(0, 0, 0);
        if (rb.velocity.y < 0) transform.rotation = Quaternion.Euler(0, 180, 0);

        if (timeMoving > 0.5f && WithinRange(transform.position.x, startPos.x, 0.2f) && WithinRange(transform.position.z, startPos.z, 0.2f))
        {
            spawner.instance = null;
            Destroy(gameObject);
        }
    }

    private bool WithinRange(float value, float target, float range)
    {
        if (Mathf.Abs(value - target) <= range) return true;

        return false;
    }
    private float MovingEquation()
    {
        return (aValue * Mathf.Pow(timeMoving, 3)) + (bValue * Mathf.Pow(timeMoving, 2)) + (cValue * timeMoving) + dValue;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player") return;

        Player player = collision.gameObject.GetComponent<Player>();

        player.lives -= 1;
        player.transform.position = player.startingPos;

        aH.Play("CollisionWithGhost");

        if (player.lives == 0)
        {
            PlayerPrefs.SetInt("PlayerScore", 0);
            FindFirstObjectByType<HighScoreMultiLevel>().TryToSetNewHighScore(PlayerPrefs.GetInt("PlayerScore")); //Only doing this so the high score is calculated and shown correctly
            eHObject.GetComponent<SceneManagement>().LoadScene(6);
        }
    }
}
