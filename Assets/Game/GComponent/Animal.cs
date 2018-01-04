using Game.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Animal : GComponent
	{
		private static ObjectPool<Animal> _pool = new ObjectPool<Animal>(10);
		public Vector2 CurrentVelocity;
		public bool Grounded;

		public List<AnimalState> States = new List<AnimalState>();
		public AnimalState CurrentState;
		public override void Recycle()
		{
			_pool.Recycle(this);
		}

		public Animal()
		{

		}
		public static Animal Make(int entityID, List<AnimalState> states)
		{
			Animal comp = _pool.GetNext();
			comp.EntityID = entityID;
			comp.States = states;
			return comp;
		}

		public void TransitionState(GameManager game, int id, GEntity.Entity entity, Type from, Type to)
		{
			for (int i = 0; i < States.Count; i++)
			{
				if (States[i].GetType() == from)
				{
					States[i].LeaveState(game, this, id, entity);
				}
				if (States[i].GetType() == to)
				{
					States[i].EnterState(game, this, id, entity);
					CurrentState = States[i];
				}
			}
		}
	}
}
