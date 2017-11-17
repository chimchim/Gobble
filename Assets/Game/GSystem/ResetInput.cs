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
				var player = game.Entities.GetComponentOf<Player>(entity);
				var input = game.Entities.GetComponentOf<InputComponent>(entity);
				if (player.Owner)
				{
					input.Space = false;
					input.RightClick = false;
				}
				input.NetworkJump = false;
				//input.GameLogicPackets.Clear();
			}
		}



		public void Initiate(GameManager game)
		{
		}
		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
	}
}
