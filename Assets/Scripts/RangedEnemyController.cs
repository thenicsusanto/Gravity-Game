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
    private int currentHealthEnemy;
    public int maxHealthEnemy = 50;
    public RangedHealthBarEnemy rangedHealthBarEnemy;
    public GameObject coinModel;
    public float coinDropCount;
    public GameObject floatingDamageText;

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
        if (isAttacking == false && (distanceToPlayer > requiredDistanceToPlayer))
        {
            ChangeAnimationState("Spider Walk");
        }

        if (distanceToPlayer <= requiredDistanceToPlayer)
        {
            stopEnemy = true;
            LookAtPlayer();
            if (Time.time - lastAttackTime >= attackCooldown && anim.GetCurrentAnimatorStateInfo(0).IsName("Spider Idle"))
            {
                StartCoroutine(Attack());
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("SpiderHitReaction") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                StartCoroutine(Attack());
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
            StopCoroutine(Attack());
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
        }
        else if (stopEnemy == false && isAttacking == false)
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

    public IEnumerator Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        LookAtPlayer();
        ChangeAnimationState("Spider Attack");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
    }

    public void RangedEnemyTakeDamage(int damage)
    {
        currentHealthEnemy -= damage;
        rangedHealthBarEnemy.SetHealthEnemy(currentHealthEnemy);

        if (currentHealthEnemy != 0)
        {
            ShowFloatingText();
        }

        if (currentHealthEnemy <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        for (int i = 0; i < coinDropCount; i++)
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

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.CrossFadeInFixedTime(newState, 0.1f);
        currentState = newState;
    }
}
