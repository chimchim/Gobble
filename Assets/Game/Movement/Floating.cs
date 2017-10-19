using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Game.GEntity;

namespace Game.Movement
{
	public class Floating : MovementState
	{
		public override void EnterState(GameManager game, Component.Movement movement, int entityID, Entity entity)
		{

		}
		public override void Update(GameManager game, Component.Movement movement, int entityID, Entity entity)
		{
			var input = game.Entities.GetComponentOf<Game.Component.Input>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);
			var entityGameObject = entity.gameObject;

			movement.CurrentVelocity.y = movement.CurrentVelocity.y - GameUnity.Gravity;
			movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
			movement.CurrentVelocity.x += input.Axis.x * GameUnity.SwimSpeed;
			movement.CurrentVelocity.x = Mathf.Clamp(movement.CurrentVelocity.x, -GameUnity.MaxWaterSpeed, GameUnity.MaxWaterSpeed);

			stats.OxygenSeconds += Time.deltaTime;
			stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

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

			if (vertGrounded)
			{
				if (movement.CurrentVelocity.y < 0)
				{
					movement.CurrentState = Component.Movement.MoveState.Grounded;
				}
				movement.CurrentVelocity.y = 0;
			}

			var layerMask = 1 << LayerMask.NameToLayer("Water");
			var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
			RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
			if (hit.collider != null)
			{
				movement.FloatJump = true;
				movement.CurrentState = Component.Movement.MoveState.Swimming;
				Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
			}
		}
		public override void LeaveState(GameManager game, Component.Movement movement, int entityID, Entity entity)
		{

		}
	}
}
