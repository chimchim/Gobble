using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using UnityEditor;

namespace Game.Movement
{
	public class RopedNew : MovementState
	{
		//bool VertGrounded;
		bool Roped;
		float FirstLen;
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
			Vector2 currentTranslate = (movement.CurrentVelocity * Time.deltaTime) + (movement.ForceVelocity * Time.deltaTime);
			Vector2 playerPosFirstMove = playerPos + currentTranslate;
			Vector2 origin = movement.CurrentRoped.origin;
			float len = movement.CurrentRoped.Length;
			if (movement.RopeColList.Count == 0)
				len = movement.CurrentRoped.FirstLen;

			float diff = (playerPosFirstMove - origin).magnitude;
			float vel = movement.CurrentRoped.Vel;
			float angle = movement.CurrentRoped.Angle;
			float aAcc = 0;

			float yMovement = 0;
			float xMovement = 0;
			float lastAngle = 0;
			float deltaTimeMult = 1 / Time.deltaTime;
			Debug.DrawLine(origin, playerPos, Color.red);
			//Debug.DrawLine(playerPos, playerPos+ ((movement.CurrentVelocity * Time.deltaTime) + (movement.ForceVelocity * Time.deltaTime)), Color.blue);
			if ((diff >= len || movement.CurrentRoped.FirstAngle) && !movement.Grounded)
			{
				#region FirstAngle
				if (!movement.CurrentRoped.FirstAngle)
				{

					movement.CurrentRoped.Length = (playerPos - origin).magnitude;
					//if (movement.RopeColList.Count == 0)
					//{
					//	movement.CurrentRoped.Length = movement.CurrentRoped.FirstLen;
					//}
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
					//if (!VertGrounded)
					//{
					float sinned = Mathf.Abs(Mathf.Sin(angle));
					float cosed = Mathf.Abs(Mathf.Cos(angle));
					Vector2 combinedVelocity = movement.CurrentVelocity + movement.ForceVelocity;
					float newXVel = combinedVelocity.x * cosed;
					float newYVel = combinedVelocity.y * sinned;

					float velXDir = newXVel * -Mathf.Sign(Mathf.Cos(angle));
					float velYDir = newYVel * Mathf.Sign(Mathf.Sin(angle));

					float newSpeed = velXDir + velYDir;
					Debug.Log("RECAULC");
					float ropeDirection = Mathf.Sign(newSpeed);
					movement.CurrentRoped.Vel = (newSpeed * Time.deltaTime / len);

					angle += movement.CurrentRoped.Vel;
					movement.CurrentRoped.Angle = angle;
					//}
				}
				#endregion
				playerPos.x = origin.x + (-len * Mathf.Sin(angle));
				playerPos.y = origin.y + (-len * Mathf.Cos(angle));
				lastAngle = angle - movement.CurrentRoped.Vel;

				xMovement = playerPos.x - entityGameObject.transform.position.x;
				yMovement = playerPos.y - entityGameObject.transform.position.y;
				//VertGrounded = false;
				Roped = true;
				movement.Grounded = false;
				movement.CurrentVelocity.y = deltaTimeMult * yMovement;
				movement.CurrentVelocity.x = 0;
				movement.ForceVelocity = (deltaTimeMult * new Vector2(xMovement, 0));
				float xDiff = Mathf.Abs(entityGameObject.transform.position.x - origin.x);
				float xDiffLenPercent = xDiff / len;
				if (entityGameObject.transform.position.y > origin.y && xDiffLenPercent < 0.5f)
				{
					//movement.RopeForce.y -= GameUnity.Gravity * GameUnity.Weight;
					//if (Mathf.Abs(vel) < 0.002f)
					//{
					//	movement.CurrentVelocity.y = -GameUnity.Gravity;
					//	movement.CurrentRoped.FirstAngle = false;
					//}
				}

				float gravity = GameUnity.RopeGravity;
				#region Rope Input
				if (playerPos.x < origin.x)
				{
					if (input.Axis.x > 0)
					{
						gravity += gravity * GameUnity.PlayerSpeed / 4;
					}
				}
				if (playerPos.x > origin.x)
				{
					if (input.Axis.x < 0)
					{
						gravity += gravity * GameUnity.PlayerSpeed / 4;
					}
				}
				#endregion
				float gravityDivier = Math.Max(1, len);

				aAcc = (-1 * gravity / gravityDivier) * Mathf.Sin(angle) * Time.deltaTime;

				movement.CurrentRoped.Vel += aAcc;
				movement.CurrentRoped.Vel *= GameUnity.RopeDamping;
				movement.CurrentRoped.Angle += movement.CurrentRoped.Vel;
			}
			else
			{
				Roped = false;
				//VertGrounded = false;
				//movement.CurrentRoped.FirstAngle = false;
				Debug.Log("Not roped");
				movement.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
				movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
				movement.RopeForce = Vector2.zero;
				movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed;
				movement.ForceVelocity.x = Mathf.Clamp(movement.ForceVelocity.x, -15, 15);
				movement.ForceVelocity.y = Mathf.Clamp(movement.ForceVelocity.y, -15, 15);
				movement.ForceVelocity.x = movement.ForceVelocity.x * GameUnity.ForceDamper;
				movement.ForceVelocity.y = movement.ForceVelocity.y * GameUnity.ForceDamper;
				movement.CurrentRoped.FirstAngle = false;

				if (input.Space && movement.Grounded)
				{
					movement.CurrentVelocity.y = GameUnity.JumpSpeed;
				}

				yMovement = movement.CurrentVelocity.y * Time.deltaTime + (movement.ForceVelocity.y * Time.deltaTime);
				xMovement = movement.CurrentVelocity.x * Time.deltaTime + (movement.ForceVelocity.x * Time.deltaTime);
				if (movement.Grounded && (playerPos + new Vector2(xMovement, 0) - origin).magnitude > len)
				{
					xMovement = 0;
				}
				
			}
			stats.OxygenSeconds += Time.deltaTime;
			stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

