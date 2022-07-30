using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAttack : MonoBehaviour
{
    public PlayerController player;
    public Animator anim;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamagePlayer(25);
            Debug.Log("Hit Player");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<EnemyController>().Invoke("SetRecentlyHitFalse", anim.GetCurrentAnimatorStateInfo(0).length);
        }
    }
}
