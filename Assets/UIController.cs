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

    public Image CardRed;
    public Image CardBlue;

    public GameObject[] Pages;

     
    // Start is called before the first frame update
    void Start()
    {
        SetPageIcon(false);
        SetCardIcon("0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateEndScreenResult(int value, int max)
    {
        EndScreenResult.text = value.ToString() + "/" + max;

        for (int i = 0; i < value; i++)
        {
            Pages[i].SetActive(true);
        }
    }

    public void UpdateNumberOfPages(int value, int max)
    {
        PagesCount.text = value.ToString()+"/"+max;
    }
    

    public void SetCardIcon(string type)
    {
        switch(type)
        {
            case "red":
                CardRed.gameObject.SetActive(true);
                CardBlue.gameObject.SetActive(false);
                break;

            case "blue":
                CardRed.gameObject.SetActive(false);
                CardBlue.gameObject.SetActive(true);
                break;

            case "0":
                CardRed.gameObject.SetActive(false);
                CardBlue.gameObject.SetActive(false);
                break;
        }
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
