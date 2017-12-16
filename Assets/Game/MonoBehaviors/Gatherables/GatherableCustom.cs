using System;
using Game;
using UnityEngine;
using System.Collections;

namespace Gatherables
{
	public class GatherableCustom : Gatherable
	{

		public int CustomIndex;

		public override GameObject GetGameObject(GameManager game)
		{
			return game.TileMap.CustomGatherables[CustomIndex];
		}

		public override void OnHit()
		{

		}
		public override void SetResource(GameManager game)
		{

		}
	}
}
