using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 10f;
	public Vector3 moveDirection;
	public float turnSmoothTime = 0.95f;

	public bool isAttacking = false;
	public float attackRange = 2f;

	bool isFreezing = false;
	private float nextFreezeTime = 0;
	public float freezeCooldown = 5f;

	bool isSummoningMeteors = false;
	private float nextSummonTime = 0;
	public float summonCooldown = 5f;

	public bool canMove = true;

	public int maxHealthPlayer = 100;
	public int currentHealthPlayer;

	private Rigidbody rb;
	[SerializeField] private PlayerCombat playerCombat;
	private Transform playerModel;
	public DynamicJoystick joystick;
	public Animator anim;
	private TrailRenderer trailRenderer;
	public HealthBarPlayer healthBarPlayer;
	public GameHandler gameHandler;
	public Collider swordCollider;
	public TrackEnemies trackEnemies;
	public string currentState;
	public int playerWeaponIndex;
	public bool isAttackPressed = false;
	private float horizontal;
	private float vertical;
	public TextMeshProUGUI coinText;
	public bool ifCooldownFreeze = false;
	public bool ifCooldownMeteor = false;

	[SerializeField]
	private AudioSource coinPickupSFX;

	[SerializeField]
	private Image coolDownImage;
	[SerializeField]
	private TextMeshProUGUI textCooldown;

	private float cooldownTimeMeteor = 10.0f;
	private float cooldownTimer = 0.0f;
	private float cooldownTimeFreeze = 8.0f;

	public Button abilityButton;

	void Start()
	{
		playerModel = transform.GetChild(0).transform;
		rb = GetComponent<Rigidbody>();
		trailRenderer = GetComponent<TrailRenderer>();
		currentHealthPlayer = maxHealthPlayer;
		healthBarPlayer.SetMaxHealthPlayer(maxHealthPlayer);
		Time.timeScale = 1f;
		Vector3 randomSpot = UnityEngine.Random.onUnitSphere * 11;
		transform.position = randomSpot;
		swordCollider.enabled = false;
		textCooldown.gameObject.SetActive(false);
		coolDownImage.fillAmount = 0.0f;
		if (FindObjectOfType<WaveSpawner>() == null)
		{
			Debug.LogError("Wavespawner not found");
		}
	}

	void Update()
	{
		if (!isAttacking && !isFreezing && !isSummoningMeteors)
		{
			if (SystemInfo.deviceType == DeviceType.Handheld)
			{
				moveDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;
			}
			else
			{
				moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
			}

		}

		if (currentState != "Attack" && isAttacking) //Failsafe
		{
			isAttacking = false;
			canMove = true;
		}

		if (moveDirection != Vector3.zero && isAttacking == false && isFreezing == false && isSummoningMeteors == false)
		{
			canMove = true;
			RotateForward();
			ChangeAnimationState("Running");

		}
		else if (!isAttacking && !isFreezing && !isSummoningMeteors)
		{
			ChangeAnimationState("Idle");
		}

		//Sword attack input
		if (Input.GetMouseButtonDown(1))
		{
			StartPlayerAttack();
		}

		//Freeze attack input
		if (Input.GetKeyDown(KeyCode.Space) && playerWeaponIndex == 4)
		{
			if (Time.time > nextFreezeTime)
			{
				nextFreezeTime = Time.time + freezeCooldown;
				StartCoroutine(FreezeEnemies());
			}
		}

		//Summon meteors input
		if (Input.GetKeyDown(KeyCode.Space) && playerWeaponIndex == 5)
		{
			if (Time.time > nextSummonTime)
			{
				nextSummonTime = Time.time + summonCooldown;
				StartCoroutine(SummonMeteorsAnim());
			}
		}

		if (currentHealthPlayer <= 0)
		{
			gameHandler.GameOverLoss();
		}

		if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
		{
			swordCollider.enabled = true;
		}

		if (ifCooldownFreeze)
		{
			ApplyCooldown(cooldownTimeFreeze);
		}
		else if (ifCooldownMeteor)
		{
			ApplyCooldown(cooldownTimeMeteor);
		}

	}

	void ApplyCooldown(float coolDownTime)
	{
		cooldownTimer -= Time.deltaTime;
		if (cooldownTimer < 0.0f)
		{
			textCooldown.gameObject.SetActive(false);
			coolDownImage.fillAmount = 0.0f;
		}
		else
		{
			textCooldown.text = Mathf.RoundToInt(cooldownTimer).ToString();
			coolDownImage.fillAmount = cooldownTimer / cooldownTimeMeteor;
		}
	}

	public void PlayAbility()
	{
		if (Time.time > nextFreezeTime && playerWeaponIndex == 4)
		{
			nextFreezeTime = Time.time + freezeCooldown;
			textCooldown.gameObject.SetActive(true);
			ifCooldownFreeze = true;
			cooldownTimer = cooldownTimeFreeze;
			StartCoroutine(FreezeEnemies());
		}
		else if (Time.time > nextSummonTime && playerWeaponIndex == 5)
		{
			nextSummonTime = Time.time + summonCooldown;
			textCooldown.gameObject.SetActive(true);
			ifCooldownMeteor = true;
			cooldownTimer = cooldownTimeMeteor;
			StartCoroutine(SummonMeteorsAnim());
		}
	}
	public void StartPlayerAttack()
	{
		if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f && currentState == "Attack") return;
		canMove = false;
		Debug.Log("can move false");
		isAttacking = true;
		playerCombat.Attack();
		FindObjectOfType<AudioManager>().Play("SwordSlash");
		if (trackEnemies.enemyContact == true)
		{
			MoveTowardsTarget(trackEnemies.closestEnemy);
		}
	}

	void RotateForward()
	{
		Vector3 dir = moveDirection;
		float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
		Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.up); //sets rotation to target angle degrees around y axis
		playerModel.localRotation = targetRotation;
	}

	void FixedUpdate()
	{
		if (canMove == true)
		{
			rb.MovePosition(rb.position + transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);
		}
	}

	public void TakeDamagePlayer(int damage)
	{
		currentHealthPlayer -= damage;
		healthBarPlayer.SetHealthPlayer(currentHealthPlayer);
	}

	public void GainHealthPlayer(int damage)
	{
		currentHealthPlayer += damage;
		if (currentHealthPlayer > 1000)
		{
			currentHealthPlayer = 1000;
		}
		healthBarPlayer.SetHealthPlayer(currentHealthPlayer);

	}

	void MoveTowardsTarget(Transform target)
	{
		Vector3 direction = target.position - playerModel.position;
		Quaternion rotation = Quaternion.LookRotation(direction, transform.TransformDirection(Vector3.up));
		playerModel.rotation = rotation;
	}

	IEnumerator FreezeEnemies()
	{
		canMove = false;
		isFreezing = true;
		rb.velocity = Vector3.zero;
		if (currentState == "Running")
		{
			ChangeAnimationState("Idle");
			yield return new WaitForSeconds(0.1f);
			ChangeAnimationState("PlayerFreezeAttack");
		}
		else
		{
			ChangeAnimationState("PlayerFreezeAttack");
		}
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
		isFreezing = false;
	}

	IEnumerator SummonMeteorsAnim()
	{
		canMove = false;
		isSummoningMeteors = true;
		rb.velocity = Vector3.zero;
		if (currentState == "Running")
		{
			ChangeAnimationState("Idle");
			yield return new WaitForSeconds(0.1f);
			ChangeAnimationState("Player Meteor Attack");
		}
		else
		{
			ChangeAnimationState("Player Meteor Attack");
		}

		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
		isSummoningMeteors = false;
	}

	public void CheckForDestructibles()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, 7f);
		FindObjectOfType<AudioManager>().Play("FreezeExplosionSound");
		foreach (Collider c in colliders)
		{
			if (c.CompareTag("Enemy"))
			{
				c.GetComponent<FreezeEnemy>().PlayFreeze();

			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Coin"))
		{
			GameManager.Instance.coins++;
			coinText.text = GameManager.Instance.coins.ToString();
			coinPickupSFX.Play();
			Destroy(collision.gameObject);
			//shopManager.CheckPurchasable();
		}
	}

	public void ChangeAnimationState(string newState)
	{
		if (currentState == newState) return;

		anim.CrossFade(newState, 0.2f);
		currentState = newState;
	}
}