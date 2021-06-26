using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 1f;
    public Vector3 distance = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
    }


    void Follow()
    {
        Vector3 cameraPos = target.position;
        cameraPos += distance;

        if(transform.position != cameraPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, cameraPos, followSpeed * Time.deltaTime);
        }
    }
}
