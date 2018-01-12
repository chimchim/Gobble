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
	public class JumpFlee : AnimalState
	{
		float otherDirectionTimer;
		float otherDirection;
		float jumpTimer;
		float length;
		float lengthTimer;
		float direction;

		public JumpFlee(int index) : base(index) { }

		public override void EnterState(GameManager game, Animal animal, Entity entity, bool host)
		{
			otherDirectionTimer = -1;
			entity.Animator.SetBool("Fall", false);
		}

		public override void Update(GameManager game, Animal animal, Entity entity, bool host, float delta)
		{
			var position = entity.gameObject.transform.position;
			var closest = ClosestPlayer(game, position);
			var fromPlayer = position - closest;
			bool isSafe = false;
			if (fromPlayer.magnitude > (GameUnity.RabbitAggro * 2.0f))
			{
				isSafe = true;
			}
			if (animal.Grounded)
			{
				direction = Math.Sign(fromPlayer.x);
				if (otherDirectionTimer > 0f)
				{
					otherDirectionTimer -= delta;
					direction = otherDirection;
				}
			}
			animal.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
			animal.CurrentVelocity.y = Mathf.Max(animal.CurrentVelocity.y, -GameUnity.MaxGravity);
			animal.CurrentVelocity.x = direction * GameUnity.RabbitSpeed * 1.6f;
			float yMovement = animal.CurrentVelocity.y * delta;
			float xMovement = animal.CurrentVelocity.x * delta;

			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;

			bool vertGrounded = false;
			bool horGrounded = false;

			Vector3 tempPos = position;
			int layer = (int)Systems.Movement.LayerMaskEnum.Grounded;
			var mask = game.LayerMasks.MappedMasks[layer];
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, mask, out vertGrounded);
			tempPos.z = -0.2f;

			if (horGrounded)
			{

			}
			if (vertGrounded)
			{
				animal.CurrentVelocity.y = 0;
			}
			if (vertGrounded && yMovement < 0)
			{
				entity.Animator.SetBool("Jump", false);
				entity.Animator.SetBool("Walk", true);
				animal.Grounded = true;
				jumpTimer += delta;
				if (jumpTimer > 0.45f)
				{
					jumpTimer = 0;
					animal.CurrentVelocity.y = game.CurrentRandom.Next(13, 22);
					entity.Animator.SetBool("Jump", true);
					entity.Animator.SetBool("Walk", false);
				}
				if (isSafe)
					animal.TransitionState(game, entity, this.GetType(), typeof(RabbitChill), host);
			}
			else
			{
				animal.Grounded = false;
			}
			
			entity.gameObject.transform.position = tempPos;
			if (lengthTimer < 0.8f)
			{
				lengthTimer += delta;
				length += Math.Abs(entity.gameObject.transform.position.x - position.x);
			}
			if (lengthTimer > 0.8f)
			{
				lengthTimer = 0;
				if (length < 0.5f)
				{
					otherDirection = -Math.Sign(fromPlayer.x);
					otherDirectionTimer = 1f;
				}
				length = 0;
			}
		}
		public override void Serialize(GameManager game, int entity, List<byte> byteArray)
		{
			throw new NotImplementedException();
		}
		public override void Deserialize(object gameState, byte[] byteData, ref int index)
		{
			
		}
		public override void LeaveState(GameManager game, Animal animal, Entity entity, bool host)
		{
			lengthTimer = 0;
			length = 0;
			jumpTimer = 0;
			otherDirectionTimer = 0;
			entity.Animator.SetBool("Jump", false);
			entity.Animator.SetBool("Walk", false);
		}
	}
}
