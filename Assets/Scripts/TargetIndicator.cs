using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    private Transform target;
    public TrackEnemies trackEnemies;
    public bool enemyInAttackRange;
    public LayerMask enemyLayer;

    void Update()
    {
        if (!Physics.CheckSphere(transform.position, 4, enemyLayer))
        {
            enemyInAttackRange = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger != true && other.CompareTag("Enemy"))
        {
            enemyInAttackRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger != true && other.CompareTag("Enemy"))
        {
            enemyInAttackRange = false;
        }
    }
}
