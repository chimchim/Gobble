using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rigid : MonoBehaviour {

	// Use this for initialization
	Vector2 CurrentVelocity;
	Rigidbody2D body;
	public float Speed;
	public float Gravity;
	public float Jump;
	void Start () {
		body = GetComponent<Rigidbody2D>();
		//body.MovePosition(new Vector2(3.2f, 0.3f));
		//Debug.Log("trans " + transform.position);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float x = UnityEngine.Input.GetAxis("Horizontal");
		float y = UnityEngine.Input.GetAxis("Vertical");
		CurrentVelocity.y += -Gravity;
		CurrentVelocity.x = x * Speed;

		if (Input.GetKeyDown(KeyCode.Space))
			CurrentVelocity.y = Jump;

		float yMovement = CurrentVelocity.y * Time.fixedDeltaTime;
		float xMovement = CurrentVelocity.x * Time.fixedDeltaTime;
		Vector2 myPos = transform.position;
		var layerMask = 1 << LayerMask.NameToLayer("Collideable");
		Debug.DrawLine(transform.position, transform.position + (new Vector3(0, yMovement)), Color.red);
		var hit = Physics2D.CapsuleCast(transform.position, new Vector2(0.4f, 1), CapsuleDirection2D.Vertical, 0, new Vector2(0, yMovement).normalized, Mathf.Abs(yMovement), layerMask);
		if (hit.transform != null)
		{
			CurrentVelocity.y = 0;
		}
		else
		{
			Debug.Log("Missed " + CurrentVelocity.y);
		}
		//.velocity = new Vector3(xMovement, yMovement);
		Vector2 newPos = transform.position + new Vector3(xMovement, yMovement);
		body.MovePosition(newPos);

	}
}