			//Debug.Log("Movement L " + new Vector2(xMovement, yMovement).magnitude/0.1f);
			int steps = (int)(new Vector2(xMovement, yMovement).magnitude / 0.1f) +3;
			steps = 1;
			float xMovePerStep = xMovement / steps;
			float yMovePerStep = yMovement / steps;
			for (int i = 0; i < steps; i++)
			{
				Vector3 tempPos = entityGameObject.transform.position;
				Vector3 oldPos = tempPos;
				if (Roped)
				{
					tempPos = RopeCollision(tempPos, xMovePerStep, yMovePerStep, movement, lastAngle, origin, oldPos);
				}
				else
				{
					tempPos = AirCollision(tempPos, xMovePerStep, yMovePerStep, movement, lastAngle, origin);
				}
				entityGameObject.transform.position = tempPos;
				if(Roped)
				CheckRopeCollision(oldPos, tempPos, movement, lastAngle);
			}
		}
		private Vector3 RopeCollision(Vector3 tempPos, float xMovement, float yMovement, Component.Movement movement, float lastAngle, Vector2 origin, Vector2 oldPos)
		{
			float xOffset = GameUnity.RopeHitBox.x;
			float yOffset = GameUnity.RopeHitBox.y;

			bool vertGrounded = false;
			bool horGrounded = false;
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, out vertGrounded);
			if (vertGrounded && yMovement != 0)
			{
				Debug.Log("vertGrounded " + vertGrounded);
				if (yMovement > 0)
				{
					movement.CurrentRoped.Angle = lastAngle;
					movement.CurrentRoped.Vel = 0;
					//movement.CurrentRoped.FirstAngle = false;
					movement.CurrentVelocity.y = 0;
					//VertGrounded = true;
				}
				else
				{
					movement.Grounded = true;
					movement.CurrentRoped.FirstAngle = false;
				}
				movement.CurrentRoped.Vel = 0;
			}
			if (xMovement != 0)
			{
				tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
				if (horGrounded)
				{
					Debug.Log("horGrounded " + horGrounded + " xMovement " + xMovement);
					if (tempPos.y < origin.y)
					{
						//VertGrounded = true;
						//movement.CurrentRoped.FirstAngle = false;
					}
					//movement.ForceVelocity.x = 0;
					movement.CurrentRoped.Angle = lastAngle;
					movement.CurrentRoped.Vel = 0;
					tempPos = oldPos;

				}
			}
			return tempPos;
		}
		private Vector3 AirCollision(Vector3 tempPos, float xMovement, float yMovement, Component.Movement movement, float lastAngle, Vector2 origin)
		{
			float xOffset = GameUnity.RopeHitBox.x;
			float yOffset = GameUnity.RopeHitBox.y;

			bool vertGrounded = false;
			bool horGrounded = false;
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, out vertGrounded);
			if (vertGrounded)
			{
				//Debug.Log("AirCollision  vertGrounded" + vertGrounded);
				movement.CurrentVelocity.y = 0;
				movement.ForceVelocity.x = 0;
			}
			else
			{
				//Debug.Log("AirCollision  not grounded" + vertGrounded + " yMovement " + yMovement);
				movement.Grounded = false;
			}
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			if (horGrounded)
			{
				//Debug.Log("AirCollision horGrounded " + horGrounded);
				movement.ForceVelocity.x = 0;
			}
			return tempPos;
		}

		public bool IsLeft(Vector2 a, Vector2 b, Vector2 c)
		{
			return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
		}

		private bool CheckRopeCollision(Vector2 oldPos, Vector2 playerPos, Component.Movement movement, float lastAngle)
		{
			if (movement.RopeColList.Count > 0)
			{
				var oldRope = movement.RopeColList[movement.RopeColList.Count -1];
				var isLeft = IsLeft(oldRope.RayCastOrigin, ((oldRope.RayCastCollideOldPos - oldRope.RayCastOrigin).normalized * 30) + oldRope.RayCastOrigin, playerPos);
				//Debug.DrawLine(oldRope.RayCastOrigin, movement.CurrentRoped.RayCastCollideOldPos, Color.blue);

				if (isLeft == oldRope.NewRopeIsLeft)
				{
					Debug.DrawLine(oldRope.RayCastOrigin, oldRope.RayCastCollideOldPos, Color.blue);
					//Debug.Log("REMOVE ROPE RayCastOrigin " + oldRope.RayCastOrigin + " origin " + oldRope.origin + " current oldPos  " + movement.CurrentRoped.RayCastCollideOldPos);
					movement.CurrentRoped.origin = oldRope.origin;
					movement.CurrentRoped.FirstAngle = false;
					movement.CurrentRoped.RayCastOrigin = oldRope.RayCastOrigin;
					movement.CurrentRoped.RayCastCollideOldPos = oldRope.RayCastCollideOldPos;
					movement.CurrentRoped.Length = (playerPos - movement.CurrentRoped.origin).magnitude;
					movement.RopeColList.RemoveAt(movement.RopeColList.Count - 1);
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
					float ropeL = (playerPos - secondHit.point).magnitude;
					if (ropeL < 0.6f)
						return false;
					movement.CurrentRoped.RayCastCollideOldPos = oldPos;
					var ropeCol = new Component.Movement.RopeCol()
					{
						RayCastOrigin = movement.CurrentRoped.RayCastOrigin,
						origin = movement.CurrentRoped.origin,
						//Length = ropeL,
						//Damp = GameUnity.RopeDamping,
						RayCastCollideOldPos = oldPos,
						NewRopeIsLeft = !isLeft
						//OldRopeCollidePos = hit.point
					};
					//Debug.Log("NEW ROPE RayCastOrigin " + movement.CurrentRoped.RayCastOrigin + " origin " + movement.CurrentRoped.origin + " oldPos " + oldPos);
					movement.RopeColList.Add(ropeCol);
					movement.CurrentRoped.origin = secondHit.point;
					movement.CurrentRoped.FirstAngle = false;
					movement.CurrentRoped.Length = ropeL;
					movement.CurrentRoped.RayCastOrigin = ((0.05f * secondHit.normal) + secondHit.point);

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
