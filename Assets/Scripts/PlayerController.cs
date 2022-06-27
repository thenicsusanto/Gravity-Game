using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 10f;
	public Vector3 moveDirection;
	private float turnSmoothVelocity;
	public float turnSmoothTime = 0.95f;
	public float dashSpeed = 8f;
	bool isDashing;

	private Rigidbody rb;
	private Transform playerModel;
	public FixedJoystick joystick;
	public Animator anim;

	void Start()
	{
		playerModel = transform.GetChild(0).transform;
		rb = GetComponent<Rigidbody>();
		anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
	}

	void Update()
	{
		
		moveDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;
		RotateForward();
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			isDashing = true;
		}
	}

    void RotateForward()
    {
		Vector3 dir = moveDirection;
		// calculate angle and rotation
		float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
		Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.up); //sets rotation to target angle degrees around y axis
		// only update rotation if direction greater than zero
		if (Vector3.Magnitude(dir) > 0.1f)
		{
			playerModel.localRotation = targetRotation;
			anim.SetBool("isRunning", true);
		} else
        {
			anim.SetBool("isRunning", false);
        }
	}

    void FixedUpdate()
	{
		rb.MovePosition(rb.position + transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);
		if (isDashing)
		{
			commitDash();
		}
	}

	void commitDash()
	{
		rb.AddForce(moveDirection * dashSpeed, ForceMode.Impulse);
		isDashing = false;
		Debug.Log("Dashing");
	}
}
