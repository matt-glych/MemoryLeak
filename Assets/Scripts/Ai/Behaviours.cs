using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Behaviours
{
    public EnemyAi owner;
    public NavMeshAgent nav;

    public float patrolWaitTimer = 0;
    public float waitTimeCopy = 0;

    public Vector3 lastKnownTargetPos;
    public bool anomalySpotted;
    public float spotTargetTimer = 0;
    public float panicDistance = 2;
    public float slowingDistance = 1;


    public Behaviours(EnemyAi owner)
    {
        this.owner = owner;
    }
   
    // change walk animation based on mvoement velocity
    public bool InPosition(Vector3 position, float stoppingDist = 0)
    {
        if (Vector3.Distance(owner.transform.position, position) <= stoppingDist)
        {
            return true;
        }
        return false;
    }

    // return true in target within sight range
    public bool InSightRange(GameObject target)
    {
        float dist = Vector3.Distance(owner.gameObject.transform.position, target.transform.position);
        if (dist <= owner.sightRange)
        {
            //Debug.DrawLine(owner.gameObject.transform.position, target.transform.position, Color.white);
            return true;
        }
        return false;
    }
    // return true in in sight and nothing inbwteen target
    public bool InSight(GameObject target)
    {
        if (InSightRange(target))
        {
            //Debug.DrawLine(owner.gameObject.transform.position, target.transform.position, Color.white);
            RaycastHit hit;
            Vector3 dir = target.transform.position - owner.gameObject.transform.position;
            Vector3 origin = owner.gameObject.transform.position;
            origin.y += 1f;
            if (Physics.Raycast(origin, dir, out hit, owner.sightRange))
            {
                //string hitWho = "hit:" + hit.transform.name;
                if (hit.collider)
                {
                    //Debug.Log("Object spotted: " +hit.collider.gameObject.name);
                    try
                    {
                        if (hit.collider.gameObject.transform.root.gameObject == target)
                        {
                            Debug.DrawLine(origin, hit.point, Color.white);
                            return true;
                        }
                        else
                        {
                            Debug.DrawLine(origin, hit.point, Color.red);
                        }
                    }
                    catch { Debug.Log("Target error.."); }
                }
            }
        }

        return false;
    }
    // true if taqrget in field of view                     
    public bool InFOV(GameObject target)
    {
        float cosAngle = Vector3.Dot((target.transform.position - owner.transform.position).normalized, owner.transform.forward);

        float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;

        return angle < owner.FOVangle;
        /*Vector3 targetDir = target.transform.position - owner.transform.position;
        float angleToPlayer = (Vector3.Angle(targetDir, owner.transform.forward));

        if (angleToPlayer >= -90 && angleToPlayer <= 90) // 180° FOV
        {
            //Debug.Log("Target in peripheral field of view");
            return true;
        }
        return false;*/
    }
    public bool IsAhead(GameObject target)
    {
        Vector3 targetDir = target.transform.position - owner.transform.position;
        float angleToPlayer = (Vector3.Angle(targetDir, owner.transform.forward));

        if (angleToPlayer >= -10 && angleToPlayer <= 10) // 180° FOV
        {
            //Debug.Log("Target in sight field of view");
            return true;
        }
        return false;
    }
    // look at target position
    public void LookAtTarget(Vector3 target)
    {
        Vector3 vector = target;
        vector.y = owner.transform.position.y;

        owner.transform.LookAt(vector);
    }
    public Quaternion LookAtXConstZ(Transform transform, Vector3 target)
    {
        // Fix X pointing at the target, and maintain the current Z direction
        Vector3 newX = target - transform.position;
        Vector3 newZ = transform.forward;

        // Calculate new Y direction
        Vector3 newY = Vector3.Cross(newZ, newX);

        // Let the library method do the heavy lifting
        Quaternion rotation = Quaternion.LookRotation(newZ, newY);

        return rotation;
    }

    // seek a target position
    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity;

        desiredVelocity = targetPos - owner.transform.position;
        desiredVelocity.Normalize();
        desiredVelocity *= owner.maxSpeed;

        return (desiredVelocity - nav.velocity);
    }

    // flee from a position to a 'panic distance'
    public Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desiredVelocity;
        desiredVelocity = owner.transform.position - targetPos;
        //Debug.Log(desiredVelocity.magnitude);
        if (desiredVelocity.magnitude > panicDistance)
        {
            return Vector3.zero;
        }

        desiredVelocity.Normalize();
        desiredVelocity *= owner.maxSpeed;
        return (desiredVelocity - nav.velocity);
    }

    // evade other object
    public Vector3 Evade(GameObject obj)
    {

        float dist = (owner.target.transform.position - owner.transform.position).magnitude;
        float lookAhead = owner.maxSpeed;

        Vector3 targetPos = owner.target.transform.position + (lookAhead * owner.target.GetComponent<CharacterController>().velocity);
        return Flee(targetPos);
    }
    // pursue
    public Vector3 Pursue(GameObject obj)
    {
        Vector3 toTarget = obj.transform.position - owner.transform.position;
        float dist = toTarget.magnitude;
        float time = dist / owner.maxSpeed;

        Vector3 targetPos = owner.target.transform.position + (time * owner.target.GetComponent<CharacterController>().velocity);

        return Seek(targetPos);
    }

    // offset pursuit target
    public Vector3 OffsetPursuitTarget(GameObject obj, Vector3 offset)
    {
        Vector3 target = Vector3.zero;
        target = obj.transform.TransformPoint(offset);

        float dist = (target - owner.transform.position).magnitude;

        float lookAhead = (dist / owner.maxSpeed);

        target = target + (lookAhead * obj.GetComponent<CharacterController>().velocity);

        return Arrive(target);
    }

    // offset pursuit team member
    Vector3 OffsetPursuitTeamMember(GameObject leader, Vector3 offset)
    {
        Vector3 target = Vector3.zero;
        target = leader.transform.TransformPoint(offset);

        float dist = (target - owner.transform.position).magnitude;

        float lookAhead = (dist / owner.maxSpeed);

        target = target + (lookAhead * leader.GetComponent<AgentBehaviours>().nav.velocity);

        return Arrive(target);
    }

    // arrive
    public Vector3 Arrive(Vector3 target)
    {
        Vector3 toTarget = target - owner.transform.position;


        float distance = toTarget.magnitude;
        if (distance <= 0.0f)
        {
            return Vector3.zero;
        }
        const float DecelerationTweaker = 10.3f;
        float ramped = owner.maxSpeed * (distance / (slowingDistance * DecelerationTweaker));

        float clamped = Math.Min(ramped, owner.maxSpeed);
        Vector3 desired = clamped * (toTarget / distance);

        return desired - nav.velocity;
    }

    // patrol between waypoints
    public Vector3 Patrol()
    {
        Vector3 target = Vector3.zero;
        int count = owner.PatrolPoints.Count;
        //Debug.Log("# of PatrolPoints:" + owner.PatrolPoints.Count);

        //if (InPosition(owner.PatrolPoints[current], 1.5f))
        if (InPosition(owner.PatrolPoints[owner.currentPatrolPoint], owner.patrolPointMinDistance))
        {
            //Debug.Log("IN POSITION");
            if (count <= 1)
            {
                return Vector3.zero;
            }
            else
            {
                patrolWaitTimer += Time.deltaTime;
                float seconds = 60 % patrolWaitTimer;
                if (seconds >= owner.patrolWaitTime)
                {
                    if (owner.currentPatrolPoint == count - 1)
                    {
                        owner.currentPatrolPoint = 0;
                    }
                    else
                    {
                        owner.currentPatrolPoint++;
                    }
                    patrolWaitTimer = 0;
                }

                if (waitTimeCopy > 0)
                {
                    owner.patrolWaitTime = waitTimeCopy;
                }
            }
        }
        else
        {
            //Debug.Log("GOING TO PATROL POINT: " + owner.currentPatrolPoint + "::" + owner.PatrolPoints[owner.currentPatrolPoint]);
            //owner.behaviours.LookAtTarget(owner.PatrolPoints[owner.currentPatrolPoint]);
            return Seek(owner.PatrolPoints[owner.currentPatrolPoint]);
        }

        return Vector3.zero;
    }

    public void SpotTargets(GameObject target)
    {
        if (InSightRange(target))
        {

            // detect anomolys in FOV
            if (InSight(target) && InFOV(target) && !owner.targetSpotted)
            {
                if (IsAhead(target) && !owner.targetSpotted)
                {
                    Debug.Log("TARGET SPOTTED AHEAD!");
                    //owner.TargetSpotted(target);
                }
                if (Vector3.Distance(target.transform.position, owner.transform.position) < owner.sightRange / 3)
                {
                    //Debug.Log("TARGET SPOTTED IN FOV!");
                    //owner.TargetSpotted(target);
                }
                //Debug.Log("Anomoly spotted.");
                lastKnownTargetPos = target.transform.position;
                anomalySpotted = true;
            }
        }

        // detect close running sound
        if (Vector3.Distance(target.transform.position, owner.transform.position) < owner.sightRange / 2)
        {
            if (target.GetComponent<CharacterController>().velocity.magnitude > 4f)
            {
                Debug.Log("TARGET HEARD FROM BEHIND!");
                //owner.TargetSpotted(target);
            }
        }

        if (anomalySpotted)
        {
            spotTargetTimer += Time.deltaTime;
            float seconds = 60 % spotTargetTimer;

            if (seconds >= owner.spotTargetDelay)
            {
                if (InFOV(target) && InSight(target))
                {

                    if (!owner.targetSpotted)
                    {
                        Debug.Log("TARGET IN SIGHT!");
                        //owner.TargetSpotted(target);
                    }
                }

                else
                {
                    Debug.Log("TARGET LOST....INVESTIGATING!");
                    owner.PatrolPoints.Insert(owner.currentPatrolPoint, lastKnownTargetPos);
                    //waitTimeCopy = owner.patrolWaitTime;
                    //owner.patrolWaitTime = 0;
                    //Debug.Log("investigate time: " + owner.investigateWaitTime);
                    //Debug.Log("patrol wait time: " + owner.patrolWaitTime);
                    spotTargetTimer = 0;
                    anomalySpotted = false;
                    owner.currentState = State.Patrol;
                }
            }
        }
    }


    // find the 'best' safe space from a list of safe spaces
    Vector3 BestSafeSpace(GameObject from, Transform[] objects)
    {
        // examples of best: closest, second closest, furthest away, one that does not cross player path and/or danger zones, etc.
        Vector3[] safeSpaces = FindSafeSpaces(from, objects);
        int closest = 0;
        int furthest = safeSpaces.Length - 1;
        return safeSpaces[closest];
    }
    // finds safe spaces behind known obsticals, sorted by distance
    Vector3[] FindSafeSpaces(GameObject from, Transform[] obsticals)
    { //finds safe spaces behind obsticals
        Vector3[] positions = new Vector3[obsticals.Length];
        //Debug.Log("Safe spaces: " + positions.Length);
        for (int i = 0; i < obsticals.Length; i++)
        {
            Vector3 player_origin = from.transform.position;
            player_origin.y += 1f;

            Vector3 dest = obsticals[i].position;
            dest.y += 1f;
            Debug.DrawLine(player_origin, dest, Color.yellow);

            Vector3 difference = (obsticals[i].position - from.transform.position);

            Vector3 endpoint = obsticals[i].position + (difference.normalized * (1 + obsticals[i].localScale.x));
            //Debug.DrawLine(player_origin, endpoint, Color.yellow);
            positions[i] = endpoint;
        }
        return SortByDistance(positions);
    }
    // sort a list of Vector3 items by distance
    Vector3[] SortByDistance(Vector3[] items)
    {
        Vector3[] positions_sorted = items.OrderBy(x => Vector3.Distance(owner.transform.position, x)).ToArray();
        return positions_sorted;
    }
}
