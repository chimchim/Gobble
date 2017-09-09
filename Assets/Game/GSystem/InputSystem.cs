using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class InputSystem : ISystem
	{
		// gör en input translator?

        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Game.Component.Input, Player, ActionQueue>();

		public void Update(GameManager game)
		{
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				if (player.Owner)
				{
					var input = game.Entities.GetComponentOf<Game.Component.Input>(entity);
					float x = UnityEngine.Input.GetAxis("Horizontal");
					float y = UnityEngine.Input.GetAxis("Vertical");
					input.Axis = new Vector2(x, y);
					input.Space = UnityEngine.Input.GetKeyDown(KeyCode.Space);

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
