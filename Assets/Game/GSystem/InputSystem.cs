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
				if (player.Dead)
					continue;
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
					resources.FreeArm.up = -screenDirection;
					if (entity.Animator.transform.eulerAngles.y > 6)
					{
						resources.FreeArm.up = screenDirection;
						resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
					}
					bool blockedByGUI = game.RealVariables.HoveringUI || game.RealVariables.ChangingItem;
					CheckChangingItems(game, itemHolder, inventory);
					ItemChangeInput(game, e, itemHolder, input);
					input.MousePos = mousePos;
					input.ArmDirection = resources.FreeArm.up;
					input.ScreenDirection = screenDirection;
					input.Axis = new Vector2(x, y);
					input.Space = UnityEngine.Input.GetKeyDown(KeyCode.Space) || input.Space;
					input.OnRightDown = UnityEngine.Input.GetKeyDown(KeyCode.Mouse1) || input.OnRightDown;
					input.RightDown = (UnityEngine.Input.GetKey(KeyCode.Mouse1) && (!blockedByGUI || input.RightDown));
					input.LeftDown = (UnityEngine.Input.GetKey(KeyCode.Mouse0) && (!blockedByGUI || input.LeftDown));
					input.OnLeftDown = (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0) || input.OnLeftDown) && !blockedByGUI;

					input.E = UnityEngine.Input.GetKeyDown(KeyCode.E) || input.E;

					if (Input.GetKeyDown(KeyCode.Mouse2) && player.IsHost)
					{
						game.AddAction(() =>
						{
							HandleNetEventSystem.AddEvent(game, e, NetEventCreateAnimal.Make(0, mousePos));
						});

						//game.CreateFullPlayer(false, "adw", false, 1, 0, Characters.Yolanda);
						//HandleNetEventSystem.AddEvent(game, e, NetEventCreateAnimal.Make(0, mousePos));

					}
					if (Input.GetKeyDown(KeyCode.C))
					{
						inventory.Crafting.gameObject.SetActive(!craftingActive);
						craftingActive = !craftingActive;
						if (!craftingActive)
							game.RealVariables.HoveringUI = false;
					}
				}
				else
				{
					resources.FreeArm.up = input.ArmDirection;
				}
			}
		}
		private void CheckChangingItems(GameManager game, ItemHolder holder, InventoryComponent inv)
		{
			bool changing = game.RealVariables.ChangingItem;

			if (changing && UnityEngine.Input.GetKeyUp(KeyCode.Mouse0))
			{
				GameUnity.FollowMouse.gameObject.SetActive(false);
				game.RealVariables.ChangingItem = false;
				var currentItem = inv.MainInventory.GetItem(inv.CurrentItemIndex);
				var switch2 = game.RealVariables.CurrentSwitch2;
				var switch1 = game.RealVariables.CurrentSwitch;
				if (switch2 == null)
				{
					Debug.Log("Switch2 NULL");
					switch1.Item.ThrowItem(game, holder.EntityID);
					if (currentItem == game.RealVariables.CurrentSwitch.Item)
					{
						holder.Hands.OwnerActivate(game, holder.EntityID);
					}
				}
				else
				{
					if (currentItem == switch1.Item)
					{
						Debug.Log("currentItem switch 1 " + switch1.Item);
						if (switch2.Item != null)
							switch2.Item.OwnerActivate(game, holder.EntityID);
						currentItem.OwnerDeActivate(game, holder.EntityID);
					}
					if (currentItem == switch2.Item)
					{
						Debug.Log("currentItem switch 2 " + switch1.Item);
						switch1.Item.OwnerActivate(game, holder.EntityID);
						if(currentItem != null)
							currentItem.OwnerDeActivate(game, holder.EntityID);
					}
					var sp2 = switch2.Image.sprite;
					switch2.SetImage(switch1.Image.sprite);
					if (sp2 != null)
						switch1.SetImage(sp2);
					else
						switch1.UnsetImage();

					if(switch2.Item != null)
						switch1.SetQuantity(switch2.Item.Quantity);
					if (switch1.Item != null)
						switch2.SetQuantity(switch1.Item.Quantity);

					var i = switch2.Item;
					switch2.Item = switch1.Item;
					switch1.Item = i;

				}

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
