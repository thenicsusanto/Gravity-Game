using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    public Transform parentObject;
    private MeleeEnemyController meleeEnemyCollidedWith;
    private RangedEnemyController rangedEnemyCollidedWith;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if(other.GetComponent<MeleeEnemyController>() && other.GetComponent<MeleeEnemyController>().recentlyHit == false)
            {
                meleeEnemyCollidedWith = other.GetComponent<MeleeEnemyController>();
                Vector3 toTarget = (other.transform.position - parentObject.position).normalized;

                if (Vector3.Dot(toTarget, parentObject.forward) > 0)
                {
                    other.GetComponent<MeleeEnemyController>().MeleeEnemyTakeDamage(25);
                    other.GetComponent<MeleeEnemyController>().recentlyHit = true;
                    other.GetComponent<MeleeEnemyController>().anim.Rebind();
                    if (meleeEnemyCollidedWith.currentState != "Reaction Hit")
                    {
                        other.GetComponent<MeleeEnemyController>().ChangeAnimationState("Reaction Hit");
                        StartCoroutine(SetMeleeHitReactionFalse());
                    }
                    else if (meleeEnemyCollidedWith.currentState == "Reaction Hit")
                    {
                        meleeEnemyCollidedWith.anim.CrossFade("Reaction Hit", 0.1f);

                    }
                }
                else
                {
                    return;
                }
            } else if(other.GetComponent<RangedEnemyController>() && other.GetComponent<RangedEnemyController>().recentlyHit == false)
            {
                rangedEnemyCollidedWith = other.GetComponent<RangedEnemyController>();
                Vector3 toTarget = (other.transform.position - parentObject.position).normalized;

                if (Vector3.Dot(toTarget, parentObject.forward) > 0)
                {
                    rangedEnemyCollidedWith.RangedEnemyTakeDamage(25);
                    rangedEnemyCollidedWith.recentlyHit = true;
                    rangedEnemyCollidedWith.anim.Rebind();
                    if (rangedEnemyCollidedWith.currentState != "SpiderHitReaction")
                    {
                        other.GetComponent<RangedEnemyController>().ChangeAnimationState("SpiderHitReaction");
                        StartCoroutine(SetRangedHitReactionFalse());
                    }
                    else if (rangedEnemyCollidedWith.currentState == "SpiderHitReaction")
                    {
                        rangedEnemyCollidedWith.anim.CrossFade("SpiderHitReaction", 0.1f);

                    }
                } 
                else
                {
                    return;
                }
            }
        }
    }

    IEnumerator SetMeleeHitReactionFalse()
    {
        yield return new WaitForSeconds(meleeEnemyCollidedWith.anim.GetCurrentAnimatorStateInfo(0).length);
    }

    IEnumerator SetRangedHitReactionFalse()
    {
        yield return new WaitForSeconds(rangedEnemyCollidedWith.anim.GetCurrentAnimatorStateInfo(0).length);
    }
}
