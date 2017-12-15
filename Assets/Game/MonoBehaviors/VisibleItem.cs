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
	[HideInInspector]
	public float timer;
	Vector2 _offset;
	bool grounded;

	MappedMasks Mask;
	public void OnEnable()
	{
		var offset = GetComponent<BoxCollider2D>();
		_offset.x = offset.size.x / 2;
		_offset.y = offset.size.y / 2;
		#region Masks
		Mask = new MappedMasks
		{
			UpLayers = new LayerMask[]
			{
				LayerMask.GetMask("Collideable")
			},
			DownLayers = new LayerMask[]
			{
				LayerMask.GetMask("Collideable"),
				LayerMask.GetMask("Platform")
			}
		}; 
		#endregion
	}

	public void FixedUpdate()
	{
		timer -= Time.deltaTime;
		//if (grounded && (Math.Abs(Force.x) <= 0 || !grounded))
		//	return;
		Force.y += -GameUnity.Gravity * GameUnity.Weight;
		Force.x = Force.x * GameUnity.ForceDamper;
		Force.x = Math.Abs(Force.x) < 0.5f ? 0 : Force.x;

		float yMovement = Force.y * Time.deltaTime;
		float xMovement = Force.x * Time.deltaTime;

		Vector3 tempPos = transform.position;
		bool vertGrounded = false;
		bool horGrounded = false;
		
		tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, _offset.x, _offset.y, Mask, out vertGrounded);
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
		transform.position = new Vector3(tempPos.x, tempPos.y, -0.15f);


	}
	void OnTriggerEnter2D(Collider2D other)
	{
		var idholder = other.GetComponent<IdHolder>();
		if (idholder != null && CallBack != null)
		{
			if (idholder.Owner)
			{
				enabled = false;
				CallBack.Invoke(idholder.ID);
			}
		}
	}
	public IEnumerator TriggerTime()
	{
		float ShakeTime = 0.5f;
		float counter = 0;
		while (true)
		{
			counter += Time.deltaTime;
			if (counter >= ShakeTime)
			{
				GetComponent<BoxCollider2D>().isTrigger = true;
				yield break;
			}
			yield return null;
		}
	}
}
