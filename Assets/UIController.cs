using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image PageIcon;
    public Text PagesCount;
    public GameObject EndScreen;
    public Text EndScreenResult;


    // Start is called before the first frame update
    void Start()
    {
        SetPageIcon(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateEndScreenResult(int value, int max)
    {
        EndScreenResult.text = value.ToString() + "/" + max;
    }

    public void UpdateNumberOfPages(int value, int max)
    {
        PagesCount.text = value.ToString()+"/"+max;
    }
    


    public void SetPageIcon(bool collected)
    {
        Color col = PageIcon.color;
        if (collected)
        {
            col.a = 1;
            PagesCount.gameObject.SetActive(true);
        }
        else
        {
            col.a = 0.25f;
            PagesCount.gameObject.SetActive(false);
        }
        PageIcon.color = col;
    }
}
