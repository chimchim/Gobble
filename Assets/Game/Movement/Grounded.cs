﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Game.Actions;
using Game.GEntity;
using Game.Component;
using Game.Systems;

namespace Game.Movement
{
	public class Grounded : MovementState
	{
		public int PlayerPlatformLayer = LayerMask.NameToLayer("PlayerPlatform");
		public int PlayerLayer = LayerMask.NameToLayer("Player");
		int platformLayer = LayerMask.NameToLayer("Platform");
		public Collider2D JumpLadder;
		public float JumpLadderTimer;
		float groundTimer;
		public override void EnterState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
		public override void Update(GameManager game, MovementComponent movement, int entityID, Entity entity, float delta)
		{

			#region Variables
			movement.CurrentLayer = (int)Systems.Movement.LayerMaskEnum.Grounded;
			var input = game.Entities.GetComponentOf<InputComponent>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);
			var player = game.Entities.GetComponentOf<Game.Component.Player>(entityID);
			var animator = entity.Animator;
			var entityGameObject = entity.gameObject; 
			#endregion

			if (input.Axis.y < 0)
			{
				movement.CurrentLayer = (int)Systems.Movement.LayerMaskEnum.Roped;
			}
			animator.SetBool("Roped", false);
			movement.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
			movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
			
			movement.ForceVelocity.x = Mathf.Clamp(movement.ForceVelocity.x, -15, 15);
			movement.ForceVelocity.y = Mathf.Clamp(movement.ForceVelocity.y, -15, 15);
			movement.ForceVelocity.x = movement.ForceVelocity.x * GameUnity.ForceDamper;
			movement.ForceVelocity.y = movement.ForceVelocity.y * GameUnity.ForceDamper;

			float combinedSpeed = Mathf.Abs(movement.ForceVelocity.x + (input.Axis.x * GameUnity.PlayerSpeed));
			float signedCombinedSpeed = Mathf.Sign(movement.ForceVelocity.x + input.Axis.x * GameUnity.PlayerSpeed);
			float forceXSpeed = Mathf.Abs(movement.ForceVelocity.x);

			if (combinedSpeed > GameUnity.PlayerSpeed && !movement.Grounded)
			{
				movement.ForceVelocity.x = signedCombinedSpeed * forceXSpeed;
			}
			else
			{
				movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed;
			}

			animator.SetBool("Run", (movement.Grounded && input.Axis.x != 0));

			if ((input.Space /*&& (movement.Grounded || groundTimer < 0.2f)*/ && player.Owner))
			{
				HandleNetEventSystem.AddEventAndHandle(game, entityID, NetJump.Make(entityID));
			}
			if (!movement.Grounded)
			{
				groundTimer += delta;
			}
		
			float yMovement = movement.CurrentVelocity.y * delta + (movement.ForceVelocity.y * delta);
			float xMovement = movement.CurrentVelocity.x * delta + (movement.ForceVelocity.x * delta);
			Vector2 tempPos = entityGameObject.transform.position;
			var mask = player.Enemy ? game.LayerMasks.MappedMasksEnemy[movement.CurrentLayer] : game.LayerMasks.MappedMasks[movement.CurrentLayer];
			var capsule =  entityGameObject.GetComponent<CapsuleCollider2D>();
			float yPos = (((capsule.size.y / 2) + (capsule.size.x *1.5f/2)) - Mathf.Abs(yMovement));
			int layer = 0;
			movement.Grounded = Game.Systems.Movement.CheckGrounded(tempPos, yMovement, yPos, mask, out layer);
			entityGameObject.layer = (layer == platformLayer && yMovement < 0) ? PlayerPlatformLayer : PlayerLayer;

			if (movement.Grounded)
			{
				if(yMovement < 0)
					groundTimer = 0;
				movement.CurrentVelocity.y = 0;
			}
			#region LadderCheck
			var ladder1 = VerticalMovementLadder(tempPos, yMovement, capsule.size.x, capsule.size.y);
			if (ladder1/* && (input.Axis.x != 0 || input.Axis.y != 0)*/)
			{
				var skipLadder = (JumpLadderTimer > 0 || ladder1 == JumpLadder);
				if (!skipLadder)
				{
					groundTimer = 0;
					movement.CurrentState = MovementComponent.MoveState.Ladder;
				}
			} 
			#endregion
			JumpLadderTimer -= delta;
			Vector2 newPos = tempPos + new Vector2(xMovement, yMovement);
			movement.Body.MovePosition(newPos);
			animator.SetBool("Jump", !movement.Grounded);
			NetSync(game, player, movement, input, entityID, delta);

		}

		public void NetSync(GameManager game, Player player, MovementComponent movement, InputComponent input, int e, float delta)
		{
			var entity = game.Entities.GetEntity(e);
			var otherTransform = entity.gameObject.transform;
			Vector2 otherPosition = otherTransform.position;
			Vector2 networkPosition = input.NetworkPosition;
			Debug.DrawLine(otherPosition, networkPosition, Color.green);

			Vector2 diff = networkPosition - otherPosition;
			if (!player.Owner && movement.CurrentState != MovementComponent.MoveState.Roped)
			{

				float speed = GameUnity.NetworkLerpSpeed + (GameUnity.NetworkLerpSpeed * (diff.magnitude / GameUnity.NetworkLerpSpeed));
				Vector2 translate = diff.normalized * GameUnity.NetworkLerpSpeed * delta;
				translate = translate.magnitude > diff.magnitude ? diff : translate;
				Vector2 translatePos = otherPosition + translate;
				movement.Body.MovePosition(translatePos);
				Vector2 newPos = otherTransform.position;
				Vector2 newPosDiff = newPos - otherPosition;
				float dot = Vector2.Dot(newPosDiff.normalized, diff.normalized);
				if (dot < 0)
				{
					Debug.Log("SNAP");
					movement.Body.MovePosition(networkPosition);
				}
				if (diff.magnitude > 3)
				{
					movement.Body.MovePosition(networkPosition);
				}
			}
		}
		public static Collider2D VerticalMovementLadder(Vector3 pos, float y, float Xoffset, float yoffset)
		{
			float half = yoffset - (yoffset / 10);
			float fullRayDistance = yoffset + Mathf.Abs(y) - half;
			var layerMask = 1 << LayerMask.NameToLayer("Ladder");

			float sign = Mathf.Sign(y);
			Vector3 firstStartY = pos;
			RaycastHit2D hitsY = new RaycastHit2D();
			hitsY = Physics2D.Raycast(firstStartY, Vector3.up * sign, 0.1f, layerMask);

			//var laddered = ((hitsY.collider != null));
			return hitsY.collider;
		}

		public override void LeaveState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
	}
}
