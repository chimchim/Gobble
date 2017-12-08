using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class NetEventComponent : GComponent
	{
		private static ObjectPool<NetEventComponent> _pool = new ObjectPool<NetEventComponent>(10);

		public List<NetEvent> NetEvents = new List<NetEvent>();
		public int CurrentEventID;
		public override void Recycle()
		{

			_pool.Recycle(this);
		}
		public NetEventComponent()
		{

		}
		public static NetEventComponent Make(int entityID)
		{
			NetEventComponent comp = _pool.GetNext();
			comp.EntityID = entityID;
			return comp;
		}
	}
}
