using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public List<AttackSO> combo;
    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;
    private SwordCollider sc;
    private PlayerController pc;
    public bool attackEnded;

    Animator anim;
    private HashSet<GameObject> hitEnemies;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        sc = GetComponentInChildren<SwordCollider>();
        pc = GetComponent<PlayerController>();
        hitEnemies = new HashSet<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        ExitAttack();
    }

    public void Attack()
    {
        if (Time.time - lastComboEnd > 0.3f && comboCounter <= combo.Count)
        {
            CancelInvoke("EndCombo");
            if (Time.time - lastClickedTime >= 0.2f)
            {
                anim.runtimeAnimatorController = combo[comboCounter].animatorOV;
                pc.currentState = "Attack";
                anim.Play("Attack", 0, 0);
                sc.damageComboAdder = combo[comboCounter].damage;
                comboCounter++;
                lastClickedTime = Time.time;

                if (comboCounter + 1 > combo.Count)
                {
                    comboCounter = 0;
                }
            }
            pc.swordCollider.enabled = true;
            hitEnemies.Clear();
        }
    }

    public void ExitAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Debug.Log("Attack ended!");
            pc.swordCollider.enabled = false;
            attackEnded = true;
            Invoke("EndCombo", 1);
        }
    }

    public void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        pc.isAttacking = false;
    }

    public bool IsEnemyAlreadyHit(GameObject enemy)
    {
        return hitEnemies.Contains(enemy);
    }

    public void RegisterHitEnemy(GameObject enemy)
    {
        hitEnemies.Add(enemy);
    }
}