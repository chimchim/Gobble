using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Input : GComponent
	{
		private static ObjectPool<Input> _pool = new ObjectPool<Input>(100);
		public Vector2 Axis;
		public bool Space;
		public bool RightClick;
		public Vector2 CurrentVelocity;

		public enum MoveState
		{
			Grounded,
			Swimming,
			Floating,
			FlyingDebug,
			Roped
		}
		public MoveState State;
		public bool Grounded;
		public float FallingTime;

		public int FloatingCounter;
		public bool FloatJump;
		public float SwimTime;
		public int OxygenDeplationTick;

		public Vector2 RopePosistion;
		public float RopeLength;
		public Input()
		{

		}
		public static Input Make(int entityID)
		{
			Input comp = _pool.GetNext();
			comp.EntityID = entityID;
			comp.OxygenDeplationTick = 1;
			return comp;
		}
	}
}
