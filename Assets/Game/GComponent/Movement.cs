using System.Collections.Generic;
using UnityEngine;

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
		public Roped CurrentRoped;

		public struct Roped
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
		public MoveState State;
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
			return comp;
        }
    }
}
