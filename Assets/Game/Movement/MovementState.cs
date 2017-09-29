using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Movement
{
	public abstract class MovementState
	{
		public Game.Component.Movement MoveComp;
		public Game.Component.Input InputComp;
		public Game.Component.Stats StatsComp;

		public abstract void EnterState(GameManager game, Component.Movement moveComp, int entityID, GameObject entityGameObject);
		public abstract void Update(GameManager game, Component.Movement moveComp, int entityID, GameObject entityGameObject);
		public abstract void LeaveState(GameManager game, Component.Movement moveComp, int entityID, GameObject entityGameObject);
	}
}
