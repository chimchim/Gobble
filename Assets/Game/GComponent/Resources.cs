using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Resources : GComponent
	{
		private static ObjectPool<Resources> _pool = new ObjectPool<Resources>(10);
		public GraphicRope GraphicRope;
		public string Character;

		public override void Recycle()
		{
			GraphicRope = null;
			Character = "";
			_pool.Recycle(this);
		}
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
