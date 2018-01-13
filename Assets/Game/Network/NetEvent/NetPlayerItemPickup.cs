using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;
using Game.Systems;

public class NetPlayerItemPickup : NetEvent
{
	private static ObjectPool<NetPlayerItemPickup> _pool = new ObjectPool<NetPlayerItemPickup>(10);

	public int ItemID;
	public int Player;

	public override void Handle(GameManager game)
	{
		var visibles = game.WorldItems;
		var player = game.Entities.GetComponentOf<Player>(Player);
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(Player);
		VisibleItem item = null;
		Debug.Log("handle Netplayer pick up 1");
		for (int i = visibles.Count - 1; i > -1; i--)
		{
			if (visibles[i].Item.ItemNetID == ItemID)
			{
				item = visibles[i];
				break;
			}
		}
		if (!player.Owner)
		{
			itemHolder.Items.Add(ItemID, item.Item);
			Item.SetInHand(game, Player, item.Item.CurrentGameObject);
			item.Item.CurrentGameObject.SetActive(false);
			item.Item.CurrentGameObject.GetComponent<Collider2D>().enabled = false;
			item.enabled = false;
			visibles.Remove(item);
		}
		else
		{
			Debug.Log("handle Netplayer pick up 2");
			var inv = game.Entities.GetComponentOf<InventoryComponent>(Player);
			var ingredientType = (item.Item as Ingredient).IngredientType;
			foreach (Item stackable in itemHolder.Items.Values)
			{
				if (stackable.TryStack(game, item.Item))
				{
					Ingredient.SetCraftingData(game, Player, (int)ingredientType);
					inv.InventoryBackpack.SetQuantity(stackable);
					inv.MainInventory.SetQuantity(stackable);
					HandleNetEventSystem.AddEvent(game, Player, NetDestroyWorldItem.Make(item.Item.ItemNetID));
					return;
				}
			}
			if (!item.Item.HasSlot(inv))
				return;
			game.GameResources.AllItems.IngredientAmount[(int)ingredientType] = item.Item.Quantity;
			Ingredient.SetCraftingData(game, Player, (int)ingredientType);
			item.Item.OnPickup(game, Player, item.gameObject);
			item.enabled = false;
		}
	}

	public static NetPlayerItemPickup Make()
	{
		return _pool.GetNext();
	}

	public static NetPlayerItemPickup Make(int player, int itemID)
	{
		var evt = _pool.GetNext();
		evt.ItemID = itemID;
		evt.Player = player;
		return evt;
	}

	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}

	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		int player = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		int itemID = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);

		Player = player;
		ItemID = itemID;
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetPlayerItemPickup));
		outgoing.AddRange(BitConverter.GetBytes(8));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(ItemID));

	}
}
