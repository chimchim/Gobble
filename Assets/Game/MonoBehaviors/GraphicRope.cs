﻿using Game;
using Game.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicRope : MonoBehaviour {

	public Rope RopeItem;
	// Player
	Transform PlayerTransform;
	MovementComponent PlayerMovement;
	InputComponent PlayerInput;
	// Draw
	public int drawIndex = 0;
	List<Transform> Ropes;
	Transform FrontRope;
	public bool UpdateFront;

	//Pos
	Vector2 NewRopePosition;
	Vector2 NewRopeDirection;
	public bool UpdateNewRope;
	public bool Owner;
	Vector2 CurrentVelocity;
	Vector2[] DrawThrowPositions = new Vector2[2];
	GameObject RopeFab;

	void Start ()
	{
		UpdateFront = true;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		//UpdateRope();
	}
	public void DeActivate()
	{
		RopeItem = null;
		for (int i = 0; i < Ropes.Count; i++)
		{
			Ropes[i].position = Vector3.zero;
			Ropes[i].localScale = new Vector3(1, 1, 1);
		}
		UpdateFront = true;
		FrontRope.position = Vector3.zero;
		FrontRope.localScale = new Vector3(1, 1, 1);

	}			 
	public void MakeRopes()
	{
		var ropeParent = new GameObject();
		ropeParent.name = "RopeParent";
		List<Transform> ropes = new List<Transform>();
		FrontRope = (GameObject.Instantiate(UnityEngine.Resources.Load("Prefabs/RopeFront", typeof(GameObject))) as GameObject).transform;
		RopeFab = GameObject.Instantiate(UnityEngine.Resources.Load("Prefabs/Rope", typeof(GameObject))) as GameObject;
		ropes.Add(RopeFab.transform);

		for (int i = 0; i < 60; i++)
		{
			var newRope = GameObject.Instantiate(RopeFab);
			newRope.transform.parent = ropeParent.transform;
			newRope.SetActive(true);
			ropes.Add(newRope.transform);
		}
		Ropes = ropes;
	}

	//public 
	public void UpdateRope()
	{
		if (UpdateNewRope)
		{
			Vector2 currentMove = CurrentVelocity * Time.deltaTime;
			CurrentVelocity.y += -GameUnity.Gravity;

			var layerMask = 1 << LayerMask.NameToLayer("Collideable") | 1 << LayerMask.NameToLayer("Treetop");
			Vector2 playerPos = new Vector2(PlayerTransform.position.x, PlayerTransform.position.y);
			Vector2 currentL = (NewRopePosition - playerPos);

			RaycastHit2D hit = Physics2D.Raycast(playerPos, currentL.normalized, (currentMove.magnitude + currentL.magnitude), layerMask);
			if (hit.collider != null)
			{
				float ropeL = (playerPos - hit.point).magnitude;
				if (ropeL > 0)
				{
					if (Owner)
					{
						PlayerMovement.CurrentState = MovementComponent.MoveState.Roped;
						PlayerMovement.CurrentRoped = new MovementComponent.RopedData()
						{
							RayCastOrigin = ((0.05f * hit.normal) + hit.point),
							origin = hit.point,
							Length = ropeL,
							Damp = GameUnity.RopeDamping
						};

						PlayerInput.RopeConnected = new InputComponent.NetworkRopeConnected()
						{
							RayCastOrigin = ((0.05f * hit.normal) + hit.point),
							Position = playerPos,
							Origin = hit.point,
							Length = ropeL
						};
						PlayerMovement.RopeList.Add(PlayerMovement.CurrentRoped);
					}
					UpdateNewRope = false;
					UpdateFront = true;

					return;
				}
			}
			NewRopePosition += currentMove;
			DrawThrowPositions[0] = NewRopePosition;
			DrawThrowPositions[1] = playerPos;
			UpdateFront = true;
			DrawRope(DrawThrowPositions, playerPos, 0);
		}
	}
	public void ThrowRope(GameManager game, int entity, MovementComponent movement, InputComponent input)
	{
		DeActivate();
		PlayerInput = input;
		PlayerTransform  = game.Entities.GetEntity(entity).gameObject.transform;
		PlayerMovement = movement;
		Vector2 direction = input.MousePos - new Vector2(PlayerTransform.position.x, PlayerTransform.position.y);
		NewRopePosition = PlayerTransform.position;
		NewRopeDirection = direction.normalized;
		UpdateNewRope = true;
		CurrentVelocity = NewRopeDirection * GameUnity.RopeThrowStartSpeed;

	}
	public void DrawRope(Vector2[] drawPositions, Vector2 playerPos, int ropeIndex)
	{
		if (UpdateFront)
		{
			Vector2 first = drawPositions[0];
			Vector2 second = drawPositions[1];
			Vector2 direction = (second - first).normalized;
			FrontRope.position = first +(-direction*0.2f);
			FrontRope.right = direction;
			UpdateFront = false;
		}
		
		drawIndex = 0;


		//drawPositions
		
		for (int i = 0; i < drawPositions.Length - 1; i++)
		{
			Vector2 first = drawPositions[i];
			Vector2 second = drawPositions[i + 1];
			//Debug.DrawLine(first, second, Color.blue);
			Vector2 direction = (second - first).normalized;
			float len = (second - first).magnitude;
			int ropeAmount = (int)(len / 0.51f);
			float extra = len - (0.51f * ropeAmount);
			Vector2 nextPos = first;

			for (int j = 0; j < ropeAmount; j++)
			{
				if (Ropes.Count < (drawIndex + 1))
				{
					var newRope = GameObject.Instantiate(RopeFab);
					newRope.SetActive(true);
					Ropes.Add(newRope.transform);
				}
				Ropes[drawIndex].name = second.ToString();
				Ropes[drawIndex].position = first + (direction * 0.51f * j) + (direction * 0.255f);
				nextPos = first + (direction * 0.51f * j);
				Ropes[drawIndex].localScale = new Vector3(1, 1, 1);
				Ropes[drawIndex].right = direction;
				drawIndex++;
			}
			if (Ropes.Count < (drawIndex + 1))
			{
				var newRope = GameObject.Instantiate(RopeFab);
				newRope.SetActive(true);
				Ropes.Add(newRope.transform);
			}
			var pos = first +(direction * 0.51f * (ropeAmount - 1)) + (direction * 0.255f) + (direction * 0.255f) + (direction * extra / 2);
			if (float.IsNaN(pos.x))
			{
				Debug.LogError("SHIT IS NAN");
				continue;
			}
			Ropes[drawIndex].position = pos;
			Ropes[drawIndex].name = "jerry";
			Ropes[drawIndex].localScale = new Vector3(extra / 0.51f, Ropes[drawIndex].localScale.y, Ropes[drawIndex].localScale.z);
			Ropes[drawIndex].right = direction;
			drawIndex++;
		}
		for (int j = drawIndex; j < Ropes.Count; j++)
		{ 
			Ropes[j].position = Vector3.zero;
		
		}
	}
}
