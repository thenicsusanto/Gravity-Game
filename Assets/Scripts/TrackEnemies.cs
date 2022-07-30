using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEnemies : MonoBehaviour
{
    private GameObject[] multipleEnemies;
    public Transform closestEnemy;
    public bool enemyContact;
    public LayerMask enemyLayer;
    // Start is called before the first frame update
    void Start()
    {
        closestEnemy = null;
        enemyContact = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Physics.CheckSphere(transform.position, 4, enemyLayer))
        {
            enemyContact = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.isTrigger != true && other.CompareTag("Enemy"))
        {
            if (closestEnemy != null)
            {
                //closestEnemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            }

            closestEnemy = getClosestEnemy();
            //closestEnemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            enemyContact = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger != true && other.CompareTag("Enemy"))
        {
            //closestEnemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            enemyContact = false;
        }
    }

    void OnDestroy()
    {
        if (closestEnemy != null)
        {
            //closestEnemy.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
    }

    public Transform getClosestEnemy()
    {
        multipleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestTransform = null;
        foreach(GameObject enemy in multipleEnemies)
        {
            float currentDistance;
            currentDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if(currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestTransform = enemy.transform;
            }
        }
        if(closestEnemy != null)
        {
            Debug.DrawLine(transform.position, closestEnemy.transform.position);
        }
        return closestTransform;
    }
}
