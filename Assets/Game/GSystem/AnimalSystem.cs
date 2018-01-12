using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;
namespace Game.Systems
{
	public class AnimalSystem : ISystem
	{
		// gör en input translator?

		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Animal>();
		private readonly Bitmask _player = Bitmask.MakeFromComponents<Player>();
		public void Update(GameManager game, float delta)
		{
			var animals = game.Entities.GetEntitiesWithComponents(_bitmask);
			var players = game.Entities.GetEntitiesWithComponents(_player);
			foreach (int p in players)
			{
				var player = game.Entities.GetComponentOf<Player>(p);
				if (!player.Owner)
					continue;
				foreach (int e in animals)
				{
					var animal = game.Entities.GetComponentOf<Animal>(e);
					var entity = game.Entities.GetEntity(e);
					animal.CurrentState.Update(game, animal, entity, player.IsHost, delta);
					float signDir = animal.CurrentVelocity.x;
					if (Mathf.Abs(signDir) > 0.3f)
					{
						int mult = (int)Mathf.Max((1 + Mathf.Sign(signDir)), 1);
						entity.Animator.transform.eulerAngles = new Vector3(entity.Animator.transform.eulerAngles.x, mult * 180, entity.Animator.transform.eulerAngles.z);
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
