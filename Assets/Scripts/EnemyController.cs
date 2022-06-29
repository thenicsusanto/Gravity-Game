using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float runSpeed = 4;
    private Transform target;
    private Vector3 moveDirection;
    public Rigidbody rb;
    private Transform enemyModel;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyModel = transform.GetChild(0).transform;
    }

    private void Awake()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();
    }
    
    void FixedUpdate()
    {
        rb.MovePosition(transform.position + moveDirection * Time.fixedDeltaTime * runSpeed);
    }

    void LookAtPlayer()
    {
        moveDirection = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(moveDirection, enemyModel.up);
        transform.rotation = rotation;
    }
}
