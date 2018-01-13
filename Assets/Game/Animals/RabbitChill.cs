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
	public class RabbitChill : AnimalState
	{
		public float tempTimer;
		private float chillTimer;

		bool eating;
		public RabbitChill(int index) : base(index) { }
		public override void EnterState(GameManager game, Animal animal, Entity entity, Player host)
		{
			if (!host.IsHost)
				return;
			int rand = game.PrivateRandom.Next(0, 2);
			chillTimer = 1;
			if (rand == 0)
			{
				eating = true;
				chillTimer = GameUnity.RabbitChillTimer;
			}
		}

		public override void Update(GameManager game, Animal animal, Entity entity, Player host, float delta)
		{
			var position = entity.gameObject.transform.position;
			if (host.IsHost)
			{
				var closest = ClosestPlayer(game, position);
				var fromPlayer = position - closest;
				if (fromPlayer.magnitude < GameUnity.RabbitAggro)
				{
					animal.TransitionState(game, entity, this.GetType(), typeof(JumpFlee), host);
					return;
				}
				tempTimer += delta;
				if (tempTimer > chillTimer)
				{
					chillTimer = 1;
					tempTimer = 0;
					eating = false;
					int rand = game.PrivateRandom.Next(0, 4);
					if (rand == 0)
						return;
					if (rand == 1)
					{
						chillTimer = GameUnity.RabbitChillTimer;
						eating = true;
						return;
					}
					if (rand == 2)
					{
						animal.TransitionState(game, entity, this.GetType(), typeof(RabbitDig), host);
						return;
					}
					animal.TransitionState(game, entity, this.GetType(), typeof(RabbitPatrol), host);
				}
			}
			animal.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
			animal.CurrentVelocity.y = Mathf.Max(animal.CurrentVelocity.y, -GameUnity.MaxGravity);
			animal.CurrentVelocity.x = 0;
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
			entity.gameObject.transform.position = tempPos;
			entity.Animator.SetBool("Eat", eating);
		}

		public override void InnerSerialize(GameManager game, Animal animal, List<byte> byteArray)
		{
			byteArray.AddRange(BitConverter.GetBytes(eating));
		}

		public override void InnerDeSerialize(GameManager game, Animal animal, byte[] byteData, ref int index)
		{
			eating = BitConverter.ToBoolean(byteData, index);
			index += sizeof(bool);
		}

		public override void LeaveState(GameManager game, Animal animal, Entity entity, Player host)
		{
			entity.Animator.SetBool("Eat", false);
			tempTimer = 0;
			eating = false;
		}
	}
}
