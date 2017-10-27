using System.Collections.Generic;
using UnityEngine;


namespace Game.Component
{
	public class Input : GComponent
	{
		private static ObjectPool<Input> _pool = new ObjectPool<Input>(10);
		public Vector2 Axis;
		public Vector2 MousePos;
		public bool Space;
		public bool RightClick;
		public List<Client.GameLogicPacket> GameLogicPackets = new List<Client.GameLogicPacket>();
		public NetworkRopeConnected RopeConnected;
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
