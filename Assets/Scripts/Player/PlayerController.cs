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
	public bool canMove = true;
	public int maxHealthPlayer = 100;
	public int currentHealthPlayer;
	public float attackRange = 2f;
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
	public int baseAttack = 25;

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
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		if (moveDirection != Vector3.zero && isAttacking == false)
		{
			canMove = true;
			RotateForward();
			ChangeAnimationState("Running");
		}
		else if(isAttacking == false)
		{
			ChangeAnimationState("Idle");
		}

		if (Input.GetMouseButtonDown(0))
		{
			if(!isAttacking)
            {
				if (trackEnemies.enemyContact == true)
				{
					MoveTowardsTarget(trackEnemies.closestEnemy);
				}
				StartCoroutine(PlayRandomAttack());
			}
		}

		if(Input.GetKeyDown(KeyCode.Space))
        {
			if(playerWeaponIndex == 4)
            {
				anim.Play("PlayerFreezeAttack");
			}
        }

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
	}

    void RotateForward()
    {
		Vector3 dir = moveDirection;
		// calculate angle and rotation
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

	public IEnumerator PlayRandomAttack()
	{
		canMove = false;
		rb.velocity = Vector3.zero;
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
		yield return new WaitForEndOfFrame();
		swordCollider.enabled = true;
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
		swordCollider.enabled = false;
		yield return new WaitForEndOfFrame();
		isAttacking = false;
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

	void ChangeAnimationState(string newState)
    {
		if (currentState == newState) return;

		anim.Play(newState);
		currentState = newState;
    }
}

	

