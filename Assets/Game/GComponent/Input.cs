using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Input : GComponent
	{
		private static ObjectPool<Input> _pool = new ObjectPool<Input>(100);
		public Vector2 Axis;
		public bool Space;

		public Vector2 CurrentVelocity;
		public float SwimGravity;
		public float EnterWater;
		public bool Grounded;
		public bool Swimming;
		public Input()
		{

		}
		public static Input Make(int entityID)
		{
			Input comp = _pool.GetNext();
			comp.EntityID = entityID;
			return comp;
		}
	}
}
