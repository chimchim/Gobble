using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetItemPickup : NetEvent
{
	private static ObjectPool<NetItemPickup> _pool = new ObjectPool<NetItemPickup>(10);

	public int ItemID;
	public int Player;

	public override void Handle(GameManager game)
	{
		var visibles = game.WorldItems;
		
		for (int i = visibles.Count - 1; i > -1; i--)
		{
			if (visibles[i].Item.ItemNetID == ItemID)
			{
				var itemHolder = game.Entities.GetComponentOf<ItemHolder>(Player);
				itemHolder.Items.Add(ItemID, visibles[i].Item);
				Item.SetInHand(game, Player, visibles[i].Item.CurrentGameObject);
				visibles[i].Item.CurrentGameObject.SetActive(false);
				visibles[i].Item.CurrentGameObject.GetComponent<Collider2D>().enabled = false;
				visibles[i].enabled = false;
				visibles.RemoveAt(i);
				break;
			}
		}
	}

	public static NetItemPickup Make()
	{
		return _pool.GetNext();
	}

	public static NetItemPickup Make(int player, int netEventID, int itemID)
	{
		var evt = _pool.GetNext();
		evt.NetEventID = netEventID;
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
