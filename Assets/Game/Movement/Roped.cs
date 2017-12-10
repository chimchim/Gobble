using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Game.GEntity;
using Game.Component;

namespace Game.Movement
{
	public class Roped : MovementState
	{
		public override void EnterState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
		private void DrawRopes(GameManager game, MovementComponent movement, ResourcesComponent resources, Vector2 position)
		{
			int positionAmount = ((movement.RopeList.Count - 1) * 2) + 2;
			Vector2[] drawPositions = new Vector2[positionAmount];
			if (movement.RopeList.Count == 0)
			{
				Debug.LogError("ROPE HAS 0 COUNT");
				return;
			}
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
			resources.GraphicRope.DrawRope(drawPositions, position, movement.RopeList.Count - 1);

		}

		public static float GetAngle(Vector2 position, Vector2 origin)
		{
			float angle2 = Vector2.Angle((origin - position).normalized, (-Vector2.up));
			float angle = 0;
			if (origin.x > position.x)
			{
				angle = (Mathf.PI / 180f) * (180 - angle2);
			}
			else
			{
				angle = (Mathf.PI / 180f) * -(180 - angle2);
			}
			return angle;
		}

		public static void ReleaseRope(ResourcesComponent resources, MovementComponent movement, Vector2 playerPos, Vector2 transformPos)
		{
			var diff1 = playerPos - transformPos;
			float speed = diff1.y * (1/Time.fixedDeltaTime);

			float clampedSpeed = Math.Min(speed, 20);
			float jumpSpeedMult = clampedSpeed / 20;
			jumpSpeedMult = 0.5f;
			if (speed > 10)
			{
				jumpSpeedMult = clampedSpeed / 20;

			}
			if (speed < 1)
			{
				jumpSpeedMult = 0;
			}
			movement.CurrentVelocity.y += GameUnity.JumpSpeed * jumpSpeedMult;
			movement.CurrentVelocity.y = Mathf.Clamp(movement.CurrentVelocity.y, 0, (GameUnity.JumpSpeed + GameUnity.JumpSpeed));
			movement.ForceVelocity.x = diff1.x * (1 / Time.fixedDeltaTime);
			resources.GraphicRope.DeActivate();
			movement.RopeList.Clear();
			movement.CurrentState = MovementComponent.MoveState.Grounded;
		}
		public override void Update(GameManager game, MovementComponent movement, int entityID, Entity entity, float delta)
		{
			var player = game.Entities.GetComponentOf<Player>(entityID);
			var input = game.Entities.GetComponentOf<InputComponent>(entityID);
			var stats = game.Entities.GetComponentOf<Stats>(entityID);
			var resources = game.Entities.GetComponentOf<ResourcesComponent>(entityID);
			var animator = entity.Animator;
			animator.SetBool("Roped", true);
			animator.SetBool("Run", false);
			var entityGameObject = entity.gameObject;
			Vector2 playerPos = entityGameObject.transform.position;

			Vector2 currentTranslate = (movement.CurrentVelocity * delta) + (movement.ForceVelocity * delta);
			Vector2 playerPosFirstMove = playerPos + currentTranslate;
			Vector2 origin = movement.CurrentRoped.origin;
			
			if (!player.Owner)
			{
				Vector2 syncDiff = input.NetworkPosition - playerPos;
				if (syncDiff.magnitude > 3)
				{
					var oldposer = entityGameObject.transform.position;
					int iterations = (int)(syncDiff.magnitude / 0.1f);
					var move = new Vector3((syncDiff.normalized * 0.1f).x, (syncDiff.normalized * 0.1f).y, 0);
					for (int i = 0; i < iterations; i++)
					{
						entityGameObject.transform.position += move;
						CheckRopeCollision(oldposer, entityGameObject.transform.position, movement);
					}
				}
			}

			float len = movement.CurrentRoped.Length;
			float diff = (playerPosFirstMove - origin).magnitude;
			float vel = movement.CurrentRoped.Vel;
			float angle = movement.CurrentRoped.Angle;
			float aAcc = 0;

			float yMovement = 0;
			float xMovement = 0;
			float lastAngle = 0;
			float deltaTimeMult = 1 / delta;
			if ((oldRoped || movement.RopeList.Count - 1 > 0 || diff > len || movement.CurrentRoped.FirstAngle || (currentTranslate.y < 0 && playerPos.y < origin.y)) && !movement.Grounded)
			{
				#region First Angle
				if (!movement.CurrentRoped.FirstAngle)
				{
					if (movement.RopeList.Count - 1 == 0)
						movement.CurrentRoped.Length = (playerPos - origin).magnitude;


					len = movement.CurrentRoped.Length;
					//Debug.Log("ROPE LEN " + len);
					angle = GetAngle(playerPos, origin);
					
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
					movement.CurrentRoped.Vel = (newSpeed * delta / len);
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
					//Debug.Log("newspeed " + newSpeed + " newVel " + movement.CurrentRoped.Vel);
				}
				#endregion

				
				playerPos.x = origin.x + (-len * Mathf.Sin(angle));
				playerPos.y = origin.y + (-len * Mathf.Cos(angle));
				lastAngle = angle - movement.CurrentRoped.Vel;
				xMovement = playerPos.x - entityGameObject.transform.position.x;
				yMovement = playerPos.y - entityGameObject.transform.position.y;
				Debug.DrawLine(playerPos, playerPos+(Vector2.up * 4), Color.red);
				if ((input.Space || input.NetworkJump))
				{
					if (player.Owner)
					{
						if (resources.GraphicRope.RopeItem.SemiActive)
						{
							var rope = resources.GraphicRope.RopeItem;
							resources.GraphicRope.RopeItem = null;
							rope.OwnerDeActivate(game, resources.EntityID);
						}
					}
					ReleaseRope(resources, movement, playerPos, new Vector2(entityGameObject.transform.position.x, entityGameObject.transform.position.y));
					return;
				}

				movement.CurrentVelocity.y = deltaTimeMult * yMovement;
				movement.ForceVelocity = (deltaTimeMult * new Vector2(xMovement, 0));
				float gravity = GameUnity.RopeGravity;
				#region Rope Input
				if (playerPos.x < origin.x)
				{
					if (input.Axis.x > 0)
					{
						gravity += gravity * GameUnity.PlayerSpeed/6 * GameUnity.RopeSpeedMult;
					}
				}
				if (playerPos.x > origin.x)
				{
					if (input.Axis.x < 0)
					{
						gravity += gravity * GameUnity.PlayerSpeed/6 * GameUnity.RopeSpeedMult;
					}
				}
				#endregion
				float gravityDivier = Math.Max(1, len);
				
				aAcc = (-1 * gravity / gravityDivier) * Mathf.Sin(angle) * delta;

				movement.CurrentRoped.Vel += aAcc;
				movement.CurrentRoped.Vel *= movement.CurrentRoped.Damp;
				movement.CurrentRoped.Angle += movement.CurrentRoped.Vel;

			}
			#region Else 
			else
			{
				movement.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
				movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
				if(movement.Grounded)
					movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed;

				movement.ForceVelocity.x = Mathf.Clamp(movement.ForceVelocity.x, -15, 15);
				movement.ForceVelocity.y = Mathf.Clamp(movement.ForceVelocity.y, -15, 15);
				movement.ForceVelocity.x = movement.ForceVelocity.x * GameUnity.ForceDamper;
				movement.ForceVelocity.y = movement.ForceVelocity.y * GameUnity.ForceDamper;

				yMovement = movement.CurrentVelocity.y * delta + (movement.ForceVelocity.y * delta);
				xMovement = movement.CurrentVelocity.x * delta + (movement.ForceVelocity.x * delta);
				animator.SetBool("Roped", false);
				animator.SetBool("Jump", false);
				if(input.Axis.x != 0)
					animator.SetBool("Run", true);
				Vector2 diffvec = playerPos - movement.CurrentRoped.origin + new Vector2(xMovement, yMovement);
				if ((input.Space && movement.Grounded) || input.NetworkJump)
				{
					movement.CurrentVelocity.y = GameUnity.JumpSpeed;
				}
				if (diffvec.magnitude > len)
				{
					movement.CurrentRoped.FirstAngle = false;
					movement.Grounded = false;
					return;
				}
				
			} 
			#endregion
			stats.OxygenSeconds += delta;
			stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

			float xOffset = GameUnity.RopeHitBox.x;
			float yOffset = GameUnity.RopeHitBox.y;

			bool vertGrounded = false;
			bool horGrounded = false;
			Vector3 oldPos = entityGameObject.transform.position;
			Vector3 tempPos = entityGameObject.transform.position;
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, out vertGrounded);
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			bool vertHorGrounded = vertGrounded || horGrounded;
			if (vertHorGrounded)
			{
				bool vertGrounded2 = false;
				bool horGrounded2 = false;
				var tempPos2 = oldPos;
				tempPos2 = Game.Systems.Movement.HorizontalMovement(tempPos2, xMovement, xOffset, yOffset, out horGrounded2);
				tempPos2 = Game.Systems.Movement.VerticalMovement(tempPos2, yMovement, xOffset, yOffset, out vertGrounded2);
				bool horVertGrounded = vertGrounded2 || horGrounded2;
				if (!horVertGrounded)
				{
					vertGrounded = vertGrounded2;
					horGrounded = horGrounded2;
					tempPos = tempPos2;
				}
			}
			entityGameObject.transform.position = tempPos;
			
