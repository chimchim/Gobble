using Game;
using Game.Component;
using Game.GEntity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Movement
{
	public abstract class MovementState
	{
	
		public abstract void EnterState(GameManager game, MovementComponent moveComp, int entityID, Entity entity);
		public abstract void Update(GameManager game, MovementComponent moveComp, int entityID, Entity entity, float delta);
		public abstract void LeaveState(GameManager game, MovementComponent moveComp, int entityID, Entity entity);
	}
}
