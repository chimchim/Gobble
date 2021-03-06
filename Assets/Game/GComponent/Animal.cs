﻿using Game.Movement;
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

		public Vector2 HostPosition;
		public Vector2 HostVelocity;
		public float Health;
		public float Weight;
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
			comp.Weight = 2;
			return comp;
		}

		public void TransitionState(GameManager game, GEntity.Entity entity, Type from, Type to, Player host)
		{
			for (int i = 0; i < States.Count; i++)
			{
				if (States[i].GetType() == from)
				{
					States[i].LeaveState(game, this, entity, host);
				}
			}
			for (int i = 0; i < States.Count; i++)
			{
				if (States[i].GetType() == to)
				{
					States[i].EnterState(game, this, entity, host);
					CurrentState = States[i];
				}
			}
		}

		public void SyncPosition(GameManager game, Vector2 hostPos, Vector2 hostVel)
		{

		}
	}
}
