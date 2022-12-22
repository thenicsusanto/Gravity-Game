using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnemyProjectile : MonoBehaviour
{
    public RangedEnemyController rangedEnemyController;
    public GameObject projectilePrefab;
    public Transform shotPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, shotPoint.rotation);
        projectile.GetComponent<Rigidbody>().velocity = shotPoint.transform.forward * 6;
        rangedEnemyController.isAttacking = false;
    }
}
