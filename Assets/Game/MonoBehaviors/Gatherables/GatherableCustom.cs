using System;
using Game;
using UnityEngine;
using System.Collections;
using GatherLevel = GatherableScriptable.GatherLevel;

namespace Gatherables
{
	public class GatherableCustom : Gatherable
	{

		public int CustomIndex;

		public override GameObject GetGameObject(GameManager game)
		{
			return game.TileMap.CustomGatherables[CustomIndex];
		}

		public override bool OnHit(GameManager game, GatherLevel level)
		{
			if (level >= GatherScript.Level)
			{
				GetComponentInChildren<Animator>().SetTrigger("Hit");
				HitsTaken++;
				if (HitsTaken >= GatherScript.HitsNeeded)
					return true;
			}

			return false;
		}
		public override void SetResource(GameManager game)
		{

		}
	}
}
