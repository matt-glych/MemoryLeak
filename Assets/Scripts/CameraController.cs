using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookSpeed = 3;
    private Vector2 rotation;
    public void Look() // Look rotation
    {
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -15f, 15f);
        transform.eulerAngles = new Vector2(rotation.x, rotation.y) * lookSpeed;
        //transform.localRotation = Quaternion.Euler(rotation.x * lookSpeed, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Look();
    }
}
