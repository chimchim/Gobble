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
		LayerMask itemLayer = LayerMask.GetMask("Item");
		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);

			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);	
				var itemHolder = game.Entities.GetComponentOf<ItemHolder>(e);
				if (player.IsHost && player.Owner)
				{
					foreach (int r in entities)
					{
						Helper.CheckItemPickup(game, r);
					}
				}
				foreach (Item item in itemHolder.ActiveItems)
				{
					item.GotUpdated = false;
					item.Input(game, e, delta);
				}
				for (int k = itemHolder.ActiveItems.Count - 1; k >= 0; k--)
				{
					if (itemHolder.ActiveItems[k].Remove)
					{
						itemHolder.ActiveItems[k].Remove = false;
						itemHolder.ActiveItems[k].OwnerDeActivate(game, e);
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
				
				
				var force = new Vector2(0, 8);
				if (player.Owner)
				{
					var itemHolder = game.Entities.GetComponentOf<ItemHolder>(e);
					var inventory = game.Entities.GetComponentOf<InventoryComponent>(e);
					for (int i = 0; i < game.GameResources.AllItems.AllItemsList.Count; i++)
					{

						var scriptItem = game.GameResources.AllItems.AllItemsList[i];
						scriptItem.MakeItem = () =>
						{
							var position = game.Entities.GetEntity(e).gameObject.transform.position;
							HandleNetEventSystem.AddEvent(game, e, NetCreateItem.Make(e, scriptItem.WhatItem, position, force));
							Item toRemove = null;
							foreach (Item item in itemHolder.Items.Values)
							{
								var currentItem = item as Ingredient;
								if (currentItem != null)
								{
									for (int j = 0; j < scriptItem.IngredientsNeeded.Count; j++)
									{
										if (currentItem.IngredientType == scriptItem.IngredientsNeeded[j].Ingredient)
										{
											item.Quantity -= scriptItem.IngredientsNeeded[j].AmountNeeded;
											game.GameResources.AllItems.IngredientAmount[(int)currentItem.IngredientType] -= scriptItem.IngredientsNeeded[j].AmountNeeded;
											if (item.Quantity <= 0)
											{
												toRemove = item;
												if (itemHolder.ActiveItems.Contains(item))
												{
													item.OwnerDeActivate(game, e);
													itemHolder.Hands.OwnerActivate(game, e);
												}

												inventory.InventoryBackpack.RemoveItem(item);
												inventory.MainInventory.RemoveItem(item);
												HandleNetEventSystem.AddEventIgnoreOwner(game, e, NetDestroyItem.Make(e, item.ItemNetID));
											}
											else
											{
												inventory.InventoryBackpack.SetQuantity(item);
												inventory.MainInventory.SetQuantity(item);
											}
										}
									}
								}
							}
							if (toRemove != null)
							{
								itemHolder.Items.Remove(toRemove.ItemNetID);
								GameObject.Destroy(toRemove.CurrentGameObject);
								toRemove.Recycle();
							}
							inventory.Crafting.SetCurrent();
						};
					}
				}
			}
		}
		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
	}
}
