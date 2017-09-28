using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{

	// Use this for initialization

	public Transform bob;
	public Transform Stop;
	public float Len;
	Vector2 origin;
	Vector2 bobPosition;

	public float angle = 0;
	public float AccConstant;
	public float Damping;
	public float Gravity;
	float aVel = 0.0f;
	float aAcc = 0.0f;
	void Start()
	{
		origin = transform.position;
		bobPosition = origin + new Vector2(0, -Len);
		angle = Mathf.PI / 1.5f;
		//aVel = 0.05f;
	}

	// Update is called once per frame
	bool passed = false;
	void FixedUpdate()
	{

		float len = Len;
		Vector2 stopvec = Stop.position - new Vector3(origin.x, origin.y, 0);
		Vector2 currentLineStop = (bobPosition - origin).normalized * stopvec.magnitude;
		Debug.DrawLine(currentLineStop, origin, Color.red);
		if (currentLineStop.x > Stop.position.x && !passed)
		{
			Len = Len - stopvec.magnitude;// (bobPosition - new Vector2(Stop.position.x, Stop.position.y)).magnitude;
										  //Debug.Log("IF " + len);
										  //bobPosition.x = Stop.position.x + (-len * Mathf.Sin(angle));
										  //bobPosition.y = Stop.position.y + (-len * Mathf.Cos(angle));
			passed = true;
			origin = Stop.position;

		}

		bobPosition.x = origin.x + (-Len * Mathf.Sin(angle));
		bobPosition.y = origin.y + (-Len * Mathf.Cos(angle));
		

		bob.position = bobPosition;
		float xAxis = Input.GetAxis("Horizontal");
		float gravity = Gravity;
		if (bob.position.x < origin.x)
		{
			if (xAxis > 0)
			{
				gravity += gravity * 0.5f;
			}
		}
		if (bob.position.x > origin.x)
		{
			if (xAxis < 0)
			{
				gravity += gravity * 0.5f;
			}
		}
		aAcc = (-1 * gravity / len) * Mathf.Sin(angle) * Time.deltaTime;

		aVel += aAcc;
		aVel *= Damping;
		angle += aVel;

	}
}
