using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Vampire : Enemy
{
    [Header("Vampire Inscribed")]
    public LayerMask obstructionMask;
    public Vector3 bottomLeftBoundary;
    public Vector3 topRightBoundary;

    private float abilityCooldownTime = 3;
    private float timeToTriggerAbility = 0;

    private int teleportDistanceFromPlayer = 3; //The distance away from the player, the enemy will teleport to.
    private Player player;

    private void Start()
    {
        base.Start();
        player = FindFirstObjectByType<Player>();
    }

    private void FixedUpdate()
    {
        if (mode == eMode.chase)
        {
            timeToTriggerAbility += Time.deltaTime;

            if (timeToTriggerAbility > abilityCooldownTime)
            {
                timeToTriggerAbility = 0;
                TriggerAbility();
            }
        }
        else
        {
            timeToTriggerAbility = 0;
        }

        base.FixedUpdate();
    }

    private void TriggerAbility()
    {
        int distanceLeft = teleportDistanceFromPlayer;
        Vector3 currentDecidedPos = player.gameObject.transform.position;
        currentDecidedPos = RoundTeleportLocation(currentDecidedPos);
        currentDecidedPos.y = -1;
        bool[] canNotTravel = new bool[] { true, true, true, true };
        int iterations = 0;

        while (distanceLeft > 0)
        {
            Vector3 temp = currentDecidedPos;

            float tempFace = PlayerDirectionTraveling();

            //Currently, this for loop does not work great.
            for (int i = 0; i < canNotTravel.Length; i++)
            {
                if (!canNotTravel[i] && tempFace == i)
                {
                    tempFace++;
                    i = 0;
                    if (tempFace == 4) tempFace = 0;
                }
                if (!canNotTravel[0] && !canNotTravel[1] && !canNotTravel[2] && !canNotTravel[3]) break;
            }

            switch (tempFace)
            {
                case 0:
                    temp.x += 1;
                    if (CheckPointWithRaycast(temp) && currentDecidedPos.x + 1 < topRightBoundary.x)
                    {
                        currentDecidedPos.x += 1;
                        distanceLeft--;
                        canNotTravel[0] = true;
                        canNotTravel[1] = true;
                        canNotTravel[2] = false;
                        canNotTravel[3] = true;

                        Debug.Log("CurrentPositionVampAbility: " + currentDecidedPos.x + ", 0, " + currentDecidedPos.z);
                        Debug.Log("CurrentPositionVampAbilityFromTemp: " + temp.x + ", 0, " + temp.z);
                    }
                    else
                    {
                        canNotTravel[0] = false;
                    }
                    break;
                case 1:
                    temp.z -= 1;
                    if (CheckPointWithRaycast(temp) && currentDecidedPos.z + 1 > bottomLeftBoundary.z)
                    {
                        currentDecidedPos.z -= 1;
                        distanceLeft--;
                        canNotTravel[0] = true;
                        canNotTravel[1] = true;
                        canNotTravel[2] = true;
                        canNotTravel[3] = false;

                        Debug.Log("CurrentPositionVampAbility: " + currentDecidedPos.x + ", 0, " + currentDecidedPos.z);
                        Debug.Log("CurrentPositionVampAbilityFromTemp: " + temp.x + ", 0, " + temp.z);
                    }
                    else
                    {
                        canNotTravel[1] = false;
                    }
                    break;
                case 2:
                    temp.x -= 1;
                    if (CheckPointWithRaycast(temp) && currentDecidedPos.x + 1 > bottomLeftBoundary.x)
                    {
                        currentDecidedPos.x -= 1;
                        distanceLeft--;
                        canNotTravel[0] = false;
                        canNotTravel[1] = true;
                        canNotTravel[2] = true;
                        canNotTravel[3] = true;

                        Debug.Log("CurrentPositionVampAbility: " + currentDecidedPos.x + ", 0, " + currentDecidedPos.z);
                        Debug.Log("CurrentPositionVampAbilityFromTemp: " + temp.x + ", 0, " + temp.z);
                    }
                    else
                    {
                        canNotTravel[2] = false;
                    }
                    break;
                case 3:
                    temp.z += 1;
                    if (CheckPointWithRaycast(temp) && currentDecidedPos.z + 1 < topRightBoundary.z)
                    {
                        currentDecidedPos.z += 1;
                        distanceLeft--;
                        canNotTravel[0] = true;
                        canNotTravel[1] = false;
                        canNotTravel[2] = true;
                        canNotTravel[3] = true;

                        Debug.Log("CurrentPositionVampAbility: " + currentDecidedPos.x + ", 0, " + currentDecidedPos.z);
                        Debug.Log("CurrentPositionVampAbilityFromTemp: " + temp.x + ", 0, " + temp.z);
                    }
                    else
                    {
                        canNotTravel[3] = false;
                    }
                    break;
            }

            //cancels the telepot
            if ((!canNotTravel[0] && !canNotTravel[1] && !canNotTravel[2] && !canNotTravel[3]) || 
            (currentDecidedPos.x >= topRightBoundary.x || currentDecidedPos.x <= bottomLeftBoundary.x || currentDecidedPos.z >= topRightBoundary.z || currentDecidedPos.z <= bottomLeftBoundary.z))
            {
                currentDecidedPos = transform.position;
                break;
            }

            iterations++;
            if (iterations >= 10) break;
        }

        currentDecidedPos.y = 0;
        transform.position = currentDecidedPos;
        rb.velocity = Vector3.zero;
        base.facing = base.OppositeFacing((int)PlayerDirectionTraveling());
        transform.rotation = Quaternion.Euler(0, base.facing * 90 + 90, 0);
    }

    //Returns true if the enemy would be able to be at that location
    private bool CheckPointWithRaycast(Vector3 origin)
    {
        origin.y = 0.1f;
        if (!Physics.Raycast(origin, new Vector3(0, 1, 0), 1f, obstructionMask))
        {
            Debug.Log("No Obstruction at point: " + origin.x + ", 0, " + origin.z);
            return true;
        }

        Debug.Log("Obstruction at point: " + origin.x + ", 0, " + origin.z);
        return false;
    }
    //Returns with the facing value for the direction the player is traveling
    private float PlayerDirectionTraveling()
    {
        return player.facing;
    }

    private Vector3 RoundTeleportLocation(Vector3 location)
    {
        float playerFacing = PlayerDirectionTraveling();

        switch (playerFacing)
        {
            case 0:
                return new Vector3(RoundDown(location.x), location.y, location.z);
            case 1:
                return new Vector3(location.x, location.y, RoundUp(location.z));
            case 2:
                return new Vector3(RoundUp(location.x), location.y, location.z);
            case 3:
                return new Vector3(location.x, location.y, RoundDown(location.z));
            default:
                return new Vector3(Convert.ToInt32(location.x), location.y, Convert.ToInt32(location.z));
        }
    }

    private float RoundUp(float amount)
    {
        return Mathf.Ceil(amount);
    }
    private float RoundDown(float amount)
    {
        return Mathf.Floor(amount);
    }
}
