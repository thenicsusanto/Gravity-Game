using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float runSpeed = 4;
    public Transform target;
    private Vector3 moveDirection;
    public Rigidbody rb;
    private Transform enemyModel;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyModel = transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = (target.transform.position - transform.position).normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + moveDirection * Time.fixedDeltaTime * runSpeed);
    }
}
