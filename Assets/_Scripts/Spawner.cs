//Used on the crypt that will spawn the ghost. Will only work with ghosts since they no clip with structures
//and they are not apart of the Enemy class as ghost does not use anything from Enemy.

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Incribed")]
    public GameObject spawnerPrefab;
    public float spawnDelay; //time before enemy is spawned after hasSpawned has become false;
    public float objectRotation;

    [Header("Dynamic")]
    public GameObject instance = null;

    private Vector3 directionFacing = Vector3.zero;
    private bool hasSpawned = false;
    private float timeToSpawn = 0;

    private AudioHandler aH;

    private void Start()
    {
        aH = FindFirstObjectByType<AudioHandler>();

        Debug.Log("GameObject y rotation = " + transform.rotation.y);

        switch(objectRotation)
        {
            case 0:
                directionFacing = new Vector3(0, 0, 1);
                break;
            case 90:
                directionFacing = new Vector3(1, 0, 0);
                //Debug.Log("Case 90 degrees chosen");
                break;
            case 180:
                directionFacing = new Vector3(0, 0, -1);
                //Debug.Log("Case 180 degrees chosen");
                break;
            case 270:
                directionFacing = new Vector3(-1, 0, 0);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (instance == null)
        {
            hasSpawned = false;

            if (timeToSpawn == 0) timeToSpawn = Time.time + spawnDelay;
        }
        if (hasSpawned || Time.time < timeToSpawn) return;

        instance = Instantiate(spawnerPrefab, transform.position, Quaternion.identity);
        aH.PlayMultiClipSound("GhostSpawn");

        Ghost ghostComp = instance.GetComponent<Ghost>();

        ghostComp.moveDirection = directionFacing;
        ghostComp.SetSpawner(this);

        hasSpawned = true;
        timeToSpawn = 0;
    }
}
