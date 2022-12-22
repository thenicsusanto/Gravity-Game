using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeleeEnemyController : MonoBehaviour
{
    public float runSpeed = 4;
    private Transform target;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private Transform enemyModel;
    private float distanceToPlayer;
    public float requiredDistanceToPlayer;
    private GameObject player;
    public int damage;
    public float lastAttackTime;
    public float attackCooldown = 2;
    public int currentHealthEnemy;
    public int maxHealthEnemy = 50;
    public MeleeHealthBarEnemy meleeHealthBarEnemy;
    bool stopEnemy = false;
    public bool recentlyHit = false;
    public GameObject coinModel;
    public float coinDropCount;
    public GameObject floatingDamageText;
    public string currentState;
    public Animator anim;
    public bool isAttacking;
    public bool recentlyHitPlayer;
    public Collider swordCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyModel = transform.GetChild(0).transform;
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealthEnemy = maxHealthEnemy;
        meleeHealthBarEnemy.SetMaxHealthEnemy(maxHealthEnemy);
    }

    void Awake()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(target.position, transform.position);
        if (isAttacking == false && (distanceToPlayer > requiredDistanceToPlayer))
        { 
            ChangeAnimationState("Alien Walking");
        }

        if(distanceToPlayer <= requiredDistanceToPlayer)
        {
            stopEnemy = true;
            if (GetComponent<Rigidbody>().constraints != RigidbodyConstraints.FreezeAll)
            {
                LookAtPlayer();
            }
            if (Time.time - lastAttackTime >= attackCooldown && anim.GetCurrentAnimatorStateInfo(0).IsName("Alien Idle"))
            {
                Attack();
            } else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Reaction Hit") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                Attack();
            } else if(!isAttacking && currentState != "Reaction Hit")
            {
                ChangeAnimationState("Alien Idle");
            }
        } else if(distanceToPlayer > requiredDistanceToPlayer)
        {
            LookAtPlayer();
            stopEnemy = false;
        }

        if(recentlyHit == true)
        {
            Attack();
            if(player.GetComponent<PlayerController>().isAttacking == false)
            {
                recentlyHit = false;
            }
        }

        if(isAttacking)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Alien Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
            {
                swordCollider.enabled = true;
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

    public void Attack()
    {
        isAttacking = true;
        LookAtPlayer();
        ChangeAnimationState("Alien Attack");
    }

    public void MeleeEnemyTakeDamage(int damage)
    {
        currentHealthEnemy -= damage;
        meleeHealthBarEnemy.SetHealthEnemy(currentHealthEnemy);

        if(currentHealthEnemy != 0)
        {
            ShowFloatingText();
        }

        if(currentHealthEnemy <= 0)
        {
            Die();
        }
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            if (recentlyHit == false)
            {
                MeleeEnemyTakeDamage(other.GetComponent<SwordCollider>().damageToTake);
                recentlyHit = true;
                anim.Rebind();
                if (currentState != "Reaction Hit")
                {
                    ChangeAnimationState("Reaction Hit");
                    StartCoroutine(SetMeleeHitReactionFalse());
                }
                else if (currentState == "Reaction Hit")
                {
                    anim.CrossFade("Reaction Hit", 0.1f);
                }
            }
        }
     }

    IEnumerator SetMeleeHitReactionFalse()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    IEnumerator PauseCoroutine(bool condition)
    {
        while(condition == true)
        {
            yield return null;
        }
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);
        currentState = newState;
    }
}
