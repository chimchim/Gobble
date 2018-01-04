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

		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			if (Input.GetKeyDown(KeyCode.Z))
			{
				Vector2 mousePos = UnityEngine.Input.mousePosition;
				mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
				game.CreateRabbit(mousePos);
			}
			foreach (int e in entities)
			{
				var animal = game.Entities.GetComponentOf<Animal>(e);
				var entity = game.Entities.GetEntity(e);
				animal.CurrentState.Update(game, animal, e, entity, delta);
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
