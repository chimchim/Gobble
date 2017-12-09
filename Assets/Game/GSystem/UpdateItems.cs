using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class UpdateItems : ISystem
	{
		// gör en input translator?
		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<InputComponent, Player, ActionQueue>();

		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);

			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);	
				var itemHolder = game.Entities.GetComponentOf<ItemHolder>(e);
				foreach (Item item in itemHolder.ActiveItems)
				{
					item.Input(game, e);
				}

				if (player.Owner)
				{

					var netEvents = game.Entities.GetComponentOf<NetEventComponent>(e);
					var input = game.Entities.GetComponentOf<InputComponent>(e);
					if (input.E)
					{
						var position = game.Entities.GetEntity(e).gameObject.transform.position;
						position += new Vector3(0, 2, 0);
						int forceX = game.CurrentRandom.Next(0, 11);
						int forceXNeg = game.CurrentRandom.Next(0, 2);
						forceX = forceXNeg == 1 ? -forceX : forceX;
						int forceY = game.CurrentRandom.Next(0, 10);
						var force = new Vector2(forceX, forceY);
						var itemrand = game.CurrentRandom.Next(0, 2);
						force = input.ArmDirection * 5;
						if (itemrand == 0)
						{
							netEvents.CurrentEventID++;
							netEvents.NetEvents.Add(NetCreateItem.Make(e, netEvents.CurrentEventID, Item.ItemID.Pickaxe, position, force));

						}
						if (itemrand == 1)
						{
							netEvents.CurrentEventID++;
							netEvents.NetEvents.Add(NetCreateItem.Make(e, netEvents.CurrentEventID, Item.ItemID.Rope, position, force));
						}
					}
				}
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
