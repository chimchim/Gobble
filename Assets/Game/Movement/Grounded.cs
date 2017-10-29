﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Game.Actions;
using Game.GEntity;
using Game.Component;

namespace Game.Movement
{
	public class Grounded : MovementState
	{
		public override void EnterState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
		public override void Update(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{
			var input = game.Entities.GetComponentOf<InputComponent>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);
			var player = game.Entities.GetComponentOf<Game.Component.Player>(entityID);
			var animator = entity.Animator;
			var entityGameObject = entity.gameObject;

			animator.SetBool("Run", false);
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

			stats.OxygenSeconds += Time.deltaTime;
			stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

			if (movement.Grounded && input.Axis.x != 0)
			{
				animator.SetBool("Run", true);
			}
			if ((input.Space && movement.Grounded && player.Owner) || input.NetworkJump)
			{
				Game.Systems.Movement.DoJump(game, player.EntityID);
			}

			float yMovement = movement.CurrentVelocity.y * Time.deltaTime + (movement.ForceVelocity.y * Time.deltaTime);
			float xMovement = movement.CurrentVelocity.x * Time.deltaTime + (movement.ForceVelocity.x * Time.deltaTime);

			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;

			bool vertGrounded = false;
			bool horGrounded = false;

			Vector3 tempPos = entityGameObject.transform.position;
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, out vertGrounded);
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			entityGameObject.transform.position = tempPos;
			movement.Grounded = vertGrounded;
			if (vertGrounded)
			{
				animator.SetBool("Jump", false);
				if (movement.FallingTime > GameUnity.ExtraFallSpeedAfter)
				{
					float fallMulti = movement.FallingTime - GameUnity.ExtraFallSpeedAfter;
					AffectHP fallDamage = AffectHP.Make(-GameUnity.FallDamage * fallMulti);
					fallDamage.Apply(game, entityID);
					fallDamage.Recycle();
				}
				movement.ForceVelocity.y = 0;
				if (yMovement < 0)
				{
					movement.ForceVelocity.x *= 0.88f;
				}
				movement.FallingTime = 0;
				movement.CurrentVelocity.y = 0;
			}
			else
			{
				animator.SetBool("Jump", true);
				if (movement.CurrentVelocity.y < 0)
				{
					movement.FallingTime += Time.deltaTime;
				}
			}

			if(horGrounded)
			{
				movement.ForceVelocity.x = 0;
			}
			var layerMask = 1 << LayerMask.NameToLayer("Water");
			var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
			RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
			if (hit.collider != null)
			{
				movement.FallingTime = 0;
				movement.CurrentState = MovementComponent.MoveState.Swimming;
				Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
			}

		}
		public override void LeaveState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
	}
}
