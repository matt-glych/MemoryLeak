using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class PlayerController : StarterAssets.ThirdPersonController
{
    public string keycard;
    
    public float interactTimer;
    public float interactTime;

    public bool beingEscorted;
    public bool thrownOut;
    public bool canMove;

    public Transform bouncerAgent; 
    public float struggleDifficulty;
    public float struggleValue;
    public float struggleTime;
    private float struggleTimer;

    private SoundManager soundManager;

    public bool meowing;

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
        keycard = "0";

        soundManager = GameController.gameController.soundManager;
        thrownOut = false;
        meowing = false;
        beingEscorted = false;
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
        if(!beingEscorted && canMove)
            GetComponent<AudioSource>().PlayOneShot(soundManager.Grapple);
            //AudioSource.PlayClipAtPoint(soundManager.Grapple, transform.position);
        transform.position = agent.position + agent.forward * 1.25f;
        GetComponent<Collider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        transform.parent = agent;
        bouncerAgent = agent;
        beingEscorted = true;

    }

    public void StopEscort()
    {
        Vector3 pos = bouncerAgent.position - bouncerAgent.forward * 1.25f;

        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(pos, out myNavHit, 100, -1))
        {
            transform.position = myNavHit.position;
        }
        transform.position = bouncerAgent.position - bouncerAgent.forward * 1.25f;
        GetComponent<Collider>().enabled = true;
        GetComponent<CharacterController>().enabled = true;
        transform.parent = null;
        beingEscorted = false;
    }

    public void ThrownOut()
    {
        if(!thrownOut)
        {
            //AudioSource.PlayClipAtPoint(soundManager.GameOver, transform.position);
            GetComponent<AudioSource>().PlayOneShot(soundManager.GameOver);
            thrownOut = true;
        }
        Vector3 pos = bouncerAgent.position - bouncerAgent.forward * 1.25f;

        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(transform.position, out myNavHit, 100, -1))
        {
            transform.position = myNavHit.position;
        }
        //transform.position = bouncerAgent.position - bouncerAgent.forward * 1.25f;
        //GetComponent<Collider>().enabled = true;
        //GetComponent<CharacterController>().enabled = true;
        transform.parent = null;
           
        beingEscorted = false;
    }

    public void HandleInteractions()
    {
        bool interactPressed = _input.interact;
        meowing = _input.interact;

        if(interactPressed)
        {
            GetComponent<AudioSource>().PlayOneShot(soundManager.Meow);
            if (beingEscorted)
            {
                struggleValue += 0.25f;
            }
            else
            {
                
            }

            _input.interact = false;
        }
    }

    public void CollectCard(Collider card)
    {
        Debug.Log("HIT CARD");
        if (keycard == "0")
        {
            if (card.gameObject.GetComponent<Keycard>().colour == Keycard.CardColor.Red)
                keycard = "red";

            if (card.gameObject.GetComponent<Keycard>().colour == Keycard.CardColor.Blue)
                keycard = "blue";
            Destroy(card.gameObject);
        }

        else if (keycard == "red")
        {

            if (card.gameObject.GetComponent<Keycard>().colour == Keycard.CardColor.Blue)
            {
                keycard = "blue";
                GameController.gameController.levelController.DropCard("red");
                Destroy(card.gameObject);
            }
        }

        else if (keycard == "blue")
        {
            if (card.gameObject.GetComponent<Keycard>().colour == Keycard.CardColor.Red)
            {
                keycard = "red";
                GameController.gameController.levelController.DropCard("blue");
                Destroy(card.gameObject);
            }    
        }

        GameObject.Find("UI").GetComponent<UIController>().SetCardIcon(keycard);
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //GameController.gameController.checkpointManager.save.LoadState(this.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        // hit card
        if (other.gameObject.GetComponent<Keycard>() != null)
        {
            //AudioSource.PlayClipAtPoint(soundManager.pickupPaper, transform.position);
            GetComponent<AudioSource>().PlayOneShot(soundManager.pickupPaper);
            CollectCard(other);

        }

        // hit door
        if (other.gameObject.GetComponent<Door>() != null)
        {
            Debug.Log("HIT DOOR");
            if(!other.GetComponent<Door>().isOpen)
            {
               
                if(other.GetComponent<Door>().colour == Door.DoorColor.Red)
                {
                    if(keycard == "red")
                    {
                        //AudioSource.PlayClipAtPoint(soundManager.doorOpen, other.transform.position);
                        GetComponent<AudioSource>().PlayOneShot(soundManager.doorOpen);
                        other.GetComponent<Door>().Open();
                    }
                    else
                    {
                        //AudioSource.PlayClipAtPoint(soundManager.doorLocked, other.transform.position);
                        GetComponent<AudioSource>().PlayOneShot(soundManager.doorLocked);
                    }
                       
                }

                if (other.GetComponent<Door>().colour == Door.DoorColor.Blue)
                {
                    if (keycard == "blue")
                    {
                        //AudioSource.PlayClipAtPoint(soundManager.doorOpen, other.transform.position);
                        GetComponent<AudioSource>().PlayOneShot(soundManager.doorOpen);
                        other.GetComponent<Door>().Open();
                    }
                    else
                    {
                        //AudioSource.PlayClipAtPoint(soundManager.doorLocked, other.transform.position);
                        GetComponent<AudioSource>().PlayOneShot(soundManager.doorLocked);
                    }
                        
                }
            }
                
        }
        // hit water
        if (other.gameObject.GetComponent<Water>() != null)
        {
            Debug.Log("HIT WATER");
            GameController.gameController.gameOver = true;
            if(canMove)
                //AudioSource.PlayClipAtPoint(soundManager.hitWater,transform.position);
                GetComponent<AudioSource>().PlayOneShot(soundManager.hitWater);
            canMove = false;
            Invoke(nameof(Respawn), 2f);
            //Respawn();
        }

        // hit suitcase
        if (other.gameObject.GetComponent<Suitcase>()!= null)
        {
            //AudioSource.PlayClipAtPoint(soundManager.Meow, transform.position);
            GetComponent<AudioSource>().PlayOneShot(soundManager.Meow);
            GameController.gameController.soundManager.SetMusic2();
            GameController.gameController.levelController.CollectItem(other.gameObject.transform);
        }
        // hit page
        if (other.gameObject.GetComponent<Page>() != null)
        {
            //AudioSource.PlayClipAtPoint(soundManager.pickupPaper, transform.position);
            GetComponent<AudioSource>().PlayOneShot(soundManager.pickupPaper);
            GameController.gameController.levelController.CollectItem(other.gameObject.transform);
        }
        // hit player exit
        if (other.gameObject.tag == "PlayerExit")
        {
            GameController.gameController.gameOver = true;
            //AudioSource.PlayClipAtPoint(soundManager.Meow, transform.position);
            GetComponent<AudioSource>().PlayOneShot(soundManager.Meow);
            GameController.gameController.levelController.OnLevelComplete();
        }
        

        if (other.gameObject.GetComponent<EnemyAi>() != null)
        {
            //Debug.Log("HIT");
            //Respawn();
        }
    }
}
