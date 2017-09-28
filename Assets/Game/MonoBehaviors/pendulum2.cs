using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum2 : MonoBehaviour
{

	// Use this for initialization

	public Transform bob;
	public Transform Stop;
	public float Len;
	Vector2 origin;
	Vector2 bobPosition;
	Vector2 startDir;
	public float angle = 0;
	public float AccConstant;
	public float Damping;
	public float Gravity;
	float aVel = 0.0f;
	float aAcc = 0.0f;
	void Start()
	{
		Len = (bob.transform.position - transform.position).magnitude;
		startDir = (bob.transform.position - transform.position).normalized;
	}

	// Update is called once per frame
	bool passed = false;
	void FixedUpdate()
	{
		aAcc = (-1 * Gravity / Len) * Mathf.Sin(angle) * Time.deltaTime;

	}
}