			if (vertGrounded && yMovement != 0)
			{
				if (yMovement > 0)
				{
					movement.CurrentVelocity.y = 0;
					tempPos = oldPos;
					entityGameObject.transform.position = oldPos;
					movement.CurrentRoped.Angle = GetAngle(new Vector2(entityGameObject.transform.position.x, entityGameObject.transform.position.y), origin);
					movement.CurrentRoped.Vel = -movement.CurrentRoped.Vel * GameUnity.RopeBouncy;
				}
				else
				{
					movement.ForceVelocity = Vector2.zero;
					movement.Grounded = true;
				}
			}
			else
			{	if (movement.Grounded)
				{
					movement.CurrentRoped.FirstAngle = false;
				}
				movement.Grounded = false;
				
			}
			if (horGrounded && !vertGrounded)
			{
				movement.CurrentVelocity.y = 0;
				tempPos = oldPos;
				entityGameObject.transform.position = oldPos;
				movement.CurrentRoped.Angle = GetAngle(new Vector2(entityGameObject.transform.position.x, entityGameObject.transform.position.y), origin); ;
				movement.CurrentRoped.Vel = -movement.CurrentRoped.Vel * GameUnity.RopeBouncy;
			}
			oldRoped = false;
			
			var collided = CheckRopeCollision(oldPos, tempPos, movement);

