using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 10f;
	public Vector3 moveDirection;
	public float turnSmoothTime = 0.95f;
	public float dashSpeed = 8f;
	private float nextDashTime = 0;
	public float dashCooldown = 2f;
	bool isDashing = false;
	bool isAttacking = false;
	public bool canMove = true;
	public int maxHealthPlayer = 100;
	public int currentHealthPlayer;
	public float attackRange = 2f;
	public int playerAttackDamage = 25;
	public LayerMask enemyLayers;
	public Transform attackPoint;
	private Rigidbody rb;	
	private Transform playerModel;
	public FixedJoystick joystick;
	public Animator anim;
	private TrailRenderer trailRenderer;
	public HealthBarPlayer healthBarPlayer;
	public GameHandler gameHandler;
	public float viewDistance = 7f;
	RaycastHit closestEnemy;

	void Start()
	{
		playerModel = transform.GetChild(0).transform;
		rb = GetComponent<Rigidbody>();
		//anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
		trailRenderer = GetComponent<TrailRenderer>();
		currentHealthPlayer = maxHealthPlayer;
		healthBarPlayer.SetMaxHealthPlayer(maxHealthPlayer);
		Time.timeScale = 1f;
	}

	void Update()
	{	
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		if(moveDirection != Vector3.zero && isAttacking == false && !anim.GetCurrentAnimatorStateInfo(0).IsTag("SwordAttack") && !anim.IsInTransition(0) == true)
		{
			canMove = true;
			RotateForward();
			anim.SetBool("isRunning", true);
		} else if(!anim.IsInTransition(0) == true)
        {
			anim.SetBool("isRunning", false);
        }
		

		if (Input.GetKeyDown(KeyCode.Space) && isAttacking == false && !anim.IsInTransition(0) == true)
		{
			
			DetectEnemies();
			PlayWrapper();
			//MoveTowardsTarget(closestEnemy.collider.gameObject.GetComponent<EnemyController>(), 1);


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
		rb.AddForce(moveDirection * dashSpeed, ForceMode.Impulse);
		trailRenderer.emitting = true;
		isDashing = false;
		Debug.Log("Dashing");
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
		isAttacking = true;
		rb.velocity = Vector3.zero;
		anim.SetInteger("AttackIndex", UnityEngine.Random.Range(0, 4));
		anim.SetTrigger("Attack");
		Debug.Log("Attacking");
		yield return new WaitForSeconds(.8f);
		isAttacking = false;

	}

	public void PlayWrapper()
    {
		StartCoroutine(PlayRandomAttack());
    }

	public void TakeDamagePlayer(int damage)
    {
		currentHealthPlayer -= damage;
		healthBarPlayer.SetHealthPlayer(currentHealthPlayer);
    }

    void OnDrawGizmosSelected()
    {
		if(attackPoint == null)
        {
			return;
        }
		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

	void MoveTowardsTarget(EnemyController target, float duration)
    {
		transform.DOLookAt(target.transform.position, 0.2f);
		transform.DOMove(target.transform.position, duration);
		Debug.Log("moving");
    }

	void DetectEnemies()
    {
		Ray ray = new Ray(transform.position, moveDirection);
		RaycastHit[] sphereCastHits = Physics.SphereCastAll(ray, 4f, 4, enemyLayers);
		float lastClosestEnemyDistance = 0;
		foreach (RaycastHit hit in sphereCastHits)
        {
			hit.transform.gameObject.SendMessage("EnemyTakeDamage", playerAttackDamage); //sends damage to all enemies hit
			float currentEnemyDistance = Vector3.SqrMagnitude(hit.transform.position - transform.position);
            if (currentEnemyDistance < lastClosestEnemyDistance)
            {
				lastClosestEnemyDistance = currentEnemyDistance;
				closestEnemy = hit;
            }
        }
    }
}
			
