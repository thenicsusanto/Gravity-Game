using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemy : MonoBehaviour
{
    public PlayerController playerController;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public int attackDamage = 25;
    
    public void DetectEnemy()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, 4, enemyLayers);

        foreach(Collider enemy in hitEnemies)
        {
            
            if(enemy.GetComponent<EnemyController>().recentlyHit == false)
            {
                enemy.GetComponent<EnemyController>().recentlyHit = true;
                enemy.GetComponent<EnemyController>().EnemyTakeDamage(attackDamage);
            }
        }
    }

    public void SetRecentlyHitFalse()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach(EnemyController enemy in enemies)
        {
            enemy.recentlyHit = false;
        }
    }
}
