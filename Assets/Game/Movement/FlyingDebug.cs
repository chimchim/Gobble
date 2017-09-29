using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;

namespace Game.Movement
{
	public class FlyingDebug : MovementState
	{
		public override void EnterState(GameManager game, Component.Movement moveComp, int entityID, GameObject entityGameObject)
		{

		}
		public override void Update(GameManager game, Component.Movement movement, int entityID, GameObject entityGameObject)
		{
			var input = game.Entities.GetComponentOf<Game.Component.Input>(entityID);
			var stats = game.Entities.GetComponentOf<Game.Component.Stats>(entityID);

			movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed * 3;
			movement.CurrentVelocity.y = input.Axis.y * GameUnity.PlayerSpeed * 3;
			float yMovement = movement.CurrentVelocity.y * Time.deltaTime;
			float xMovement = movement.CurrentVelocity.x * Time.deltaTime;

			entityGameObject.transform.position += new Vector3(xMovement, yMovement, 0);
		}
		public override void LeaveState(GameManager game, Component.Movement moveComp, int entityID, GameObject entityGameObject)
		{

		}
	}
}
