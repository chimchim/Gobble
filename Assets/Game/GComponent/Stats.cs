using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Stats : GComponent
	{
		private static ObjectPool<Stats> _pool = new ObjectPool<Stats>(100);
		public float HP;
		public float Air;
		public bool Alive;
		public Stats()
		{

		}
		public static Stats Make(int entityID, float hp, float air)
		{
			Stats comp = _pool.GetNext();
			comp.EntityID = entityID;
			comp.Alive = true;
			comp.HP = hp;
			comp.Air = air;
			return comp;
		}
	}
}
