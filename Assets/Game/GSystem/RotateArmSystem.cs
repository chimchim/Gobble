using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class RotateArmSystem : ISystem
	{

        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<InputComponent, Player, ActionQueue>();

		public void Update(GameManager game, float delta)
		{
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);

			foreach (int e in entities)
			{
				var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
				var input = game.Entities.GetComponentOf<InputComponent>(e);
				var player = game.Entities.GetComponentOf<Player>(e);
				var entity = game.Entities.GetEntity(e);
				float speed = player.CharacterStats.ArmRotationSpeed;
				Vector2 pos = entity.gameObject.transform.position;
				if (entity.Animator.transform.eulerAngles.y > 6)
				{
					input.Dir = Helper.RotateTowards(input.Dir, input.ScreenDirection, speed * delta);
					resources.FreeArm.up = input.Dir;
					resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
				}
				else
				{
					input.Dir = Helper.RotateTowards(input.Dir, input.ScreenDirection, speed * delta);
					resources.FreeArm.up = -input.Dir;
				}

				Debug.DrawLine(pos, pos + (input.Dir * 3), Color.blue);

			}
		}

		public void Initiate(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);

			foreach (int e in entities)
			{
				var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
				var input = game.Entities.GetComponentOf<InputComponent>(e);
				var entity = game.Entities.GetEntity(e);
				input.Dir = new Vector2(1, 0);
				//if (entity.Animator.transform.eulerAngles.y > 6)
				//{
				//	input.Dir = input.ScreenDirection;
				//	resources.FreeArm.up = input.ScreenDirection;
				//	resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
				//}
				//else
				//{
				//
				//	input.Dir = -input.ScreenDirection;
				//	resources.FreeArm.up = -input.ScreenDirection;
				//}
			}
		}
	}
}
