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
		public int PlayerLayer = LayerMask.NameToLayer("Player");
		int platformLayer = LayerMask.NameToLayer("Platform");
		public Collider2D JumpLadder;
		public float JumpLadderTimer;
		float groundTimer;
		public override void EnterState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{
			Debug.Log("Enter Grounded");
		}
		public override void Update(GameManager game, MovementComponent movement, int entityID, Entity entity, float delta)
		{

			#region Variables
			movement.CurrentLayer = (int)Systems.Movement.LayerMaskEnum.Grounded;
			var input = game.Entities.GetComponentOf<InputComponent>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);
			var player = game.Entities.GetComponentOf<Game.Component.Player>(entityID);
			var animator = entity.Animator;
			var entityGameObject = entity.gameObject;
			float MoveSpeed = stats.CharacterStats.MoveSpeed;
			#endregion

			if (input.Axis.y < 0)
			{
				movement.CurrentLayer = (int)Systems.Movement.LayerMaskEnum.Roped;
			}
			movement.CurrentVelocity.y += -GameUnity.Gravity * stats.CharacterStats.Weight;
			movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
			
			movement.ForceVelocity.x = Mathf.Clamp(movement.ForceVelocity.x, -15, 15);
			movement.ForceVelocity.y = Mathf.Clamp(movement.ForceVelocity.y, -15, 15);
			movement.ForceVelocity.x = movement.ForceVelocity.x * GameUnity.ForceDamper;
			movement.ForceVelocity.y = movement.ForceVelocity.y * GameUnity.ForceDamper;

			float combinedSpeed = Mathf.Abs(movement.ForceVelocity.x + (input.Axis.x * MoveSpeed));
			float signedCombinedSpeed = Mathf.Sign(movement.ForceVelocity.x + input.Axis.x * MoveSpeed);
			float forceXSpeed = Mathf.Abs(movement.ForceVelocity.x);

			if (combinedSpeed > MoveSpeed && !movement.Grounded)
			{
				movement.ForceVelocity.x = signedCombinedSpeed * forceXSpeed;
			}
			else
			{
				movement.CurrentVelocity.x = input.Axis.x * MoveSpeed;
			}

			if ((input.Space && (movement.Grounded || groundTimer < 0.2f || GameUnity.DebugMode) && player.Owner))
			{
				HandleNetEventSystem.AddEventAndHandle(game, entityID, NetJump.Make(entityID));
			}
			if (!movement.Grounded)
			{
				groundTimer += delta;
			}
		
			float yMovement = movement.CurrentVelocity.y * delta + (movement.ForceVelocity.y * delta);
			float xMovement = movement.CurrentVelocity.x * delta + (movement.ForceVelocity.x * delta);
			Vector2 tempPos = entityGameObject.transform.position;
			var mask = player.Enemy ? game.LayerMasks.MappedMasksEnemy[movement.CurrentLayer] : game.LayerMasks.MappedMasks[movement.CurrentLayer];
			var box =  entityGameObject.GetComponent<BoxCollider2D>();
			var circle = entityGameObject.GetComponent<CircleCollider2D>();
			float yPos = -circle.offset.y + (circle.radius * 1.1f);
			if (yMovement > 0)
				yPos = box.offset.y  + box.size.y/2;
			int layer = 0;
			movement.Grounded = Game.Systems.Movement.CheckGrounded(tempPos, yMovement, yPos, mask, out layer);

			if (movement.Grounded)
			{
				if(yMovement < 0)
					groundTimer = 0;
				movement.CurrentVelocity.y = 0;
				yMovement = 0;

			}
			#region LadderCheck
			
			var ladder1 = VerticalMovementLadder(tempPos, yMovement, box.size.x, box.size.y);
			if (ladder1/* && (input.Axis.x != 0 || input.Axis.y != 0)*/ && !player.Dead)
			{
				var skipLadder = (JumpLadderTimer > 0 || ladder1 == JumpLadder);
				if (!skipLadder)
				{
					groundTimer = 0;
					movement.CurrentState = MovementComponent.MoveState.Ladder;
					Debug.Log("Go Ladder");
				}
				else
				{
					Debug.Log("JumpLadderTimer > 0 " + (JumpLadderTimer > 0) + " ladder1 == JumpLadder " + (ladder1 == JumpLadder));
				}
			}
			if (JumpLadderTimer <= 0)
				JumpLadder = null;
			#endregion
			JumpLadderTimer -= delta;

			Vector2 newPos = tempPos + new Vector2(xMovement, yMovement);
			movement.Body.MovePosition(newPos);
			Debug.DrawLine(tempPos, newPos, Color.blue);
			if (animator.isActiveAndEnabled)
			{
				animator.SetBool("Run", (movement.Grounded && input.Axis.x != 0));
				animator.SetBool("Jump", !movement.Grounded);
			}
			if(game.Client != null)
				Game.Systems.Movement.NetSync(game, player, movement, input, entityID, delta);

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

		public override void LeaveState(GameManager game, MovementComponent movement, int entityID, Entity entity)
		{

		}
	}
}
