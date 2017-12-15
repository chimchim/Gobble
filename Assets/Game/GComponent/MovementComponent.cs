using System.Collections.Generic;
using UnityEngine;
using Game.Movement;
using LayerMaskEnum = Game.Systems.Movement.LayerMaskEnum;

namespace Game.Component
{
    public class MovementComponent : GComponent
    {
        private static ObjectPool<MovementComponent> _pool = new ObjectPool<MovementComponent>(10);
		// Swim
		public int FloatingCounter;
		public bool FloatJump;
		public float SwimTime;
		public int OxygenDeplationTick;

		public Vector2 CurrentVelocity;
		public Vector2 ForceVelocity;
		public RopedData CurrentRoped;
		public RopedData OldRope;
		public List<RopedData> RopeList;

		public int CurrentLayer;
		public struct RopedData
		{
			public float Vel;
			public float Angle;
			public Vector2 origin;
			public Vector2 RayCastOrigin;
			public Vector2 RayCastCollideOldPos;
			public Vector2 OldRopeCollidePos;
			public bool NewRopeIsLeft;
			public float Length;
			public bool FirstAngle;
			public float Damp;
		}

		public enum MoveState
		{
			Grounded,
			Swimming,
			Floating,
			FlyingDebug,
			Roped
		}
		public MoveState CurrentState;
		public MovementState[] States;
		//public MovementState CurrentState;
		public bool Grounded;
		public float FallingTime;
		public MovementComponent()
        {
            
        }
		public override void Recycle()
		{
			Grounded = false;
			FallingTime = 0;
			States = null;
			RopeList = null;
			FloatingCounter = 0;
			FloatJump = false;
			SwimTime = 0;
			OxygenDeplationTick = 0;

			CurrentVelocity = Vector2.zero;
			ForceVelocity = Vector2.zero;
			RopeList.Clear();
			_pool.Recycle(this);
		}
		public static MovementComponent Make(int entityID)
        {
			MovementComponent comp = _pool.GetNext();
            comp.EntityID = entityID;
			comp.OxygenDeplationTick = 1;
			comp.States = new MovementState[5];
			comp.States[(int)MoveState.Grounded] = new Grounded();
			comp.States[(int)MoveState.Swimming] = new Swim();
			comp.States[(int)MoveState.Floating] = new Floating();
			comp.States[(int)MoveState.FlyingDebug] = new FlyingDebug();
			comp.States[(int)MoveState.Roped] = new Roped();
			comp.CurrentState = MoveState.Grounded;
			comp.RopeList = new List<RopedData>();

			return comp;
        }
    }
}
