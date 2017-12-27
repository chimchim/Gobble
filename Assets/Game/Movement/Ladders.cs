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
	public class Ladders : MovementState
	{

		public override void EnterState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
		public override void Update(GameManager game, MovementComponent movement, int entityID, Entity entity, float delta)
		{
			movement.CurrentLayer = (int)Systems.Movement.LayerMaskEnum.Ladder;
			var input = game.Entities.GetComponentOf<InputComponent>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);
			var player = game.Entities.GetComponentOf<Game.Component.Player>(entityID);
			var animator = entity.Animator;
			var entityGameObject = entity.gameObject;

			animator.SetBool("Run", false);
			animator.SetBool("Roped", false);

			movement.ForceVelocity = Vector2.zero;
			//float sign = input.Axis.y == 0 ? 0 : Mathf.Sign(input.Axis.y);
			//float inputY = Math.Max(Mathf.Abs(input.Axis.y), 0.6f) * sign;
			movement.CurrentVelocity.y = input.Axis.y * GameUnity.PlayerSpeed * 1.3f;
			movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed;
			float yMovement = movement.CurrentVelocity.y * delta;
			float xMovement = movement.CurrentVelocity.x * delta;

			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;

			bool vertGrounded = false;
			bool horGrounded = false;
			Vector3 tempPos = entityGameObject.transform.position;

			var mask = game.LayerMasks.MappedMasks[movement.CurrentLayer];
			var tempPos1 = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			tempPos1 = Game.Systems.Movement.VerticalMovement(tempPos1, yMovement, xOffset, yOffset, mask, out vertGrounded);
			entityGameObject.transform.position = tempPos1;

			if (yMovement == 0)
				yMovement = -0.1f;
			var ladder1 = Grounded.VerticalMovementLadder(tempPos, yMovement, xOffset, yOffset);
			if (!ladder1)
			{
				if (Math.Abs(input.Axis.x) <= 0 && input.Axis.y > 0)
				{
					if ((input.Space && player.Owner))
					{
						movement.CurrentState = MovementComponent.MoveState.Grounded;
						var grounded = movement.States[(int)MovementComponent.MoveState.Grounded] as Grounded;
						grounded.JumpLadder = ladder1;
						grounded.JumpLadderTimer = 0.15f;
						HandleNetEventSystem.AddEventAndHandle(game, entityID, NetJump.Make(entityID));
					}

					entityGameObject.transform.position = tempPos;
					return;
				}
				if (input.Axis.y > 0)
				{
					movement.CurrentVelocity.y = Mathf.Sign(input.Axis.y) * GameUnity.PlayerSpeed * 1.3f;
				}
				movement.CurrentState = MovementComponent.MoveState.Grounded;
				return;
			}
			if ((input.Space && player.Owner))
			{
				movement.CurrentState = MovementComponent.MoveState.Grounded;
				var grounded = movement.States[(int)MovementComponent.MoveState.Grounded] as Grounded;
				grounded.JumpLadder = ladder1;
				grounded.JumpLadderTimer = 0.15f;
				HandleNetEventSystem.AddEventAndHandle(game, entityID, NetJump.Make(entityID));
			}
		}
		public override void LeaveState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
	}
}
