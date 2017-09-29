using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Game.Actions;

namespace Game.Movement
{
	public class Grounded : MovementState
	{
		public override void EnterState(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{

		}
		public override void Update(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{
			var input = game.Entities.GetComponentOf<Game.Component.Input>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);

			movement.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
			movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
			movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed;

			stats.OxygenSeconds += Time.deltaTime;
			stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

			if (input.Space && movement.Grounded)
			{
				movement.CurrentVelocity.y = GameUnity.JumpSpeed;
			}

			float yMovement = movement.CurrentVelocity.y * Time.deltaTime;
			float xMovement = movement.CurrentVelocity.x * Time.deltaTime;

			float xOffset = 0.35f;
			float yOffset = 0.65f;

			bool vertGrounded = false;
			bool horGrounded = false;

			Vector3 tempPos = entityGameObject.transform.position;
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, out vertGrounded);
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			entityGameObject.transform.position = tempPos;
			movement.Grounded = vertGrounded;
			if (vertGrounded)
			{
				if (movement.FallingTime > GameUnity.ExtraFallSpeedAfter)
				{
					float fallMulti = movement.FallingTime - GameUnity.ExtraFallSpeedAfter;
					AffectHP fallDamage = AffectHP.Make(-GameUnity.FallDamage * fallMulti);
					fallDamage.Apply(game, entityID);
					fallDamage.Recycle();
				}
				movement.FallingTime = 0;
				movement.CurrentVelocity.y = 0;
			}
			else
			{
				if (movement.CurrentVelocity.y < 0)
				{
					movement.FallingTime += Time.deltaTime;
				}
			}
			var layerMask = 1 << LayerMask.NameToLayer("Water");
			var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
			RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
			if (hit.collider != null)
			{
				movement.FallingTime = 0;
				movement.CurrentState = Component.Movement.MoveState.Swimming;
				Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
			}

		}
		public override void LeaveState(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{

		}
	}
}
