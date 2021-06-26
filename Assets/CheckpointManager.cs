using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public SaveState save;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*    public void SaveState()
    {

    }*/
}

public class SaveState
{
    Vector3 positionCopy;
    Quaternion rotationCopy;

    public SaveState(Transform player)
    {
        positionCopy = player.position;
        rotationCopy = player.rotation;
    }

    public void LoadState(Transform player)
    {
        player.GetComponent<CharacterController>().enabled = false;
        player.position = positionCopy;
        player.rotation = rotationCopy;
        player.GetComponent<CharacterController>().enabled = true;
    }
}