using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class InitResources : ISystem
	{
		// gör en input translator?

		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Game.Component.Input, Player>();

		public void Update(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				if (player.Owner)
				{

				}
			}
		}


		public void Initiate(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				var resources = game.Entities.GetComponentOf<Game.Component.Resources>(entity);
				if (player.Owner)
				{
					GameObject Ropes = new GameObject();
					Ropes.AddComponent<GraphicRope>();
					Ropes.GetComponent<GraphicRope>().MakeRopes();
					resources.GraphicRope = Ropes.GetComponent<GraphicRope>();
				}
			}
		}

		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
	}
}
