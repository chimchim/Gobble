using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Resources : GComponent
	{
		private static ObjectPool<Resources> _pool = new ObjectPool<Resources>(10);

		public bool DrawRopes;
		public List<GameObject> Ropes;
		public Resources()
		{

		}
		public static Resources Make(int entityID)
		{
			Resources comp = _pool.GetNext();
			comp.EntityID = entityID;
			return comp;
		}
	}
}
