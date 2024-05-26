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

    public int attackDamage;
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
    public GameObject bloodVFX;
    private bool isBurning;

    public float burnCooldown;
    public bool isDead = false;
    public GameObject canvas;

    [SerializeField]
    private AudioSource enemyAttackSound;
    [SerializeField]
    private AudioSource enemyHitSound;
    [SerializeField]
    private AudioSource enemyDeathSound;
    [SerializeField]
    private AudioSource enemyBurningSound;

    private bool waitForSoundRunning;

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
            if (GetComponent<Rigidbody>().constraints != RigidbodyConstraints.FreezeAll && !isDead)
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
        } else if(distanceToPlayer > requiredDistanceToPlayer && GetComponent<Rigidbody>().constraints != RigidbodyConstraints.FreezeAll && !isDead)
        {
            LookAtPlayer();
            stopEnemy = false;
        }

        if(recentlyHit == true)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if(pc.GetComponent<PlayerCombat>().attackEnded)
            {
                Debug.Log("Recently hit turned off");
                pc.GetComponent<PlayerCombat>().attackEnded = false;
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

        if(isDead == true)
        {
            swordCollider.enabled = false;
            canvas.SetActive(false);
            gameObject.tag = "Untagged";
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
        if(waitForSoundRunning == false)
        {
            enemyAttackSound.Play();
            StartCoroutine(WaitForSound());
        }      
        LookAtPlayer();
        ChangeAnimationState("Alien Attack");
    }

    IEnumerator WaitForSound()
    {
        waitForSoundRunning = true;
        yield return new WaitForSeconds(3.5f);
        waitForSoundRunning = false;
    }

    public void MeleeEnemyTakeDamage(int damage)
    {
        currentHealthEnemy -= damage;
        meleeHealthBarEnemy.SetHealthEnemy(currentHealthEnemy);

        if(currentHealthEnemy > 0)
        {
            ShowFloatingText();
        }

        if (rb.constraints == RigidbodyConstraints.FreezeAll)
        {
            if(currentHealthEnemy <= 0)
            {
                rb.constraints = RigidbodyConstraints.None;
                StopAllCoroutines();
                StartCoroutine(Die());
            }
        } 
        else if (currentHealthEnemy <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        yield return new WaitForSeconds(0.1f);
        ChangeAnimationState("Alien Death");
        enemyDeathSound.Play();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        for (int i = 0; i < coinDropCount; i++)
        {
            //Change this to just show coin number not actally drop coins because that takes too long
            DropCoin();
        }
        FindObjectOfType<AudioManager>().Play("CoinDropSound");
        GameManager.Instance.enemiesAlive--;
        Destroy(gameObject);
    }
    void DropCoin()
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitCircle;
        GameObject coin = Instantiate(coinModel, transform.position + randomPos + (transform.up * 1.5f), Quaternion.identity);
        Destroy(coin, 10f);
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
            PlayerCombat playerCombat = other.GetComponentInParent<PlayerCombat>();
            if (playerCombat != null && !playerCombat.IsEnemyAlreadyHit(gameObject))
            {
                playerCombat.RegisterHitEnemy(gameObject);
                MeleeEnemyTakeDamage(other.GetComponent<SwordCollider>().damageToTake);
                enemyHitSound.Play();
                GameObject blood = Instantiate(bloodVFX, transform.position, Quaternion.identity);
                Destroy(blood, 0.5f);
                if(currentHealthEnemy <= 0)
                {
                    return;
                }
                recentlyHit = true;
                if (currentState != "Reaction Hit")
                {
                    if(player.GetComponent<PlayerController>().playerWeaponIndex == 3)
                    {
                        if(!isBurning)
                        {
                            StartCoroutine(BurnEnemy(5f, 30));
                        }
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
        isBurning = true;
        float currentTime = Time.time;
        burningVFX.Play();
        enemyBurningSound.Play();
        while (Time.time < currentTime + burnTime)
        {
            MeleeEnemyTakeDamage(damagePerTick);
            yield return new WaitForSeconds(1f);
        }
        burningVFX.Stop();
        enemyBurningSound.Stop();
        isBurning = false;
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
