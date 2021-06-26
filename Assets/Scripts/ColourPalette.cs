using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPalette : MonoBehaviour
{
    [Header("Colours")]
    public Color Positive;
    public Color Negative;
    public Color Neutral;
    [Header("Public Materials")]
    public Material Material1;
    public float EmissionValue1;

    //private float t = 0;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    Color RandomColour1()
    {
        return Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 1f);
    }
    Color RandomColour2()
    {
        return Random.ColorHSV(0f, 1f, 1f, 0.5f, 0.5f, 1f);
    }

}
