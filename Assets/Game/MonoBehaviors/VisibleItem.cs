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

	bool pickable;
	MappedMasks Mask;
	public void OnEnable()
	{
		var box = GetComponent<BoxCollider2D>();
		var capsule = GetComponent<CapsuleCollider2D>();
		if (box)
		{
			_offset.x = box.size.x / 2 * transform.localScale.x;
			_offset.y = box.size.y / 2 * transform.localScale.y;
		}
		else if(capsule)
		{
			_offset.x = capsule.size.x / 2 * transform.localScale.x;
			_offset.y = capsule.size.y / 2 * transform.localScale.y;
		}
		#region Masks
		var prefered = GetComponent<PreferedLayers>();
		if (prefered == null)
		{
			Mask = new MappedMasks
			{
				UpLayers = LayerMask.GetMask("Collideable"),
				DownLayers = LayerMask.GetMask("Collideable") | LayerMask.GetMask("Platform") | LayerMask.GetMask("Treetop")
			};
		}
		else
		{
			Mask = new MappedMasks
			{
				UpLayers = prefered.UpLayers,
				DownLayers = prefered.DownLayers
			};
		}
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
	public void TryPick(int id)
	{
		if (CallBack != null && pickable)
		{
			pickable = false;
			enabled = false;
			CallBack.Invoke(id);
		}
	}
	//void OnTriggerEnter2D(Collider2D other)
	//{
	//	var idholder = other.GetComponent<IdHolder>();
	//	if (idholder != null && CallBack != null && pickable)
	//	{
	//		pickable = false;
	//		enabled = false;
	//		CallBack.Invoke(idholder.ID);
	//	}
	//}
	public IEnumerator TriggerTime()
	{
		float ShakeTime = 0.5f;
		float counter = 0;
		while (true)
		{
			counter += Time.deltaTime;
			if (counter >= ShakeTime)
			{
				pickable = true;
				yield break;
			}
			yield return null;
		}
	}
}
