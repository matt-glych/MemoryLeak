using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, Patrol, Investigate, Chase, Attack }
public class EnemyAgent : MonoBehaviour
{
    // components
    private NavMeshAgent nav;
    public AgentBehaviours behaviours;
    // state variables
    public EnemyState currentState;
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
    public float patrolPointMinDistance;

    // behaviour weights
    public float PursueWeight = 0.0f;
    public float PatrolWeight = 0.0f;

    public bool enablemovement;
    public bool enablePhysics;

    public float chaseTime;
    private float chaseTimer;

    // Start is called before the first frame update
    void Start()
    {
        currentState = EnemyState.Idle;
        behaviours = new AgentBehaviours(this);
        nav = GetComponent<NavMeshAgent>();
        //nav.updateRotation = false;
        alive = true;

        //get patrol points, using initial position as the first point
        PatrolPoints.Insert(0, gameObject.transform.position);
        foreach (Transform t in PatrolPointObjects)
        {
            PatrolPoints.Add(t.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateBehaviour();
    }

    void CalculateBehaviour()
    {
       if(targetSpotted)
        {
            behaviours.LookAtTarget(target.position);
        }


        Vector3 steering = Vector3.zero;

        Vector3 seekForce = behaviours.Seek(target.transform.position);
        Vector3 patrolForce = behaviours.Patrol();
        steering += seekForce * PursueWeight;
        steering += patrolForce * PatrolWeight;

        // enemy states
        switch (currentState)
        {
            case EnemyState.Idle:
                currentState = EnemyState.Patrol;
                behaviours.SpotTargets(target.gameObject);
                PatrolWeight = 0;
                break;

            case EnemyState.Patrol:
                behaviours.SpotTargets(target.gameObject);
                behaviours.LookAtTarget(PatrolPoints[currentPatrolPoint]);
                PatrolWeight = 1;
                break;

            case EnemyState.Chase:

                /*chaseTimer += Time.deltaTime;
                if(60%chaseTimer > chaseTime)
                {
                    currentState = EnemyState.Patrol;
                    chaseTimer = 0;
                }*/
                behaviours.SpotTargets(target.gameObject);
                behaviours.LookAtTarget(target.position);
                PursueWeight = 1;
                PatrolWeight = 0;
                break;
        }


        Vector3 targetPoint = transform.position + steering;

        nav.SetDestination(targetPoint);
    }

    // called when target is spotted
    public void TargetSpotted(GameObject target)
    {
        targetSpotted = true;
        currentState = EnemyState.Chase;
        //behaviour.LookAtTarget(target.transform.position);
        behaviours.LookAtXConstZ(this.transform, target.transform.position);
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

}
