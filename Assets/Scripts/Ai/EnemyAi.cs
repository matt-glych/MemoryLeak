using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State { Idle, Patrol, Investigate, Chase, Attack, Escort }
public class EnemyAi : MonoBehaviour
{
    // components
    private NavMeshAgent nav;
    public Behaviours behaviours;
    // state variables
    public State currentState;
    // target variables
    public Transform target;
    public bool targetSpotted;
    public float spotTargetDelay;

    public bool alive;

    // speed vaiables
    public float maxSpeed;
    public float walkSpeed;
    public float runSpeed;

    public float FOVangle = 45;
    public float sightRange = 4;

    // patrol variables
    public List<Transform> PatrolPointObjects;
    public List<Vector3> PatrolPoints;
    public int currentPatrolPoint = 0;
    public float investigateWaitTime = 0;
    public float patrolWaitTime;
    private float patrolWaitTimer;
    public float patrolPointMinDistance;
    public float waitTimeCopy;

    // behaviour weights
    public float PursueWeight = 0.0f;
    public float PatrolWeight = 0.0f;

    public bool enablemovement;
    public bool enablePhysics;

    public float chaseTime;
    private float chaseTimer;

    public bool canBouncePlayer;

    // Start is called before the first frame update
    void Start()
    {
       // set components
        nav = GetComponent<NavMeshAgent>();
        behaviours = new Behaviours(this);
        //nav.updateRotation = false;
       
        //get patrol points, using initial position as the first point
        PatrolPoints.Insert(0, gameObject.transform.position);
        foreach (Transform t in PatrolPointObjects)
        {
            PatrolPoints.Add(t.transform.position);
        }

        // set initial state
        currentState = State.Patrol;
        alive = true;
        canBouncePlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateBehaviour();
    }

    void CalculateBehaviour()
    {
        // passive behaviours
        

        // enemy states
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Patrol:
                SpotTarget(target.gameObject);
                Patrol();
                break;

            case State.Investigate:
                SpotTarget(target.gameObject);
                Investigate();
                break;

            case State.Chase:
                Chase();
                break;
            case State.Escort:
                Escort();
                break;
        }
    }

 
    // Idle; state behaviour
    public void Idle()
    {

    }

    // Patrol; state behaviour
    public void Patrol()
    {
        Vector3 waypoint = PatrolPoints[currentPatrolPoint];
        nav.SetDestination(waypoint);

        if (behaviours.InPosition(waypoint, patrolPointMinDistance))
        {
            patrolWaitTimer += Time.deltaTime;
            float seconds = 60 % patrolWaitTimer;
            if (seconds >= patrolWaitTime)
            {
                if (currentPatrolPoint == PatrolPoints.Count - 1)
                {
                    PatrolPoints.Reverse();
                    currentPatrolPoint = 0;
                }
                else
                {
                    currentPatrolPoint++;
                }
                patrolWaitTimer = 0;
            }
        }
    }

    // Investigate; state behaviour
    public void Investigate()
    {
        currentState = State.Chase;
    }

    // Chase; state behaviour
    public void Chase()
    {
        // chase target
        Vector3 targetPoint = target.transform.position;
        nav.SetDestination(targetPoint);

        // resume patrol if lose target
        if (!behaviours.InSight(target.gameObject) && !behaviours.InFOV(target.gameObject))
        {
            chaseTimer += Time.deltaTime;
            if ((chaseTimer % 60) > chaseTime)
            {
                //add current position to patrol points
                PatrolPoints.Add(transform.position);
                chaseTimer = 0;
                currentState = State.Patrol;
            }
        }
    }

    // Escort; state behaviour
    public void Escort()
    {
        // escort target
        Vector3 targetPoint = GameController.gameController.levelController.ExitPoints[0].transform.position;
        nav.SetDestination(targetPoint);

        GameObject.Find("Player").GetComponent<PlayerController>().OnEscort(this.transform);


        if(behaviours.InPosition(targetPoint,0.5f))
        {
            GameObject.Find("Player").GetComponent<PlayerController>().StopEscort();
            GameObject.Find("Player").GetComponent<PlayerController>().Respawn();
            currentState = State.Patrol;
        }

    }

    public void BreakEscort()
    {
        canBouncePlayer = false;
        Invoke(nameof(EnableBouncingPlayer),1);
        GameObject.Find("Player").GetComponent<PlayerController>().StopEscort();
       
    }

    public void EnableBouncingPlayer()
    {
        currentState = State.Patrol;
        canBouncePlayer = true;
    }

    // called when target is spotted
    public void SpotTarget(GameObject target)
    {
        if (behaviours.InSight(target.gameObject) && behaviours.InFOV(target.gameObject))
        {
            currentState = State.Investigate;
        }
    }

    // called to take damage
    public void TakeDamage(float amount)
    {
        if(GetComponent<Health>().DecreaseHealth(amount))
        {
            //AudioSource.PlayClipAtPoint(GameController.gameController.soundManager.hitHard, transform.position);
            if(alive)
            {
                //GetComponent<AudioSource>().PlayOneShot(GameController.gameController.soundManager.hitHard);
                alive = false;
                Invoke(nameof(Die), 0.25f);
            }
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            Debug.Log("player hit");
            if(canBouncePlayer)
                currentState = State.Escort;
        }
    }
}
