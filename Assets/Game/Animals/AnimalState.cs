using Game;
using Game.Component;
using Game.GEntity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Movement
{
	public abstract class AnimalState
	{
			
		public abstract void EnterState(GameManager game, Animal animal, int entityID, Entity entity);
		public abstract void Update(GameManager game, Animal animal, int entityID, Entity entity, float delta);
		public abstract void LeaveState(GameManager game, Animal animal, int entityID, Entity entity);

		public Vector3 ClosestPlayer(GameManager game, Vector3 myPos)
		{ 
			var entities = game.Entities.GetEntitiesWithComponents(Bitmask.MakeFromComponents<InputComponent>());
			Vector3 closest = new Vector3(100000, 10000, 0);
			foreach (int e in entities)
			{
				var playerPos = game.Entities.GetEntity(e).gameObject.transform.position;
				if ((playerPos - myPos).magnitude < (closest - myPos).magnitude)
				{
					closest = playerPos;
				}
			}
			return closest;
		}
	}
}
