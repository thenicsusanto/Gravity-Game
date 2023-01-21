using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSlash : MonoBehaviour
{
    public GameObject slash;
    public ShopManager shopManager;
    public SwordCollider swordCollider;
    public Transform sword;
    public TrackEnemies trackEnemies;

    void Start()
    {
        swordCollider = GetComponent<SwordCollider>();
    }

    void EnableSlashVFX()
    {
        StartCoroutine(PlaySlashVFX());
    }

    IEnumerator PlaySlashVFX()
    {
        GameObject newSlash = Instantiate(slash, sword);
        newSlash.transform.parent = null;
        yield return new WaitForSeconds(1.5f);
        Destroy(newSlash);
    }
    void DisableSlashVFX()
    {
        slash.SetActive(false);
    }

    void ShowExplosion()
    {
        swordCollider.PlayExplosionFunction();
    }

    void PlayMeteors()
    {
        FindObjectOfType<AudioManager>().Play("MeteorSummon");
        trackEnemies.SummonMeteors();
    }

    void PlaySound()
    {
        FindObjectOfType<AudioManager>().Play("EnemySlash");
    }

    void MeleeAttackEnd()
    {
        GetComponentInParent<MeleeEnemyController>().swordCollider.enabled = false;
        GetComponentInParent<MeleeEnemyController>().isAttacking = false;
    }

    void PlayerAttackEnd()
    {
        GetComponentInParent<PlayerController>().isAttacking = false;
        GetComponentInParent<PlayerController>().swordCollider.enabled = false;
        GetComponentInParent<PlayerController>().canMove = true;
    }
}
