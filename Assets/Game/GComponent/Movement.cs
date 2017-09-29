﻿using System.Collections.Generic;
using UnityEngine;
using Game.Movement;
namespace Game.Component
{
    public class Movement : GComponent
    {
        private static ObjectPool<Movement> _pool = new ObjectPool<Movement>(100);
		// Swim
		public int FloatingCounter;
		public bool FloatJump;
		public float SwimTime;
		public int OxygenDeplationTick;

		public Vector2 CurrentVelocity;
		public Vector2 ForceVelocity;
		public bool Jumped;
		public RopedData CurrentRoped;

		public struct RopedData
		{
			public float Vel;
			public float Angle;
			public Vector2 origin;
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
		public Movement()
        {
            
        }
        public static Movement Make(int entityID)
        {
            Movement comp = _pool.GetNext();
            comp.EntityID = entityID;
			comp.OxygenDeplationTick = 1;
			comp.States = new MovementState[5];
			comp.States[(int)MoveState.Grounded] = new Grounded();
			comp.States[(int)MoveState.Swimming] = new Swim();
			comp.States[(int)MoveState.Floating] = new Floating();
			comp.States[(int)MoveState.FlyingDebug] = new FlyingDebug();
			comp.States[(int)MoveState.Roped] = new Roped();
			comp.CurrentState = MoveState.Grounded;
            return comp;
        }
    }
}
