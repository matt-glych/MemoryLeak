using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{
    public UIController UIController;

    //public GameObject[] Pages;
    public GameObject Suitcase;
    public GameObject Page;
    public GameObject CardRed;
    public GameObject CardBlue;

    public Transform[] ExitPoints;


    public bool hasPages;
    bool canCollectPage;
    public float dropTime;
    private float dropTimer;

    public int totalPages;
    public int pagesCollected;


    // Start is called before the first frame update
    void Start()
    {
        canCollectPage = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasPages)
        {
            dropTimer += Time.deltaTime;

            if((dropTimer % 60) > dropTime)
            {
                Debug.Log("DROP PAGE");
                DropPage();
                dropTimer = 0;
            }
        }
    }

    // collect dropped page
    public void CollectItem(Transform obj)
    {
        switch(obj.gameObject.tag)
        {
            case "Suitcase":
                Destroy(obj.gameObject);
                CollectSuitcase();
                break;

            case "Page":
                Destroy(obj.gameObject);
                CollectPage();
                break;
    
        }
    }


    public void OnLevelComplete()
    {
        Debug.Log("LEVEL COMPLETE");
        GameObject.Find("Player").GetComponent<PlayerController>().canMove = false;
        UIController.EndScreen.SetActive(true);
        UIController?.UpdateEndScreenResult(pagesCollected, totalPages);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CollectSuitcase()
    {
        //GameController.gameController.uiController.SetPageIcon(true);
        
        totalPages = Suitcase.GetComponent<Suitcase>().totalPages;
        pagesCollected = Suitcase.GetComponent<Suitcase>().totalPages;
        UIController?.UpdateNumberOfPages(pagesCollected, totalPages);
        UIController?.SetPageIcon(true);
        Suitcase.SetActive(false);
        hasPages = true;
    }
    public void CollectPage()
    {
        pagesCollected++;
        UIController.UpdateNumberOfPages(pagesCollected, totalPages);
    }

    public void DropPage()
    {
        if(pagesCollected>0)
        {
            pagesCollected--;
            UIController.UpdateNumberOfPages(pagesCollected, totalPages);
            //canCollectPage = false;
            //Page.SetActive(true);
            Vector3 pos = Vector3.zero;
            if (GameObject.Find("Player").GetComponent<PlayerController>().beingEscorted)
            {
                pos = GameObject.Find("Player").GetComponent<PlayerController>().bouncerAgent.transform.position + -GameObject.Find("Player").GetComponent<PlayerController>().bouncerAgent.transform.forward * 2;
               
            }
            else
            {
                pos = GameObject.Find("Player").transform.position + -GameObject.Find("Player").transform.forward * 2;
            }
            
            pos.y += 0.25f;

            NavMeshHit myNavHit;
            if (NavMesh.SamplePosition(pos, out myNavHit, 1, -1))
            {
                pos = myNavHit.position;
            }

            Quaternion rotation = Quaternion.Euler(90, 90, 90);
            GameObject page = Instantiate(Page, pos, rotation);
            page.SetActive(true);
            //Page.transform.position = GameController.gameController.Player.transform.position;
        }
    }

    public void DropCard(string colour)
    {
        Vector3 pos = Vector3.zero;
        pos = GameObject.Find("Player").transform.position + -GameObject.Find("Player").transform.forward * 2;
        pos.y += 0.5f;
        Quaternion rotation = Quaternion.Euler(90, 0, 0);

        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(pos, out myNavHit, 1, -1))
        {
            pos = myNavHit.position;
        }

        switch (colour)
        {
            case "red":
                Instantiate(CardRed, pos, Quaternion.identity);
                break;
            case "blue":
                Instantiate(CardBlue, pos, Quaternion.identity);
                break;
        }
    }
}
