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

    Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        sc = GetComponentInChildren<SwordCollider>();
        pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //ExitAttack();
    }

    public void Attack()
    {
        if(Time.time - lastComboEnd > 0.3f && comboCounter <= combo.Count)
        {
            CancelInvoke("EndCombo");
            if(Time.time - lastClickedTime >= 0.2f)
            {
                anim.runtimeAnimatorController = combo[comboCounter].animatorOV;
                pc.currentState = "Attack";
                anim.Play("Attack", 0, 0);
                sc.damageComboAdder = combo[comboCounter].damage;
                comboCounter++;
                lastClickedTime = Time.time;

                if(comboCounter + 1 > combo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
    }

    public void ExitAttack()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo", 1);
        }
    }

    public void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        pc.isAttacking = false;
    }
}
