using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RangedEnemyController : MonoBehaviour
{
    public float runSpeed = 4;
    private Transform target;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private Transform enemyModel;
    private GameObject player;
    bool stopEnemy = false;
    public bool isAttacking = false;
    private float distanceToPlayer;
    public float requiredDistanceToPlayer;
    public string currentState;
    public Animator anim;
    public Transform shotPoint;
    public GameObject projectilePrefab;
    public float lastAttackTime;
    public float attackCooldown = 1;
    public bool recentlyHit = false;

    public int currentHealthEnemy;
    public int maxHealthEnemy = 50;

    public RangedHealthBarEnemy rangedHealthBarEnemy;

    public GameObject coinModel;
    public float coinDropCount;

    public GameObject floatingDamageText;

    public ParticleSystem burningVFX;
    public GameObject bloodVFX;

    public float burnCooldown;
    public bool isDead = false;
    public GameObject canvas;
    private bool isBurning;

    [SerializeField]
    private AudioSource enemyDeathSound;
    [SerializeField]
    private AudioSource enemyBurningSound;

    public int damage;
    private bool onGround = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyModel = transform.GetChild(0).transform;
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealthEnemy = maxHealthEnemy;
        rangedHealthBarEnemy.SetMaxHealthEnemy(maxHealthEnemy);
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
        if (isAttacking == false && (distanceToPlayer > requiredDistanceToPlayer) && onGround)
        {
            ChangeAnimationState("Spider Walk");
        }

        if (distanceToPlayer <= requiredDistanceToPlayer && onGround)
        {
            stopEnemy = true;
            if(GetComponent<Rigidbody>().constraints != RigidbodyConstraints.FreezeAll)
            {
                LookAtPlayer();
            }
            if (Time.time - lastAttackTime >= attackCooldown && anim.GetCurrentAnimatorStateInfo(0).IsName("Spider Idle"))
            {
                Attack();
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("SpiderHitReaction") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                Attack();
            }
            else if (!isAttacking && currentState != "SpiderHitReaction")
            {
                ChangeAnimationState("Spider Idle");
            }
        }
        else if (distanceToPlayer > requiredDistanceToPlayer)
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

        if (isDead == true)
        {
            canvas.SetActive(false);
            gameObject.tag = "Untagged";
        }

        if (isDead && rb.constraints == RigidbodyConstraints.FreezeAll)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    void FixedUpdate()
    {
        if (stopEnemy == true)
        {
            rb.velocity = Vector3.zero;
        }
        else if (stopEnemy == false && isAttacking == false && isDead == false)
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
        lastAttackTime = Time.time;
        LookAtPlayer();
        ChangeAnimationState("Spider Attack");
    }

    public void RangedEnemyTakeDamage(int damage)
    {
        currentHealthEnemy -= damage;
        rangedHealthBarEnemy.SetHealthEnemy(currentHealthEnemy);

        if (currentHealthEnemy > 0)
        {
            ShowFloatingText();
        }

        if (currentHealthEnemy <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        yield return new WaitForSeconds(0.1f);
        enemyDeathSound.Play();
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < coinDropCount; i++)
        {
            //Change this to just show coin number not actally drop coins because that takes too long
            DropCoin();
        }
        FindObjectOfType<AudioManager>().Play("CoinDropSound");
        GameManager.Instance.enemiesAlive--;
        Destroy(gameObject, 1f);
    }

    void DropCoin()
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitCircle;
        GameObject coin = Instantiate(coinModel, transform.position + randomPos + Vector3.up, Quaternion.identity);
        Destroy(coin, 10f);
    }
    void ShowFloatingText()
    {
        GameObject damageText = Instantiate(floatingDamageText, transform.localPosition + (transform.up * 3), Quaternion.identity, transform);
        damageText.GetComponentInChildren<TextMeshPro>().text = currentHealthEnemy.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if(isDead == true)
        {
            return;
        }
        if(other.CompareTag("Sword"))
        {
            if (recentlyHit == false)
            {
                RangedEnemyTakeDamage(other.GetComponent<SwordCollider>().damageToTake);
                GameObject blood = Instantiate(bloodVFX, transform.position, Quaternion.identity);
                Destroy(blood, 0.5f);
                recentlyHit = true;
                if (currentState != "SpiderHitReaction")
                {
                    if (player.GetComponent<PlayerController>().playerWeaponIndex == 3)
                    {
                        if(!isBurning)
                        {
                            StartCoroutine(BurnEnemy(5f, 15));
                        }
                    }
                    if (currentState == "Spider Attack")
                    {
                        isAttacking = false;
                        ChangeAnimationState("SpiderHitReaction");
                    }
                    else
                    {
                        ChangeAnimationState("SpiderHitReaction");
                    }
                }
                else if (currentState == "SpiderHitReaction")
                {
                    RestartAnimationState("SpiderHitReaction");
                    //StartCoroutine(SetRangedHitReactionFalse());
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Planet") == true)
        {
            onGround = true;
        }
    }

    public IEnumerator BurnEnemy(float burnTime, int damagePerTick)
    {
        isBurning = true;
        burningVFX.Play();
        enemyBurningSound.Play();
        float currentTime = Time.time;
        while(Time.time < currentTime + burnTime)
        {
            RangedEnemyTakeDamage(damagePerTick);
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

    IEnumerator SetRangedHitReactionFalse()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.Rebind();
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.CrossFadeInFixedTime(newState, 0.1f);
        currentState = newState;
    }

    public void RestartAnimationState(string newState)
    {
        anim.Play(newState);
        currentState = newState;
    }
}
