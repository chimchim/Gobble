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

		public override void EnterState(GameManager game, Animal animal, Entity entity, Player host)
		{
			otherDirectionTimer = -1;
			entity.Animator.SetBool("Fall", false);
			direction = 0;
		}

		public override void Update(GameManager game, Animal animal, Entity entity, Player host, float delta)
		{
			var position = entity.gameObject.transform.position;
			bool isSafe = false;
			var closest = ClosestPlayer(game, position);
			var fromPlayer = position - closest;
			if (host.IsHost)
			{
				if (fromPlayer.magnitude > (game.Animals.RabbitAggro * 2.0f))
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
			}

			animal.CurrentVelocity.y += -GameUnity.Gravity * animal.Weight;
			animal.CurrentVelocity.y = Mathf.Max(animal.CurrentVelocity.y, -GameUnity.MaxGravity);
			animal.CurrentVelocity.x = direction * game.Animals.RabbitSpeed * 1.6f;
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
				if (jumpTimer > 0.45f && host.IsHost)
				{
					jumpTimer = 0;
					Debug.Log("Rabbit jump ID " + animal.EntityID);
					HandleNetEventSystem.AddEventAndHandle(game, host.EntityID, NetAnimalJump.Make(animal.EntityID, game.PrivateRandom.Next(13, 22)));
				}
				if (isSafe && host.IsHost)
					animal.TransitionState(game, entity, this.GetType(), typeof(RabbitChill), host);
			}
			else
			{
				animal.Grounded = false;
			}
			if (host.IsHost)
			{
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
		}
		public override void InnerSerialize(GameManager game, Animal animal, List<byte> byteArray)
		{
			byteArray.AddRange(BitConverter.GetBytes(direction));
		}
		public override void InnerDeSerialize(GameManager game, Animal animal, byte[] byteData, ref int index)
		{
			direction = BitConverter.ToSingle(byteData, index);
			index += sizeof(float);
		}
		public override void LeaveState(GameManager game, Animal animal, Entity entity, Player host)
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
