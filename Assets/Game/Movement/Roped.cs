using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;

namespace Game.Movement
{
	public class Roped : MovementState
	{
		public override void EnterState(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{

		}
		public override void Update(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{
			var input = game.Entities.GetComponentOf<Game.Component.Input>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);

			Vector2 playerPos = entityGameObject.transform.position;
			Vector2 currentTranslate = (movement.CurrentVelocity * Time.deltaTime) + (movement.ForceVelocity * Time.deltaTime);
			Vector2 playerPosFirstMove = playerPos + currentTranslate;
			Vector2 origin = movement.CurrentRoped.origin;

			float len = movement.CurrentRoped.Length;
			float diff = (playerPosFirstMove - origin).magnitude;
			float vel = movement.CurrentRoped.Vel;
			float angle = movement.CurrentRoped.Angle;
			float aAcc = 0;
			Debug.DrawLine(playerPos, origin, Color.red);
			float yMovement = 0;
			float xMovement = 0;
			float lastAngle = 0;
			if (diff > len || movement.CurrentRoped.FirstAngle || (currentTranslate.y < 0 && playerPos.y < origin.y))
			{
				float deltaTimeMult = 1 / Time.deltaTime;
				#region First Angle
				if (!movement.CurrentRoped.FirstAngle)
				{
					movement.CurrentRoped.Length = (playerPos - origin).magnitude;
					len = movement.CurrentRoped.Length;
					float angle2 = Vector2.Angle((origin - playerPos).normalized, (-Vector2.up));
					if (origin.x > playerPos.x)
					{
						angle = (Mathf.PI / 180f) * (180 - angle2);
					}
					else
					{
						angle = (Mathf.PI / 180f) * -(180 - angle2);
					}
					movement.CurrentRoped.Angle = angle;
					movement.CurrentRoped.FirstAngle = true;
					movement.CurrentRoped.Vel = 1;

					float sinned = Mathf.Abs(Mathf.Sin(angle));
					float cosed = Mathf.Abs(Mathf.Cos(angle));
					Vector2 combinedVelocity = movement.CurrentVelocity + movement.ForceVelocity;
					float newXVel = combinedVelocity.x * cosed;
					float newYVel = combinedVelocity.y * sinned;

					float velXDir = newXVel * -Mathf.Sign(Mathf.Cos(angle));
					float velYDir = newYVel * Mathf.Sign(Mathf.Sin(angle));

					float newSpeed = velXDir + velYDir;
					float ropeDirection = Mathf.Sign(newSpeed);

					float tempAngle = movement.CurrentRoped.Vel + angle;
					playerPos.x = origin.x + (-len * Mathf.Sin(tempAngle));
					playerPos.y = origin.y + (-len * Mathf.Cos(tempAngle));

					Vector2 ropeMovePos = playerPos - new Vector2(entityGameObject.transform.position.x, entityGameObject.transform.position.y);
					float ropeSpeed = (ropeMovePos.magnitude) * deltaTimeMult;
					float velDivider = movement.CurrentRoped.Vel / ropeSpeed;
					float newVel = Mathf.Abs(velDivider) * Mathf.Abs(newSpeed) * ropeDirection; // 6 = New ropeSpeed
					movement.CurrentRoped.Vel = newVel;

					movement.CurrentVelocity = Vector2.zero;
				}
				#endregion
				playerPos.x = origin.x + (-len * Mathf.Sin(angle));
				playerPos.y = origin.y + (-len * Mathf.Cos(angle));
				lastAngle = angle - movement.CurrentRoped.Vel;

				xMovement = playerPos.x - entityGameObject.transform.position.x;
				yMovement = playerPos.y - entityGameObject.transform.position.y;

				movement.CurrentVelocity.y = deltaTimeMult * yMovement;
				movement.ForceVelocity = (deltaTimeMult * new Vector2(xMovement, 0));

				float gravity = GameUnity.RopeGravity;
				#region Rope Input
				if (playerPos.x < origin.x)
				{
					if (input.Axis.x > 0)
					{
						gravity += gravity * GameUnity.RopeSpeedMult;
					}
				}
				if (playerPos.x > origin.x)
				{
					if (input.Axis.x < 0)
					{
						gravity += gravity * GameUnity.RopeSpeedMult;
					}
				}
				#endregion
				float gravityDivier = Math.Max(1, len);
				var collided = CheckRopeCollision(playerPos, movement);
				if (!collided)
				{
					aAcc = (-1 * gravity / gravityDivier) * Mathf.Sin(angle) * Time.deltaTime;
					movement.CurrentRoped.Vel += aAcc;
					movement.CurrentRoped.Vel *= movement.CurrentRoped.Damp;
					movement.CurrentRoped.Angle += movement.CurrentRoped.Vel;
				}
				else
				{

				}
			}
			else
			{
				movement.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
				movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
				movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed;

				movement.ForceVelocity.x = Mathf.Clamp(movement.ForceVelocity.x, -15, 15);
				movement.ForceVelocity.y = Mathf.Clamp(movement.ForceVelocity.y, -15, 15);
				movement.ForceVelocity.x = movement.ForceVelocity.x * GameUnity.ForceDamper;
				movement.ForceVelocity.y = movement.ForceVelocity.y * GameUnity.ForceDamper;

				yMovement = movement.CurrentVelocity.y * Time.deltaTime + (movement.ForceVelocity.y * Time.deltaTime);
				xMovement = movement.CurrentVelocity.x * Time.deltaTime + (movement.ForceVelocity.x * Time.deltaTime);
			}
			stats.OxygenSeconds += Time.deltaTime;
			stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

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
				if (yMovement > 0)
				{
					movement.CurrentRoped.Angle = lastAngle;
					movement.CurrentRoped.Vel = -movement.CurrentRoped.Vel * 0.3f;
				}
				else
				{
					movement.CurrentState = Component.Movement.MoveState.Grounded;
				}
			}
			if (horGrounded)
			{

				movement.CurrentRoped.Angle = lastAngle;
				movement.CurrentRoped.Vel = -movement.CurrentRoped.Vel * 0.3f;
			}

			movement.Grounded = vertGrounded;
			movement.FallingTime = 0;
			var layerMask = 1 << LayerMask.NameToLayer("Water");
			var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
			RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
			if (hit.collider != null)
			{
				movement.CurrentState = Component.Movement.MoveState.Grounded;
			}

		}
		private bool CheckRopeCollision(Vector2 entityPos, Component.Movement movement)
		{
			var layerMask = 1 << LayerMask.NameToLayer("Collideable");
			Vector2 direction = entityPos - movement.CurrentRoped.RayCastOrigin;
			RaycastHit2D hit = Physics2D.Raycast(movement.CurrentRoped.RayCastOrigin, direction.normalized, movement.CurrentRoped.Length, layerMask);
			if (hit.collider != null)
			{
				float ropeL = (entityPos - hit.point).magnitude;
				movement.CurrentState = Component.Movement.MoveState.Roped;
				movement.OldRope = movement.CurrentRoped;
				movement.CurrentRoped = new Component.Movement.RopedData()
				{
					RayCastOrigin = ((0.3f * hit.normal) + hit.point),
					origin = hit.point,
					Length = ropeL,
					Damp = GameUnity.RopeDamping
				};
				return true;
			}
			return false;
		}
		public override void LeaveState(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{

		}
	}
}
