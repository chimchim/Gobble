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
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<InputComponent, Player, ActionQueue>();

		public void Update(GameManager game, float delta)
		{
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			
			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);
				var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
				var input = game.Entities.GetComponentOf<InputComponent>(e);
				var itemHolder = game.Entities.GetComponentOf<ItemHolder>(e);
				var entityTransform = game.Entities.GetEntity(e).gameObject.transform;
				var entity = game.Entities.GetEntity(e);
				if (player.Owner)
				{	
					var movement = game.Entities.GetComponentOf<MovementComponent>(e);
					
					float x = UnityEngine.Input.GetAxis("Horizontal");
					float y = UnityEngine.Input.GetAxis("Vertical");
					Vector2 mousePos = UnityEngine.Input.mousePosition;
					mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

					input.MousePos = mousePos;
					input.Axis = new Vector2(x, y);
					input.Space = UnityEngine.Input.GetKeyDown(KeyCode.Space) || input.Space;
					input.RightClick = UnityEngine.Input.GetKeyDown(KeyCode.Mouse1) || input.RightClick;

					foreach (Item item in itemHolder.Items)
					{
						if (!item.Active)
							continue;
						item.Input(game, e);
					}
				}

				bool dont = (entity.Animator.transform.eulerAngles.y > 6) && (input.MousePos.x > entityTransform.position.x);
				dont = (entity.Animator.transform.eulerAngles.y < 6) && (input.MousePos.x < entityTransform.position.x) || dont;
				if (dont)
					continue;
				Vector2 direction = (input.MousePos - new Vector2(entityTransform.position.x, entityTransform.position.y)).normalized;
				resources.FreeArm.up = -direction;
				if (entity.Animator.transform.eulerAngles.y > 6)
				{
					resources.FreeArm.up = direction;
					resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
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
