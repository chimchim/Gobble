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
					resources.Ropes = MakeRopes();
					resources.DrawRopes = true;
				}
				//resources.DrawRopes
			}
		}

		private List<GameObject> MakeRopes()
		{
			List<GameObject> ropes = new List<GameObject>();
			var rope = GameObject.Instantiate(UnityEngine.Resources.Load("Prefabs/Rope", typeof(GameObject))) as GameObject;
			ropes.Add(rope);
			var parent = new GameObject();
			rope.transform.parent = parent.transform;
			for (int i = 0; i < 30; i++)
			{
				var newRope = GameObject.Instantiate(rope);
				newRope.transform.parent = parent.transform;
				newRope.SetActive(false);
				ropes.Add(newRope);
			}
				
			return ropes;
		}
		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
	}
}
