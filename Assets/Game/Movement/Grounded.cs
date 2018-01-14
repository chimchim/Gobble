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
	public class Grounded : MovementState
	{
		public Collider2D JumpLadder;
		public float JumpLadderTimer;
		float groundTimer;
		public override void EnterState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
		public override void Update(GameManager game, MovementComponent movement, int entityID, Entity entity, float delta)
		{
			movement.CurrentLayer = (int)Systems.Movement.LayerMaskEnum.Grounded;
			var input = game.Entities.GetComponentOf<InputComponent>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);
			var player = game.Entities.GetComponentOf<Game.Component.Player>(entityID);
			var animator = entity.Animator;
			var entityGameObject = entity.gameObject;

			if (input.Axis.y < 0)
			{
				movement.CurrentLayer = (int)Systems.Movement.LayerMaskEnum.Roped;
			}
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

			animator.SetBool("Run", (movement.Grounded && input.Axis.x != 0));

			if ((input.Space && (movement.Grounded || groundTimer < 0.2f) && player.Owner))
			{
				HandleNetEventSystem.AddEventAndHandle(game, entityID, NetJump.Make(entityID));
			}
			if (!movement.Grounded)
			{
				groundTimer += delta;
			}

			float yMovement = movement.CurrentVelocity.y * delta + (movement.ForceVelocity.y * delta);
			float xMovement = movement.CurrentVelocity.x * delta + (movement.ForceVelocity.x * delta);

			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;

			bool vertGrounded = false;
			bool horGrounded = false;
			bool vertGrounded2 = false;
			bool horGrounded2 = false;

			Vector3 tempPos = entityGameObject.transform.position;
			var mask = game.LayerMasks.MappedMasks[movement.CurrentLayer];
			var tempPos1 = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			tempPos1 = Game.Systems.Movement.VerticalMovement(tempPos1, yMovement, xOffset, yOffset, mask, out vertGrounded);
			var tempPos2 = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, mask, out vertGrounded2);
			tempPos2 = Game.Systems.Movement.HorizontalMovement(tempPos2, xMovement, xOffset, yOffset, out horGrounded2);

			entityGameObject.transform.position = tempPos1;
			if ((tempPos2 - tempPos).magnitude > (tempPos1 - tempPos).magnitude)
			{
				entityGameObject.transform.position = tempPos2;
				horGrounded = horGrounded2;
				vertGrounded = vertGrounded2;
			}

			movement.Grounded = vertGrounded;
			animator.SetBool("Jump", !vertGrounded);

			if (vertGrounded)
			{
				movement.CurrentVelocity.y = 0;
				movement.ForceVelocity.y = 0;
				groundTimer = 0;
				movement.ForceVelocity.x *= 0.88f;
			}
			if(horGrounded)
			{
				movement.ForceVelocity.x = 0;
			}
		
			var ladder1 = VerticalMovementLadder(tempPos, yMovement, xOffset, yOffset);
			if (ladder1/* && (input.Axis.x != 0 || input.Axis.y != 0)*/)
			{
				var skipLadder = (JumpLadderTimer > 0 && ladder1 == JumpLadder);
				if (!skipLadder)
				{
					groundTimer = 0;
					movement.CurrentState = MovementComponent.MoveState.Ladder;
				}
			}
			JumpLadderTimer -= delta;
			var layerMask = 1 << LayerMask.NameToLayer("Water");
			var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
			RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
			if (hit.collider != null)
			{
				movement.CurrentState = MovementComponent.MoveState.Swimming;
			}		
		}

		public static Collider2D VerticalMovementLadder(Vector3 pos, float y, float Xoffset, float yoffset)
		{
			float half = yoffset - (yoffset / 10);
			float fullRayDistance = yoffset + Mathf.Abs(y) - half;
			var layerMask = 1 << LayerMask.NameToLayer("Ladder");

			float sign = Mathf.Sign(y);
			Vector3 firstStartY = pos;
			RaycastHit2D hitsY = new RaycastHit2D();
			hitsY = Physics2D.Raycast(firstStartY, Vector3.up * sign, 0.1f, layerMask);

			//var laddered = ((hitsY.collider != null));
			return hitsY.collider;
		}
		public static bool HorizontalMovementLadder(Vector3 pos, float x, float xoffset, float yoffset)
		{
			float fullRayDistance = xoffset + Mathf.Abs(x);
			var layerMask = 1 << LayerMask.NameToLayer("Ladder");
			float sign = Mathf.Sign(x);
			Vector3 firstStartX = new Vector3(0, -yoffset + 0.05f, 0) + pos;
			Vector3 secondStartX = new Vector3(0, yoffset - 0.05f, 0) + pos;
			RaycastHit2D[] hitsY = new RaycastHit2D[2];
			hitsY[0] = Physics2D.Raycast(firstStartX, Vector2.right * sign, fullRayDistance, layerMask);
			hitsY[1] = Physics2D.Raycast(secondStartX, Vector2.right * sign, fullRayDistance, layerMask);

			var laddered = ((hitsY[0].collider != null) || (hitsY[1].collider != null));
			return laddered;
		}

		public override void LeaveState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
	}
}
