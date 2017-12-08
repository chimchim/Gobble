using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class HandleNetEventSystem : ISystem
	{

		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player>();

		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				var netEvents = game.Entities.GetComponentOf<NetEventComponent>(entity).NetEvents;
				if (player.Owner)
				{

					for (int i = netEvents.Count - 1; i >= 0; i--)
					{
						if (netEvents[i].Iterations >= 4)
						{
							netEvents[i].Recycle();
							netEvents.RemoveAt(i);
							continue;
						}
						if (netEvents[i].Iterations == 0)
						{
							netEvents[i].Handle(game);
						}
						netEvents[i].Iterations++;
					}
				}
			}
		}


		public void Initiate(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var ent = game.Entities.GetEntity(entity);

			}
		}

		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
	}
}
