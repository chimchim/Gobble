using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class ResetInput : ISystem
	{
		// gör en input translator?

		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<InputComponent, Player, ActionQueue>();

		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var input = game.Entities.GetComponentOf<InputComponent>(entity);

				input.Space = false;
				input.OnRightDown = false;
				input.OnLeftDown = false;
			}
		}

		public void Initiate(GameManager game)
		{
		}
	}
}
