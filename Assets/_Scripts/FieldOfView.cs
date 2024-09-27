//By Comp-3 Interactive
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Inscribed")]
    public float radius;
    [Range(0, 360)]
    public float angle;
    public GameObject playerRef;
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [Header("Dynamic")]
    public bool canSeePlayer;
    public bool[] isBlocked;
    public Vector3 playerPos = Vector3.zero;
    public Vector3 lastKnownPos = Vector3.zero;

    private Vector3[] directions = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1) };

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        isBlocked = new bool[4];
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();

        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                RaycastHit hit;

                if (!Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    playerPos = target.transform.position;
                    lastKnownPos = playerPos;
                }
                else
                {
                    canSeePlayer = false;
                    playerPos = lastKnownPos;
                }
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
            playerPos = lastKnownPos;
        }

        for (int i = 0; i < isBlocked.Length; i++)
        {
            if (Physics.Raycast(transform.position, directions[i], 1f, obstructionMask))
                isBlocked[i] = true;
            else
                isBlocked[i] = false;
        }
    }
}
