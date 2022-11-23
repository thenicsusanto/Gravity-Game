using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAttack : MonoBehaviour
{
    public PlayerController player;
    public Animator anim;
    private MeleeEnemyController enemyController;

    void Start()
    {
        enemyController = GetComponentInParent<MeleeEnemyController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enemyController.recentlyHitPlayer == false) { 
            other.GetComponent<PlayerController>().TakeDamagePlayer(25);
            enemyController.lastAttackTime = Time.time;
            enemyController.recentlyHitPlayer = true;
            StartCoroutine(SetPlayerHitFalse());
        }
    }

    IEnumerator SetPlayerHitFalse()
    {
        yield return new WaitForSeconds(enemyController.anim.GetCurrentAnimatorStateInfo(0).length);
        enemyController.recentlyHitPlayer = false;
    }
}
