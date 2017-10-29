using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;

namespace Game.Systems
{
	public class DeadReckoning : ISystem
	{
		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player, ActionQueue>();
		bool[,] foundTile;

		public void Update(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);

			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);
				var input = game.Entities.GetComponentOf<InputComponent>(e);
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