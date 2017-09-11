using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Game.Component;
namespace Game.Actions
{
	public class AffectHP : Action
	{
		private static ObjectPool<AffectHP> _pool = new ObjectPool<AffectHP>(100);

		public float HP;
		public AffectHP()
		{

		}

		public static AffectHP Make(float hp)
		{
			AffectHP affectHP = _pool.GetNext();
			affectHP.HP = hp;
			return affectHP;
		}

		public override void Apply(GameManager game, int owner)
		{
			Player player = game.Entities.GetComponentOf<Player>(owner);
			Stats stats = game.Entities.GetComponentOf<Stats>(owner);

			stats.HP += HP;
			stats.HP = Mathf.Clamp(stats.HP, 0, GameUnity.MaxHP);
			if (player.Owner)
			{
				var hpBar = GameObject.FindObjectOfType<HpBar>();
				hpBar.SetHP(stats.HP);
			}
		}

		public override void Recycle()
		{
			_pool.Recycle(this);
		}

		public override string Type()
		{
			return "AffectHP";
		}
	}
}