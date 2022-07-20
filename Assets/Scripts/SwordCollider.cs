using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    public EnemyController enemy;
    public Animator anim;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && other.GetComponent<EnemyController>().recentlyHit == false)
        {
            other.GetComponent<EnemyController>().EnemyTakeDamage(25);
            other.GetComponent<EnemyController>().recentlyHit = true;
            Debug.Log("Hit Enemy");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().Invoke("SetRecentlyHitFalse", anim.GetCurrentAnimatorStateInfo(0).length);
        }
    }
}
