using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;
using Game.Systems;

public class NetIngredientPickup : NetEvent
{
	private static ObjectPool<NetIngredientPickup> _pool = new ObjectPool<NetIngredientPickup>(10);

	public int ItemID;
	public int Player;

	public override void Handle(GameManager game)
	{
		var visibles = game.WorldItems;
		var player = game.Entities.GetComponentOf<Player>(Player);
		VisibleItem item = null;

		for (int i = visibles.Count - 1; i > -1; i--)
		{
			if (visibles[i].Item.ItemNetID == ItemID)
			{
				item = visibles[i];
				break;
			}
		}
		item.Item.CurrentGameObject.GetComponent<Collider2D>().enabled = false;
		item.enabled = false;
		if (!player.Owner)
			return;
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(Player);
		var inv = game.Entities.GetComponentOf<InventoryComponent>(Player);
		bool picked = false;
		if (!item.Item.HasSlot(inv))
		{

		}
		else
		{
			var ingredientType = (item.Item as Ingredient).IngredientType;
			picked = true;
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
			game.GameResources.AllItems.IngredientAmount[(int)ingredientType] = item.Item.Quantity;
			Ingredient.SetCraftingData(game, Player, (int)ingredientType);
			item.Item.OnPickup(game, Player, item.gameObject);
		}
		HandleNetEventSystem.AddEvent(game, Player, NetSetInSlotClient.Make(Player, ItemID, picked));
	}

	public static NetIngredientPickup Make()
	{
		return _pool.GetNext();
	}

	public static NetIngredientPickup Make(int player, int itemID)
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
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetIngredientPickup));
		outgoing.AddRange(BitConverter.GetBytes(8));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(ItemID));

	}
}
