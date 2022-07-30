﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public float runSpeed = 4;
    private Transform target;
    private Vector3 moveDirection;
    public Rigidbody rb;
    private Transform enemyModel;
    private float distanceToPlayer;
    public float requiredDistanceToPlayer;
    private GameObject player;
    public int damage;
    private float lastAttackTime;
    public float attackCooldown = 2;
    private int currentHealthEnemy;
    public int maxHealthEnemy = 50;
    public HealthBarEnemy healthBarEnemy;
    bool stopEnemy = false;
    public bool recentlyHit = false;
    public GameObject coinModel;
    public float coinDropCount;
    public GameObject floatingDamageText;
    public string currentState;
    public Animator anim;
    public bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyModel = transform.GetChild(0).transform;
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealthEnemy = maxHealthEnemy;
        healthBarEnemy.SetMaxHealthEnemy(maxHealthEnemy);
    }

    private void Awake()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isAttacking == false)
        {
            ChangeAnimationState("Alien Walking");
        }
        distanceToPlayer = Vector3.Distance(target.position, transform.position);
        if(distanceToPlayer <= requiredDistanceToPlayer)
        {
            stopEnemy = true;
            LookAtPlayer();
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                StartCoroutine(Attack());
            }
        } else if(distanceToPlayer >= requiredDistanceToPlayer)
        {
            LookAtPlayer();
            stopEnemy = false;
        }

        if(recentlyHit == true)
        {
            if(player.GetComponent<PlayerController>().isAttacking == false)
            {
                recentlyHit = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (stopEnemy == true)
        {
            rb.velocity = Vector3.zero;
        } else if (stopEnemy == false && isAttacking == false)
        {
            rb.MovePosition(transform.position + moveDirection * Time.fixedDeltaTime * runSpeed);
        }
        
    }

    void LookAtPlayer()
    {
        moveDirection = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(moveDirection, enemyModel.up);
        transform.rotation = rotation;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        ChangeAnimationState("Alien Attack");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
        //player.GetComponent<PlayerController>().TakeDamagePlayer(damage);
    }

    public void EnemyTakeDamage(int damage)
    {
        
        currentHealthEnemy -= damage;
        healthBarEnemy.SetHealthEnemy(currentHealthEnemy);

        if(currentHealthEnemy != 0)
        {
            ShowFloatingText();
        }

        if(currentHealthEnemy <= 0)
        {
            Die();
        }
    }

    public void SetRecentlyHitFalse()
    {
        recentlyHit = false;
    }

    void Die()
    {
        for(int i=0; i<coinDropCount; i++)
        {
            DropCoin();
        }
        GameManager.Instance.enemiesAlive--;
        ShowFloatingText();
        Destroy(gameObject);
    }
    void DropCoin()
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitCircle;
        GameObject coin = Instantiate(coinModel, transform.position + randomPos + Vector3.up, Quaternion.identity);
        Destroy(coin, 4f);
    }

    void ShowFloatingText()
    {
        GameObject damageText = Instantiate(floatingDamageText, transform.localPosition + (transform.up * 3), Quaternion.identity, transform);
        damageText.GetComponentInChildren<TextMeshPro>().text = currentHealthEnemy.ToString();
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);
        currentState = newState;
        Debug.Log(currentState);
    }
}
