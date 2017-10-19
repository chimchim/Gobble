using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Game.Actions;
using Game.GEntity;

namespace Game.Movement
{
	public class Swim : MovementState
	{
		public override void EnterState(GameManager game, Component.Movement movement, int entityID, Entity entity)
		{

		}
		public override void Update(GameManager game, Component.Movement movement, int entityID, Entity entity)
		{
			var input = game.Entities.GetComponentOf<Game.Component.Input>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);
			var entityGameObject = entity.gameObject;

			movement.CurrentVelocity.y += GameUnity.WaterGravity + (input.Axis.y * GameUnity.SwimSpeed);
			movement.CurrentVelocity.y = Mathf.Clamp(movement.CurrentVelocity.y, -GameUnity.MaxWaterSpeed, GameUnity.SwimUpExtraSpeed + GameUnity.MaxWaterSpeed);
			movement.CurrentVelocity.x += input.Axis.x * GameUnity.SwimSpeed;
			movement.CurrentVelocity.x = Mathf.Clamp(movement.CurrentVelocity.x, -GameUnity.MaxWaterSpeed, GameUnity.MaxWaterSpeed);


			movement.SwimTime += Time.deltaTime;
			if (movement.SwimTime > GameUnity.LoseOxygenAfter)
			{
				float oxygenDepletionTime = movement.SwimTime - GameUnity.OxygenTime;
				if (((int)oxygenDepletionTime) == movement.OxygenDeplationTick)
				{
					movement.OxygenDeplationTick++;
					AffectHP damage = AffectHP.Make(-GameUnity.OxygenDPS);
					damage.Apply(game, entityID);
					damage.Recycle();
				}
				stats.OxygenSeconds -= Time.deltaTime;
				stats.OxygenSeconds = Mathf.Max(0, stats.OxygenSeconds);
			}
			else
			{
				stats.OxygenSeconds += Time.deltaTime;
				stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);
			}
			float yMovement = movement.CurrentVelocity.y * Time.deltaTime;
			float xMovement = movement.CurrentVelocity.x * Time.deltaTime;

			Vector2 fullmovement = new Vector2(xMovement, yMovement);
			if (yMovement < 0)
			{
				fullmovement = fullmovement.normalized * fullmovement.magnitude * GameUnity.SwimDownMult;
			}
			float xOffset = 0.35f;
			float yOffset = 0.65f;

			bool vertGrounded = false;
			bool horGrounded = false;

			Vector3 tempPos = entityGameObject.transform.position;
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, fullmovement.y, xOffset, yOffset, out vertGrounded);
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, fullmovement.x, xOffset, yOffset, out horGrounded);
			entityGameObject.transform.position = tempPos;

			if (vertGrounded)
			{
				movement.CurrentVelocity.y = 0;
			}
			if (horGrounded)
			{
				movement.CurrentVelocity.x = 0;
			}

			var layerMask = 1 << LayerMask.NameToLayer("Water");
			var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
			RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
			if (hit.collider == null)
			{
				if (yMovement > 0)
				{
					movement.FloatingCounter++;
					movement.CurrentVelocity.y = GameUnity.WaterJumpSpeed / 4f;
					if (input.Axis.y > 0 && movement.FloatingCounter >= GameUnity.FloatJumpEvery)
					{
						movement.FloatingCounter = 0;
						movement.CurrentVelocity.y = GameUnity.WaterJumpSpeed;
					}
				}
				movement.SwimTime = 0;
				movement.OxygenDeplationTick = 0;
				movement.CurrentState = Component.Movement.MoveState.Floating;
				Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
			}
		}
		public override void LeaveState(GameManager game, Component.Movement movement, int entityID, Entity entity)
		{

		}
	}
}
