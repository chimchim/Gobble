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
					foreach (NetEvent netevent in netEvents)
					{
						if (netevent.Iterations == 0)
						{
							netevent.Handle(game);
						}
						netevent.Iterations++;
					}
					for (int i = netEvents.Count - 1; i >= 0; i--)
					{
						if (netEvents[i].Iterations >= 150)
						{
							netEvents[i].Recycle();
							netEvents.RemoveAt(i);
							continue;
						}
					}
				}
			}
		}

		public static void AddEvent(Game.GameManager game, int ent, NetEvent e)
		{
			var netComp = game.Entities.GetComponentOf<NetEventComponent>(ent);
			netComp.CurrentEventID++;
			e.NetEventID = netComp.CurrentEventID;
			netComp.NetEvents.Add(e);
		}

		public static void AddEventAndHandle(Game.GameManager game, int ent, NetEvent e)
		{
			var netComp = game.Entities.GetComponentOf<NetEventComponent>(ent);
			netComp.CurrentEventID++;
			e.Iterations = 1;
			e.NetEventID = netComp.CurrentEventID;
			e.Handle(game);
			netComp.NetEvents.Add(e);
		}

		public static void AddEventIgnoreOwner(Game.GameManager game, int ent, NetEvent e)
		{
			var netComp = game.Entities.GetComponentOf<NetEventComponent>(ent);
			netComp.CurrentEventID++;
			e.Iterations = 1;
			e.NetEventID = netComp.CurrentEventID;
			netComp.NetEvents.Add(e);
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
