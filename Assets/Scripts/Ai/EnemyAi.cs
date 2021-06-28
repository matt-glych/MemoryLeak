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
    public bool bouncingPlayer;

    public bool heardFootstep;

    private Quaternion grabRot;

    public Transform Arms;

    private bool setAnims;

    public Animator anim;


    public Transform Shoulder_L;
    public Transform Shoulder_R;

    // Start is called before the first frame update
    void Start()
    {
       // set components
        nav = GetComponent<NavMeshAgent>();
        nav.angularSpeed = 1000;
        nav.acceleration = 15;
        nav.autoBraking = true;
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

        //grabRot = Quaternion.Euler(0, 0, -90);

       
    }


    // Update is called once per frame
    void Update()
    {
        if(!GameController.gameController.gameOver)
            CalculateBehaviour();
    }

    void CalculateBehaviour()
    {
        // passive behaviours
        
        if(GetComponent<CharacterController>().velocity.magnitude>0.1f && !bouncingPlayer)
        {
            anim.SetBool("walk", true);
        }
        else
        {
            anim.SetBool("walk", false);
        }

        // enemy states
        switch (currentState)
        {
            case State.Idle:
                maxSpeed = walkSpeed;
                //HearFootsteps();
                Idle();
                break;

            case State.Patrol:
                maxSpeed = walkSpeed;
                HearFootsteps();
                SpotTarget(target.gameObject);
                Patrol();
                break;

            case State.Investigate:
                HearFootsteps();
                SpotTarget(target.gameObject);
                Investigate();
                break;

            case State.Chase:
                maxSpeed = runSpeed;
                Chase();
                break;
            case State.Escort:
                maxSpeed = runSpeed;
                Escort();
                break;
        }

        nav.speed = maxSpeed;
    }

    // hear nearvy footsteps
    public void HearFootsteps()
    {
        if(!GameObject.Find("Player").GetComponent<PlayerController>().beingEscorted && !bouncingPlayer && !heardFootstep)
        {
            Transform player = GameObject.Find("Player").transform;

            // detect close running sound
            if (Vector3.Distance(GameObject.Find("Player").transform.position, transform.position) < sightRange / 2)
            {
                if (target.GetComponent<CharacterController>().velocity.magnitude > 2.5f || GameObject.Find("Player").GetComponent<PlayerController>().meowing)
                {
                    Debug.Log("TARGET HEARD FROM BEHIND!");

                    NavMeshHit myNavHit;
                    if (NavMesh.SamplePosition(player.transform.position, out myNavHit, 1, -1))
                    {
                        Invoke(nameof(AllowHearFootsteps), 2f);
                        heardFootstep = true;
                        PatrolPoints.Insert(currentPatrolPoint, myNavHit.position);
                        currentState = State.Patrol;
                        //PatrolPoints.Insert(PatrolPoints.Count-1, myNavHit.position);
                    }
                }
            }
        }
    }

    public void AllowHearFootsteps()
    {
        heardFootstep = false;
    }
 
    // Idle; state behaviour
    public void Idle()
    {
        anim.SetBool("chase", false);
        anim.speed = 1;
    }

    // Patrol; state behaviour
    public void Patrol()
    {
        
        anim.SetBool("walk", true);
        anim.SetBool("chase", false);
        anim.speed = 1;

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
        anim.SetBool("chase", false);
        anim.speed = 1;

        currentState = State.Chase;
    }

    // Chase; state behaviour
    public void Chase()
    {
        //Debug.Log("CHASING");
        //anim.SetBool("carry", true);
        if (GameObject.Find("Player").GetComponent<PlayerController>().beingEscorted && !bouncingPlayer)
        {
            currentState = State.Patrol;
        }

        /*anim.SetBool("carry", true);
        anim.Play("Carry1");*/
        anim.SetBool("chase", true);
        //RaiseArms(Shoulder_L);

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
        anim.SetBool("chase", true);
        anim.speed = -1;

        if (GameObject.Find("Player").GetComponent<PlayerController>().beingEscorted  && !bouncingPlayer)
        {
            currentState = State.Patrol;

        }

        // escort target
        Vector3 targetPoint = GameController.gameController.levelController.ExitPoints[0].transform.position;
        nav.SetDestination(targetPoint);

        GameObject.Find("Player").GetComponent<PlayerController>().OnEscort(this.transform);


        if(behaviours.InPosition(targetPoint,1f))
        {
            //PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
            GameObject.Find("Player").GetComponent<PlayerController>().canMove = false;
            //GameObject.Find("Player").GetComponent<PlayerController>().StopEscort();
            GameObject.Find("Player").GetComponent<PlayerController>().ThrownOut();
            GameController.gameController.gameOver = true;
            Invoke(nameof(RespawnPlayer),1f);
            //currentState = State.Patrol;
        }

    }

    public void RaiseArms(Transform part)
    {
        Debug.Log("RAISING ARMS");
        part.Rotate(Vector3.up * (4 * Time.deltaTime));

    }


    public void RespawnPlayer()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().Respawn();
    }

    public void BreakEscort()
    {
        anim.SetBool("chase", false);
        anim.speed = 1;

        canBouncePlayer = false;
        bouncingPlayer = false;

        //Arms.Rotate(0, 0, 90);

        currentState = State.Idle;
        Invoke(nameof(EnableBouncingPlayer),2.5f);
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
        if(!GameObject.Find("Player").GetComponent<PlayerController>().beingEscorted && !bouncingPlayer)
        {
            if (behaviours.InSight(target.gameObject) && behaviours.InFOV(target.gameObject))
            {
                currentState = State.Investigate;
            }
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
            if(!GameObject.Find("Player").GetComponent<PlayerController>().beingEscorted && !bouncingPlayer)
            {
                Debug.Log("player hit");
                if (canBouncePlayer)
                {
                    bouncingPlayer = true;

                    //Arms.Rotate(0, 0, -90);

                    currentState = State.Escort;
                }
            }
        }
    }
}
