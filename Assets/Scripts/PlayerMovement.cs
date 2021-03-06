﻿using UnityEngine;
using System.Collections;
using InControl;

public enum Direction {Right, Up, Left, Down};

public class PlayerMovement : MonoBehaviour {
	public float speed;
	public float reticleRadius;
	public float knockbackTime;
    public float slowFactor;
    public bool locked;
    public bool dashing;
    public int bufferIter;
	public Direction lastDirection;

	public Reticle reticle;

	private int direction = 8;

	private float radians;
	private float degrees;

	private string horizontalAxis;
	private string verticalAxis;

	private InputDevice controller;

	private Rigidbody2D rb2D;

	private Animator anim;

	private PlayerStats playerStats;
	private new SpriteRenderer renderer;

	private Vector2 knockbackVector;

	void Awake() {
		locked = false;
	}

	void Start() {
		playerStats = GetComponent<PlayerStats>();
		gameObject.layer = playerStats.number + 8;
		bufferIter = 0;

		knockbackVector = Vector2.zero;

		reticle.gameObject.layer = gameObject.layer;
		rb2D = GetComponent<Rigidbody2D>();
		anim.speed = 1f;
		if(playerStats.number == 0) {
			radians = 0.0f;
			lastDirection = Direction.Right;
		} else if(playerStats.number == 1) {
			radians = Mathf.PI;
			lastDirection = Direction.Left;
		}
		degrees = radians * Mathf.Rad2Deg;
		SetReticle();
		renderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		HandleMovement();
		lastDirection = (Direction)(int)((degrees + 45) / 90);
	}

	void HandleMovement() {
        if (dashing)
        {

        }
		else if(!locked) {
			if(knockbackVector != Vector2.zero) {
				SetReticle();
				return;
			}
			else {
				rb2D.velocity = GetInput();
				anim.SetInteger("xAxis", (int) rb2D.velocity.x);
				anim.SetInteger("yAxis", (int) rb2D.velocity.y);
				if(rb2D.velocity.x < 0) {
					renderer.flipX = true;
				}
				else if(rb2D.velocity.x > 0) {
					renderer.flipX = false;
				}
				if(rb2D.velocity.x != 0.0f || rb2D.velocity.y != 0.0f) {
					radians = Mathf.Atan2(rb2D.velocity.y, rb2D.velocity.x);
					degrees = radians * Mathf.Rad2Deg;
					if(degrees < 0.0f) {
						degrees += 360.0f;
						}
					SetReticle();
					}
				}
			}
		else {
			rb2D.velocity = Vector2.zero;
		}
	}

	Vector2 GetInput() {
		float computedSpeed =  speed * (float)(1 - slowFactor * bufferIter);
		if(Application.isEditor) {
			return new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis)).normalized * (float)computedSpeed;
		}
		else {
			return new Vector2(controller.DPad.X, controller.DPad.Y).normalized * (float)computedSpeed;
		}
	}

	public float CurrentShotAngle() {
			return radians * Mathf.Rad2Deg;
	}

	public void InitializeAxes(string[] controls) {
		horizontalAxis = controls[0];
		verticalAxis = controls[1];
	}

	public void InitializeController(InputDevice newController) {
		controller = newController;
	}

	public void SetReticle() {
		reticle.GetRigidbody2D().MovePosition((Vector2)transform.position + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * reticleRadius);
		reticle.GetRigidbody2D().MoveRotation(degrees - 90.0f);
	}

	public void SetAnimator(AnimatorOverrideController value) {
		anim = GetComponent<Animator>();
		anim.runtimeAnimatorController = value;
	}

	public Rigidbody2D GetRigidbody2D() {
		return rb2D;
	}

	public void StartKnockback(Transform obj, float damage) {
		Vector3 translation = transform.position - obj.position;
		knockbackVector = translation;
		knockbackVector.Normalize();
		knockbackVector.x *= 20;
		knockbackVector.y *= 20;
		playerStats.TakeDamage(damage);
		StartCoroutine(Knockback());
	}

	IEnumerator Knockback() {
		float t = 0;
		while(t < knockbackTime) {
			rb2D.velocity = knockbackVector;
			t += Time.fixedDeltaTime;
			yield return null;
		}
		knockbackVector = Vector2.zero;
	}
}
