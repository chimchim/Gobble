using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Stats : GComponent
	{
		private static ObjectPool<Stats> _pool = new ObjectPool<Stats>(10);
		public float HP;
		public float OxygenSeconds;
		public float MaxOxygenSeconds;
		public HpBar HpBar;
		public ScriptableCharacter ScrChar;
		public CharacterStats CharacterStats;
		public override void Recycle()
		{
			HpBar = null;
			HP = 0;
			OxygenSeconds = 0;
			MaxOxygenSeconds = 0;
			_pool.Recycle(this);
		}

		public Stats()
		{

		}
		public static Stats Make(int entityID, float hp, float oxygenSec, float maxOxygenSec, ScriptableCharacter scrChar)
		{
			Stats comp = _pool.GetNext();
			comp.EntityID = entityID;
			comp.HP = hp;
			comp.OxygenSeconds = oxygenSec;
			comp.MaxOxygenSeconds = maxOxygenSec;
			comp.ScrChar = scrChar;
			comp.CharacterStats = scrChar.GetStats();
			return comp;
		}
	}
}
