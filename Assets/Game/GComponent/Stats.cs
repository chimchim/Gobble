using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Stats : GComponent
	{
		private static ObjectPool<Stats> _pool = new ObjectPool<Stats>(100);
		public float HP;
		public float OxygenSeconds;
		public float MaxOxygenSeconds;
		public bool Alive;
		public Stats()
		{

		}
		public static Stats Make(int entityID, float hp, float oxygenSec, float maxOxygenSec)
		{
			Stats comp = _pool.GetNext();
			comp.EntityID = entityID;
			comp.Alive = true;
			comp.HP = hp;
			comp.OxygenSeconds = oxygenSec;
			comp.MaxOxygenSeconds = maxOxygenSec;
			return comp;
		}
	}
}
