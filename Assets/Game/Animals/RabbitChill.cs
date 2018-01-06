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
		public override void EnterState(GameManager game, Animal animal, int entityID, Entity entity)
		{
			int rand = game.CurrentRandom.Next(0, 2);
			chillTimer = 1;
			if (rand == 0)
			{
				entity.Animator.SetBool("Eat", true);
				chillTimer = GameUnity.RabbitChillTimer;
			}
		}

		public override void Update(GameManager game, Animal animal, int entityID, Entity entity, float delta)
		{
			var position = entity.gameObject.transform.position;
			var closest = ClosestPlayer(game, position);
			var fromPlayer = position - closest;
			if (fromPlayer.magnitude < GameUnity.RabbitAggro)
			{
				animal.TransitionState(game, animal.EntityID, entity, this.GetType(), typeof(JumpFlee));
				return;
			}
			tempTimer += delta;
			if (tempTimer > chillTimer)
			{
				chillTimer = 1;
				tempTimer = 0;
				entity.Animator.SetBool("Eat", false);
				int rand = game.CurrentRandom.Next(0, 5);
				if (rand == 0)
					return;
				if (rand == 1)
				{
					chillTimer = GameUnity.RabbitChillTimer;
					entity.Animator.SetBool("Eat", true);
					return;
				}
				if (rand == 2)
				{
					animal.TransitionState(game, entityID, entity, this.GetType(), typeof(RabbitDig));
					return;
				}
				animal.TransitionState(game, entityID, entity, this.GetType(), typeof(RabbitPatrol));
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
		}

		public override void LeaveState(GameManager game, Animal animal, int entityID, Entity entity)
		{
			entity.Animator.SetBool("Eat", false);
			tempTimer = 0;
		}
	}
}
