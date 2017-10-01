using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using UnityEditor;

namespace Game.Movement
{
	public class Roped : MovementState
	{

		public override void EnterState(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{

		}
		private void DrawRopes(GameManager game, Component.Movement movement, Game.Component.Resources resources, Vector2 position)
		{
			int positionAmount = (movement.RopeIndex * 2) + 2;
			Vector2[] drawPositions = new Vector2[positionAmount];

			drawPositions[0] = movement.RopeList[0].origin;
			drawPositions[1] = position;

			int currentIndex = 1;
			for (int i = 1; i < movement.RopeList.Count; i++)
			{
				var rope = movement.RopeList[i];
				drawPositions[currentIndex] = rope.OldRopeCollidePos;
				currentIndex++;
				drawPositions[currentIndex] = rope.origin;
				currentIndex++;
				drawPositions[currentIndex] = position;

			}
			resources.GraphicRope.DrawRope(drawPositions, position, movement.RopeIndex);

		}
		public override void Update(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{
			var input = game.Entities.GetComponentOf<Game.Component.Input>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);
			var resources = game.Entities.GetComponentOf<Game.Component.Resources>(entityID);
			
			Vector2 playerPos = entityGameObject.transform.position;
			DrawRopes(game, movement, resources, playerPos);
			Vector2 currentTranslate = (movement.CurrentVelocity * Time.deltaTime) + (movement.ForceVelocity * Time.deltaTime);
			Vector2 playerPosFirstMove = playerPos + currentTranslate;
			Vector2 origin = movement.CurrentRoped.origin;

			float len = movement.CurrentRoped.Length;
			float diff = (playerPosFirstMove - origin).magnitude;
			float vel = movement.CurrentRoped.Vel;
			float angle = movement.CurrentRoped.Angle;
			float aAcc = 0;
			//Debug.DrawLine(playerPos, movement.CurrentRoped.origin, Color.red);
			float yMovement = 0;
			float xMovement = 0;
			float lastAngle = 0;
			float deltaTimeMult = 1 / Time.deltaTime;
			if (movement.RopeIndex > 0 || diff > len || movement.CurrentRoped.FirstAngle || (currentTranslate.y < 0 && playerPos.y < origin.y))
			{
				
				#region First Angle
				if (!movement.CurrentRoped.FirstAngle)
				{
					if (movement.RopeIndex == 0)
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
					angle += newVel;
					movement.CurrentVelocity = Vector2.zero;
					xMovement = playerPos.x - entityGameObject.transform.position.x;
					yMovement = playerPos.y - entityGameObject.transform.position.y;
					movement.CurrentRoped.Angle = angle;
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
				
				aAcc = (-1 * gravity / gravityDivier) * Mathf.Sin(angle) * Time.deltaTime;

				movement.CurrentRoped.Vel += aAcc;
				movement.CurrentRoped.Vel *= movement.CurrentRoped.Damp;
				movement.CurrentRoped.Angle += movement.CurrentRoped.Vel;

			}
			#region Else 
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
			#endregion
			stats.OxygenSeconds += Time.deltaTime;
			stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

			float xOffset = 0.35f;
			float yOffset = 0.65f;

			bool vertGrounded = false;
			bool horGrounded = false;
			Vector3 oldPos = entityGameObject.transform.position;
			Vector3 tempPos = entityGameObject.transform.position;
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, out vertGrounded);
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			entityGameObject.transform.position = tempPos;
			if (vertGrounded)
			{
				if (yMovement > 0)
				{
					tempPos = oldPos;
					entityGameObject.transform.position = oldPos;
					movement.CurrentRoped.Angle = lastAngle;
					movement.CurrentRoped.Vel = -movement.CurrentRoped.Vel * 0.3f;
				}
				else
				{
					movement.RopeList.Clear();
					movement.RopeIndex = 0;
					movement.CurrentState = Component.Movement.MoveState.Grounded;
					resources.GraphicRope.DeActivate();
					Debug.LogError("HOOK BROOK GROUNDED yMovement " + yMovement + " tempPos " + tempPos + " oldPos " + oldPos);
				}
			}
			if (horGrounded && !vertGrounded)
			{
				tempPos = oldPos;
				entityGameObject.transform.position = oldPos;
				movement.CurrentRoped.Angle = lastAngle;
				movement.CurrentRoped.Vel = -movement.CurrentRoped.Vel * 0.3f;
			}

			var collided = CheckRopeCollision(oldPos, tempPos, movement, lastAngle);

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
		public bool IsLeft(Vector2 a, Vector2 b, Vector2 c)
		{
			return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
		}
		private bool CheckRopeCollision(Vector2 oldPos, Vector2 playerPos, Component.Movement movement, float lastAngle)
		{

			if (movement.RopeList.Count > 1)
			{
				var oldRope = movement.RopeList[movement.RopeIndex  - 1];
				var isLeft = IsLeft(oldRope.RayCastOrigin, ((movement.CurrentRoped.RayCastCollideOldPos - oldRope.RayCastOrigin).normalized * 30) + oldRope.RayCastOrigin, playerPos);

				if(isLeft == movement.CurrentRoped.NewRopeIsLeft)
				{
					float vel = movement.CurrentRoped.Vel;
					movement.CurrentRoped = oldRope;
					movement.CurrentRoped.Vel = vel;
					movement.RopeList.RemoveAt(movement.RopeIndex);
					movement.RopeIndex--;

					return false;
				}
				//Debug.DrawLine(oldRope.RayCastOrigin, movement.CurrentRoped.RayCastCollideOldPos, Color.blue);
			}
			var layerMask = 1 << LayerMask.NameToLayer("Collideable");
			Vector2 direction = playerPos - movement.CurrentRoped.RayCastOrigin;
			RaycastHit2D hit = Physics2D.Raycast(movement.CurrentRoped.RayCastOrigin, direction.normalized, direction.magnitude, layerMask);
			if (hit.collider != null)
			{

				Vector2 secondDirection = movement.CurrentRoped.origin - playerPos;
				RaycastHit2D secondHit = Physics2D.Raycast(playerPos, secondDirection.normalized, movement.CurrentRoped.Length, layerMask);
				if (secondHit.collider != null)
				{
					var isLeft = IsLeft(movement.CurrentRoped.RayCastOrigin, ((oldPos - movement.CurrentRoped.RayCastOrigin).normalized * 30) + movement.CurrentRoped.RayCastOrigin, playerPos);
					float ropeL =  (playerPos - secondHit.point).magnitude;
					movement.CurrentRoped.RayCastCollideOldPos = oldPos;
					movement.CurrentRoped = new Component.Movement.RopedData()
					{
						RayCastOrigin = ((0.05f * secondHit.normal) + secondHit.point),
						origin = secondHit.point,
						Length = ropeL,
						Damp = GameUnity.RopeDamping,
						RayCastCollideOldPos = oldPos,
						NewRopeIsLeft = !isLeft,
						OldRopeCollidePos = hit.point
					};
					movement.RopeList.Add(movement.CurrentRoped);
					movement.RopeIndex++;
				}
				
				return true;
			}
			return false;
		}
		public override void LeaveState(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{

		}
	}
}
