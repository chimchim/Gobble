using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Stats : GComponent
	{
		private static ObjectPool<Stats> _pool = new ObjectPool<Stats>(100);
		public float HP;
		public bool Alive;
		public Stats()
		{

		}
		public static Stats Make(int entityID)
		{
			Stats comp = _pool.GetNext();
			comp.EntityID = entityID;
			comp.Alive = true;
			comp.HP = 100;
			return comp;
		}
	}
}
