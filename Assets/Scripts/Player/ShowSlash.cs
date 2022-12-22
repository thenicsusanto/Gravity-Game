using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSlash : MonoBehaviour
{
    public GameObject slash;
    public ShopManager shopManager;
    public SwordCollider swordCollider;

    void Start()
    {
        swordCollider = GetComponent<SwordCollider>();
    }

    void EnableSlashVFX()
    {
        slash.SetActive(true);
    }

    void DisableSlashVFX()
    {
        slash.SetActive(false);
    }

    void ShowExplosion()
    {
        swordCollider.PlayExplosion();
    }

    void MeleeAttackEnd()
    {
        GetComponentInParent<MeleeEnemyController>().swordCollider.enabled = false;
        GetComponentInParent<MeleeEnemyController>().isAttacking = false;
    }

    void PlayerAttackEnd()
    {
        GetComponentInParent<PlayerController>().swordCollider.enabled = false;
        GetComponentInParent<PlayerController>().isAttacking = false;
        GetComponentInParent<PlayerController>().canMove = true;
    }
}
