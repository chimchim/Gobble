using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleItem : MonoBehaviour
{
	public Action<int> CallBack;
	[HideInInspector]
	public Item Item;
	[HideInInspector]
	public Vector2 Force;

	Vector2 _offset;
	bool grounded;

	public void OnEnable()
	{
		var offset = GetComponent<BoxCollider2D>();
		_offset.x = offset.size.x / 2;
		_offset.y = offset.size.y / 2;
	}

	public void FixedUpdate()
	{
		if (grounded && (Math.Abs(Force.x) <= 0 || !grounded))
			return;
		Force.y += -GameUnity.Gravity * GameUnity.Weight;
		Force.x = Force.x * GameUnity.ForceDamper;
		Force.x = Math.Abs(Force.x) < 0.5f ? 0 : Force.x;

		float yMovement = Force.y * Time.deltaTime;
		float xMovement = Force.x * Time.deltaTime;

		Vector3 tempPos = transform.position;
		bool vertGrounded = false;
		bool horGrounded = false;
		tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, _offset.x, _offset.y, out vertGrounded);
		tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, _offset.x, _offset.y, out horGrounded);
		if (vertGrounded)
		{
			grounded = true;
			Force.y = 0;
			Force.x *= 0.95f;
		}
		if (horGrounded)
		{
			Force.x = 0;
		}
		transform.position = tempPos;


	}
	void OnTriggerEnter2D(Collider2D other)
	{
		var idholder = other.GetComponent<IdHolder>();
		if (idholder != null)
		{
			if (idholder.Owner)
			{
				enabled = false;
				CallBack.Invoke(idholder.ID);
			}
		}
	}
}
