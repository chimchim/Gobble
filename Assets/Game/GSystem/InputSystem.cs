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
		bool craftingActive = true;
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<InputComponent, Player, ActionQueue>();
		private KeyCode[] AlphaKeys = new KeyCode[]
			{
				KeyCode.Alpha1,
				KeyCode.Alpha2,
				KeyCode.Alpha3
			};
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
					var inventory = game.Entities.GetComponentOf<InventoryComponent>(e);
					float x = UnityEngine.Input.GetAxis("Horizontal");
					float y = UnityEngine.Input.GetAxis("Vertical");
					Vector2 mousePos = UnityEngine.Input.mousePosition;
					mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
					
					Vector2 middleScreen = new Vector2(Screen.width / 2, Screen.height / 2);
					Vector2 screenDirection = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y) - middleScreen;
					screenDirection.Normalize();
					//resources.FreeArm.up = -screenDirection;
					//if (entity.Animator.transform.eulerAngles.y > 6)
					//{
					//	resources.FreeArm.up = screenDirection;
					//	resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
					//}
					ItemChangeInput(game, e, itemHolder, input);
					input.MousePos = mousePos;
					input.ArmDirection = resources.FreeArm.up;
					input.ScreenDirection = screenDirection;
					input.Axis = new Vector2(x, y);
					input.Space = UnityEngine.Input.GetKeyDown(KeyCode.Space) || input.Space;
					input.RightClick = UnityEngine.Input.GetKeyDown(KeyCode.Mouse1) || input.RightClick;
					input.LeftDown = (UnityEngine.Input.GetKey(KeyCode.Mouse0) && !inventory.Crafting.Hovering);
					input.OnLeftDown = (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0) || input.OnLeftDown) && !inventory.Crafting.Hovering;
					input.E = UnityEngine.Input.GetKeyDown(KeyCode.E) || input.E;

					if (Input.GetKeyDown(KeyCode.Mouse2) && player.IsHost)
					{
						game.CallBacks.Add(() =>
						{
							game.CreateFullPlayer(false, "adw", false, 1, 0, Characters.Yolanda);
						});
						//game.CreateFullPlayer(false, "adw", false, 1, 0, Characters.Yolanda);
						//HandleNetEventSystem.AddEvent(game, e, NetEventCreateAnimal.Make(0, mousePos));

					}
					if (Input.GetKeyDown(KeyCode.C))
					{
						inventory.Crafting.gameObject.SetActive(!craftingActive);
						craftingActive = !craftingActive;
						if (!craftingActive)
							inventory.Crafting.Hovering = false;
					}
				}
				//else
				//{
				//	resources.FreeArm.up = input.ArmDirection;
				//}
			}
		}

		public void ItemChangeInput(GameManager game, int entity, ItemHolder itemHolder, InputComponent input)
		{
			var inventory = game.Entities.GetComponentOf<InventoryComponent>(entity);
			var holder = game.Entities.GetComponentOf<ItemHolder>(entity);
			var maininv = inventory.MainInventory;

			if (Input.GetKeyDown(KeyCode.Q))
			{
				var item = maininv.GetItem(inventory.CurrentItemIndex);
				if (item != null)
				{
					item.ThrowItem(game, entity);
					holder.Hands.OwnerActivate(game, entity);
				}
			}

			for (int i = 0; i < AlphaKeys.Length; i++)
			{
				if (Input.GetKeyDown(AlphaKeys[i]))
				{
					for (int k = itemHolder.ActiveItems.Count - 1; k >= 0; k--)
					{
						itemHolder.ActiveItems[k].OwnerDeActivate(game, entity);	
					}
					maininv.ResetAll();
					maininv.SetChoosen(i);
					var item = maininv.GetItem(i);
					inventory.CurrentItemIndex = i;

					if (item != null)
					{
						holder.Hands.OwnerDeActivate(game, entity);
						item.SetChoosenSlot(game, entity);
						item.OwnerActivate(game, entity);
					}
					else
					{
						holder.Hands.OwnerActivate(game, entity);
					}
				}
			}
		}

		public void Initiate(GameManager game)
		{
			 var entities = game.Entities.GetEntitiesWithComponents(_bitmask);

			 foreach (int e in entities)
			 {
				 var player = game.Entities.GetComponentOf<Player>(e);
				 var inventory = game.Entities.GetComponentOf<InventoryComponent>(e);

				 if (player.Owner)
				 {
					 inventory.Crafting.gameObject.SetActive(craftingActive);
				 }
			 }
		}
        public void SendMessage(GameManager game, int reciever, Message message)
        {

        }
	}
}
