using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = FindObjectOfType<Camera>().transform;
    }

    void LateUpdate()
    {
        //transform.LookAt(transform.position + cam.forward);
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
