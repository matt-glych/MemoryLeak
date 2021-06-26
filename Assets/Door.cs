using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{
    public Renderer gfx;
    public enum DoorColor {Red, Green, Blue };
    public DoorColor colour;

    public Transform openPos;
    public float openSpeed;

    public bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        switch(colour)
        {
            case DoorColor.Red:
                gfx.material.color = Color.red;
            break;
            case DoorColor.Green:
                gfx.material.color = Color.green;
                break;
            case DoorColor.Blue:
                gfx.material.color = Color.blue;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Open()
    {
        Debug.Log("DOOR OPENING");
        StartCoroutine(MoveOverSeconds(this.gameObject, openPos.position, 1));
        isOpen = true;
    }

    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = end;
    }

}
