using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // music
    public AudioClip Music1;
    public AudioClip Music2;
    public AudioClip GameOver;
    // cat sounds
    public AudioClip[] Meows;
    public AudioClip DropPaper;
    public AudioClip Grapple;
    // mouse sounds
    public AudioClip mouse_1;
    public AudioClip mouse_2;
    // sound fx
    public AudioClip doorLocked;
    public AudioClip doorOpen;
    public AudioClip pickupPaper;
    public AudioClip hitWater;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioClip Meow
    {
        get => Meows[Random.Range(0, Meows.Length)];
    }
}
