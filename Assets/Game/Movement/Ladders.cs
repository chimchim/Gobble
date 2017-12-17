using System;
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


			stats.OxygenSeconds += delta;
			stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

			if (movement.Grounded && input.Axis.x != 0)
			{
				animator.SetBool("Run", true);
			}
			if ((input.Space && player.Owner))
			{
				HandleNetEventSystem.AddEventAndHandle(game, entityID, NetJump.Make(entityID));
			}
			movement.CurrentVelocity.y = input.Axis.y * GameUnity.PlayerSpeed;
			float yMovement = movement.CurrentVelocity.y * delta + (movement.ForceVelocity.y * delta);
			float xMovement = 0;

			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;

			bool vertGrounded = false;
			bool horGrounded = false;
			Vector3 tempPos = entityGameObject.transform.position;
			var mask = game.LayerMasks.MappedMasks[movement.CurrentLayer];
			//tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, mask, out vertGrounded);
			entityGameObject.transform.position = tempPos;
			movement.Grounded = vertGrounded;

			if (vertGrounded)
			{
			}
			else
			{
			}

			if (horGrounded)
			{

			}
		}
		public override void LeaveState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
	}
}
