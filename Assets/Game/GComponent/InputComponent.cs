using System.Collections.Generic;
using UnityEngine;


namespace Game.Component
{
	public class InputComponent : GComponent
	{
		private static ObjectPool<InputComponent> _pool = new ObjectPool<InputComponent>(10);
		public Vector2 Axis;
		public Vector2 MousePos;
		public Vector2 ArmDirection;
		
		public bool Space;
		public bool RightClick;
		public bool LeftClick;
		public bool E;

		public List<Client.GameLogicPacket> GameLogicPackets = new List<Client.GameLogicPacket>();
		public NetworkRopeConnected RopeConnected;
		public bool NetworkJump;
		public Vector2 NetworkPosition;
		public struct NetworkRopeConnected
		{
			public Vector2 RayCastOrigin;
			public Vector2 Position;
			public Vector2 Origin;
			public float Length;

		}
		public override void Recycle()
		{
			Axis = Vector2.zero;
			Space = false;
			RightClick = false;
			GameLogicPackets = null;
			GameLogicPackets = new List<Client.GameLogicPacket>();
			_pool.Recycle(this);
		}
		public InputComponent()
		{

		}
		public static InputComponent Make(int entityID)
		{
			InputComponent comp = _pool.GetNext();
			comp.EntityID = entityID;
			return comp;
		}
	}
}
