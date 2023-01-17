using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 10f;
	public Vector3 moveDirection;
	public float turnSmoothTime = 0.95f;

	public float dashSpeed = 8f;
	private float nextDashTime = 0;
	public float dashCooldown = 2f;
	bool isDashing = false;

	public bool isAttacking = false;
	public float attackRange = 2f;
	public int baseAttack = 25;

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
	private Transform playerModel;
	public FixedJoystick joystick;
	public Animator anim;
	private TrailRenderer trailRenderer;
	public HealthBarPlayer healthBarPlayer;
	public GameHandler gameHandler;
	public Collider swordCollider;
	public TrackEnemies trackEnemies;
	public string currentState;
	public int playerWeaponIndex;
	public bool isAttackPressed = false;

	void Start()
	{
		playerModel = transform.GetChild(0).transform;
		rb = GetComponent<Rigidbody>();
		trailRenderer = GetComponent<TrailRenderer>();
		currentHealthPlayer = maxHealthPlayer;
		healthBarPlayer.SetMaxHealthPlayer(maxHealthPlayer);
		Time.timeScale = 1f;
		Vector3 randomSpot = UnityEngine.Random.onUnitSphere * 17;
		transform.position = randomSpot;
		swordCollider.enabled = false;
	}

	void Update()
	{	
		if(!isAttacking && !isFreezing && !isSummoningMeteors)
        {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		}
		if (moveDirection != Vector3.zero && isAttacking == false && isFreezing == false && isSummoningMeteors == false)
		{
			canMove = true;
			RotateForward();
			ChangeAnimationState("Running");
		}
		else if(isAttacking == false && isFreezing == false && isSummoningMeteors == false)
		{
			ChangeAnimationState("Idle");
		}

		//Sword attack input
		if (Input.GetMouseButtonDown(0))
		{
			if (!isAttacking)
			{
				if (trackEnemies.enemyContact == true)
				{
					MoveTowardsTarget(trackEnemies.closestEnemy);
				}
				PlayRandomAttack();
			}
			
			if (isAttacking == true && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
            {
				StartCoroutine(WaitForAttack());				
			}
		}
		
		//Freeze attack input
		if (Time.time > nextFreezeTime)
        {
			if (Input.GetKeyDown(KeyCode.Space) && playerWeaponIndex == 4)
			{
				nextFreezeTime = Time.time + freezeCooldown;
				StartCoroutine(FreezeEnemies());
			}
		}

		//Summon meteors input
		if (Time.time > nextSummonTime)
		{
			if (Input.GetKeyDown(KeyCode.Space) && playerWeaponIndex == 5)
			{
				nextSummonTime = Time.time + summonCooldown;
				StartCoroutine(SummonMeteorsAnim());
			}
		}

		//Dash input
		if (Time.time > nextDashTime)
        {
			if (Input.GetKeyDown(KeyCode.E))
			{
				nextDashTime = Time.time + dashCooldown;
				isDashing = true;
			} else
            {
				StartCoroutine(StopDashing());
            }
		}

		if(currentHealthPlayer <= 0)
        {
			gameHandler.GameOver();
        }

		if(isAttacking)
        {
			if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
			{
				swordCollider.enabled = true;
			}
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
		if(canMove == true)
        {
			rb.MovePosition(rb.position + transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);
		}
		if(isDashing)
        {
			commitDash();
        }
	}

	IEnumerator WaitForAttack()
    {
		isAttacking = false;
		swordCollider.enabled = false;
		yield return new WaitForSeconds(0.1f);
		if (trackEnemies.enemyContact == true)
		{
			MoveTowardsTarget(trackEnemies.closestEnemy);
		}
		PlayRandomAttack();
	}

	void commitDash()
    {
		rb.AddForce(playerModel.forward * dashSpeed, ForceMode.Impulse);
		trailRenderer.emitting = true;
		isDashing = false;
	}

	IEnumerator StopDashing()
    {
		yield return new WaitForSeconds(dashCooldown);
		trailRenderer.emitting = false;
		isDashing = false;
    }

	public void PlayRandomAttack()
	{
		canMove = false;
		//rb.velocity = Vector3.zero;
		isAttacking = true;
		
		float randomAttack = UnityEngine.Random.Range(0, 4);
		if (randomAttack == 0)
		{
			ChangeAnimationState("SwordAttackHorizontal");
		}
		else if (randomAttack == 1)
		{
			ChangeAnimationState("SwordAttackDown");
		}
		else if (randomAttack == 2)
		{
			ChangeAnimationState("SwordAttackBackhand");
		}
		else if (randomAttack == 3)
		{
			ChangeAnimationState("SwordAttack360");
		}
		FindObjectOfType<AudioManager>().Play("SwordSlash");
	}

	public void TakeDamagePlayer(int damage)
    {
		currentHealthPlayer -= damage;
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
		if(currentState == "Running")
        {
			ChangeAnimationState("Idle");
			yield return new WaitForSeconds(0.1f);
			ChangeAnimationState("Player Meteor Attack");
        } else
        {
			ChangeAnimationState("Player Meteor Attack");
		}
		
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
		isSummoningMeteors = false;
	}

	public void CheckForDestructibles()
    {
		Collider[] colliders = Physics.OverlapSphere(transform.position, 4f);
		foreach(Collider c in colliders)
        {
			if(c.CompareTag("Enemy"))
            {
				c.GetComponent<FreezeEnemy>().PlayFreeze();
            }
        }
    }

	public void ChangeAnimationState(string newState)
    {
		if (currentState == newState) return;

		anim.Play(newState);
		currentState = newState;
    }
}

	

