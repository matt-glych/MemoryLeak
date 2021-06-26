using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : StarterAssets.ThirdPersonController
{
    public float interactTimer;
    public float interactTime;

    public bool beingEscorted;
    public bool canMove;

    public Transform bouncerAgent; 
    public float struggleDifficulty;
    public float struggleValue;
    public float struggleTime;
    private float struggleTimer;

    public override void AssignAnimationIDs()
    {
        base.AssignAnimationIDs();
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void CameraRotation()
    {
        base.CameraRotation();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override void GroundedCheck()
    {
        base.GroundedCheck();
    }

    public override void JumpAndGravity()
    {
        base.JumpAndGravity();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void Move()
    {
        base.Move();
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }

    public override void Start()
    {
        base.Start();
        canMove = true;
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override void Update()
    {
        if(canMove)
            base.Update();

        HandleInteractions();

        if(beingEscorted)
        {
            //Debug.Log(struggleValue);
            if(struggleValue >0)
            {
                struggleValue -= 0.2f * Time.deltaTime;
            }

            if(struggleValue >= struggleDifficulty+1)
            {
                bouncerAgent.GetComponent<EnemyAi>().BreakEscort();
                struggleValue = 0;
            }
        }
    }

    public void OnEscort(Transform agent)
    {
        transform.position = agent.position + agent.forward * 1.25f;
        GetComponent<Collider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        transform.parent = agent;
        bouncerAgent = agent;
        beingEscorted = true;

    }

    public void StopEscort()
    {
        transform.position = bouncerAgent.position - bouncerAgent.forward * 1.25f;
        GetComponent<Collider>().enabled = true;
        GetComponent<CharacterController>().enabled = true;
        transform.parent = null;
        beingEscorted = false;
    }

    public void HandleInteractions()
    {
        bool interactPressed = _input.interact;

        if(interactPressed)
        {
            if(beingEscorted)
            {
                struggleValue += 0.25f;
            }

            _input.interact = false;
        }
    }



    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //GameController.gameController.checkpointManager.save.LoadState(this.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        // hit water
        if (other.gameObject.GetComponent<Water>() != null)
        {
            Debug.Log("HIT WATER");
            Respawn();
        }

        // hit suitcase
        if (other.gameObject.GetComponent<Suitcase>()!= null)
        {
            GameController.gameController.levelController.CollectItem(other.gameObject.transform);
        }
        // hit page
        if (other.gameObject.GetComponent<Page>() != null)
        {
            GameController.gameController.levelController.CollectItem(other.gameObject.transform);
        }
        // hit player exit
        if (other.gameObject.tag == "PlayerExit")
        {
            GameController.gameController.levelController.OnLevelComplete();
        }
        

        if (other.gameObject.GetComponent<EnemyAi>() != null)
        {
            //Debug.Log("HIT");
            //Respawn();
        }
    }
}
