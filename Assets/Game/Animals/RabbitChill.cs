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
			if (rand == 0)
			{
				entity.Animator.SetBool("Eat", true);
				chillTimer = 3;
			}
		}

		public override void Update(GameManager game, Animal animal, int entityID, Entity entity, float delta)
		{
			var position = entity.gameObject.transform.position;

			tempTimer += delta;
			if (tempTimer > chillTimer)
			{
				chillTimer = 1;
				tempTimer = 0;
				entity.Animator.SetBool("Eat", false);
				int rand = game.CurrentRandom.Next(0, 4);
				if (rand == 0)
					return;
				if (rand == 1)
				{
					chillTimer = 3;
					entity.Animator.SetBool("Eat", true);
					return;
				}
				animal.TransitionState(game, entityID, entity, this.GetType(), typeof(RabbitPatrol));
			}
		}

		public override void LeaveState(GameManager game, Animal animal, int entityID, Entity entity)
		{
			entity.Animator.SetBool("Eat", false);
			tempTimer = 0;
		}
	}
}