			DrawRopes(game, movement, resources, playerPos);
			movement.FallingTime = 0;
			var layerMask = 1 << LayerMask.NameToLayer("Water");
			var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
			RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
			if (hit.collider != null)
			{
				movement.CurrentState = MovementComponent.MoveState.Grounded;
				resources.GraphicRope.DeActivate();
				movement.RopeList.Clear();
			}

		}
		public bool IsLeft(Vector2 a, Vector2 b, Vector2 c)
		{
			return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
		}
		bool oldRoped = false;
		private bool CheckRopeCollision(Vector2 oldPos, Vector2 playerPos, MovementComponent movement)
		{

			if (movement.RopeList.Count > 1)
			{
				var oldRope = movement.RopeList[movement.RopeList.Count - 2];
				var isLeft = IsLeft(oldRope.RayCastOrigin, ((movement.CurrentRoped.RayCastCollideOldPos - oldRope.RayCastOrigin).normalized * 30) + oldRope.RayCastOrigin, playerPos);

				if(isLeft == movement.CurrentRoped.NewRopeIsLeft)
				{
					float vel = movement.CurrentRoped.Vel;
					movement.CurrentRoped = oldRope;
					movement.CurrentRoped.Vel = vel;
					movement.RopeList.RemoveAt(movement.RopeList.Count - 1);
					oldRoped = true;
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
					if (ropeL < 0.9f)
						return false;
					movement.CurrentRoped.RayCastCollideOldPos = oldPos;
					movement.CurrentRoped = new MovementComponent.RopedData()
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
				}
				
				return true;
			}
			return false;
		}
		public override void LeaveState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
	}
}
