using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    public Renderer gfx;
    public enum CardColor { Red, Green, Blue };
    public CardColor colour;

    // Start is called before the first frame update
    void Start()
    {
        switch (colour)
        {
          /*  case CardColor.Red:
                gfx.material.color = Color.red;
                break;
            case CardColor.Green:
                gfx.material.color = Color.green;
                break;
            case CardColor.Blue:
                gfx.material.color = Color.blue;
                break;*/
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
