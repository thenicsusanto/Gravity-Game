using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float expForce;
    public float radius;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddExplosionForce(expForce, transform.position, radius);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
