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
	bool isAttacking = false;
	public bool canMove = true;
	public int maxHealth = 100;
	public int currentHealth;

	private Rigidbody rb;
	private Transform playerModel;
	public FixedJoystick joystick;
	public Animator anim;
	private TrailRenderer trailRenderer;
	public HealthBar healthbar;
	public GameHandler gameHandler;

	void Start()
	{
		playerModel = transform.GetChild(0).transform;
		rb = GetComponent<Rigidbody>();
		//anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
		trailRenderer = GetComponent<TrailRenderer>();
		currentHealth = maxHealth;
		healthbar.SetMaxHealth(maxHealth);
		Time.timeScale = 1f;
	}

	void Update()
	{
		moveDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;
		if(moveDirection != Vector3.zero && isAttacking == false && !anim.GetCurrentAnimatorStateInfo(0).IsTag("SwordAttack") && !anim.IsInTransition(0) == true)
		{
			canMove = true;
			RotateForward();
			anim.SetBool("isRunning", true);
		} else if(!anim.IsInTransition(0) == true)
        {
			canMove = false;
			anim.SetBool("isRunning", false);
        }
		

		if (Input.GetMouseButtonDown(1) && isAttacking == false && !anim.IsInTransition(0) == true)
		{
			PlayWrapper();
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

		if(currentHealth <= 0)
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

	public void TakeDamage(int damage)
    {
		currentHealth -= damage;
		healthbar.SetHealth(currentHealth);
    }
}
			
