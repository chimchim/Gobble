using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;
using Game.Systems;

public class NetItemPickup : NetEvent
{
	private static ObjectPool<NetItemPickup> _pool = new ObjectPool<NetItemPickup>(10);

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
			picked = true;
			foreach (Item stackable in itemHolder.Items.Values)
			{
				if (stackable.TryStack(game, item.Item))
				{
					inv.InventoryBackpack.SetQuantity(stackable);
					inv.MainInventory.SetQuantity(stackable);
					picked = false;
					HandleNetEventSystem.AddEvent(game, Player, NetDestroyWorldItem.Make(item.Item.ItemNetID));
					return;
				}
			}
			item.Item.OnPickup(game, Player, item.gameObject);
		}
		HandleNetEventSystem.AddEventAndHandle(game, Player, NetSetInSlotClient.Make(Player, ItemID, picked));
	}

	public static NetItemPickup Make()
	{
		return _pool.GetNext();
	}

	public static NetItemPickup Make(int player, int itemID)
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
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetItemPickup));
		outgoing.AddRange(BitConverter.GetBytes(8));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(ItemID));

	}
}
