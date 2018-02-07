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
	public class FlyingDebug : MovementState
	{
		public override void EnterState(GameManager game, MovementComponent moveComp, int entityID, Entity entity)
		{

		}
		public override void Update(GameManager game, MovementComponent movement, int entityID, Entity entity, float delta)
		{
			var input = game.Entities.GetComponentOf<InputComponent>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);
			float PlayerSpeed = stats.CharacterStats.MoveSpeed;
			movement.CurrentVelocity.x = input.Axis.x * PlayerSpeed * 3;
			movement.CurrentVelocity.y = input.Axis.y * PlayerSpeed * 3;
			float yMovement = movement.CurrentVelocity.y * delta;
			float xMovement = movement.CurrentVelocity.x * delta;

			entity.gameObject.transform.position += new Vector3(xMovement, yMovement, 0);
		}
		public override void LeaveState(GameManager game, MovementComponent moveComp, int entityID, Entity entity)
		{

		}
	}
}
