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
    public ParticleSystem burningVFX;
    private float currentBurnTime;
    public float burnCooldown;
    public bool isDead = false;

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
        if (isAttacking == false && (distanceToPlayer > requiredDistanceToPlayer) && isDead == false)
        { 
            ChangeAnimationState("Alien Walking");
        }

        if(distanceToPlayer <= requiredDistanceToPlayer && isDead == false)
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
            //Attack();
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
        } else if (stopEnemy == false && isAttacking == false && isDead == false)
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
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        ChangeAnimationState("Alien Death");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        for (int i = 0; i < coinDropCount; i++)
        {
            //Change this to just show coin number not actally drop coins because that takes too long
            DropCoin();
        }
        GameManager.Instance.enemiesAlive--;
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
        if(currentState == "Alien Death")
        {
            return;
        }
        if (other.CompareTag("Sword"))
        {
            if (recentlyHit == false)
            {
                MeleeEnemyTakeDamage(other.GetComponent<SwordCollider>().damageToTake);
                if(currentHealthEnemy <= 0)
                {
                    return;
                }
                recentlyHit = true;
                if (currentState != "Reaction Hit")
                {
                    if(player.GetComponent<PlayerController>().playerWeaponIndex == 3)
                    {
                        burningVFX.Play();
                        currentBurnTime = Time.time;

                        StartCoroutine(BurnEnemy(5f, 15));
                    }
                    if(currentState == "Alien Attack")
                    {
                        isAttacking = false;
                        ChangeAnimationState("Reaction Hit");
                    } else
                    {
                        ChangeAnimationState("Reaction Hit");
                    }
                    //StartCoroutine(SetMeleeHitReactionFalse());
                }
                else if (currentState == "Reaction Hit")
                {
                    RestartAnimationState("Reaction Hit");
                    //StartCoroutine(SetMeleeHitReactionFalse());
                }
            }
        }
     }

    IEnumerator BurnEnemy(float burnTime, int damagePerTick)
    {
        float currentTime = Time.time;
        burningVFX.Play();
        while (Time.time < currentTime + burnTime)
        {
            MeleeEnemyTakeDamage(damagePerTick);
            yield return new WaitForSeconds(1f);
        }
        burningVFX.Stop();
    }
    public void PlayBurnEnemy(float burnTime, int damagePerTick)
    {
        StartCoroutine(BurnEnemy(burnTime, damagePerTick));
    }

    IEnumerator SetMeleeHitReactionFalse()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.Rebind();
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

    public void RestartAnimationState(string newState)
    {
        anim.Play(newState);
        currentState = newState;
    }
}
