using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    // inspector variables
    [SerializeField, Tooltip("Attractor object to be drawn to, if left blank first available world will be used")]
    private GravityAttractor attractor;

    // privates
    private Transform myTransform;
    private Rigidbody rb;

    // Use this for initialization
    private void Start()
    {
        // set rigidbody
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (attractor != null)
        {
            attractor.Attract(myTransform);
        }
    }
}
