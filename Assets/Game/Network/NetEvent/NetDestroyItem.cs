using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetDestroyItem : NetEvent
{
	private static ObjectPool<NetDestroyItem> _pool = new ObjectPool<NetDestroyItem>(10);
	public int Player;
	public int ItemNetID;
	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}
	public override void Handle(GameManager game)
	{
		var inventoryMain = game.Entities.GetComponentOf<InventoryComponent>(Player);
		var holder = game.Entities.GetComponentOf<ItemHolder>(Player);
		var item = holder.Items[ItemNetID];
		holder.Items.Remove(ItemNetID);
		item.ClientDeActivate(game, Player);
		GameObject.Destroy(item.CurrentGameObject);
		Recycle();
	}

	public static NetDestroyItem Make()
	{
		return _pool.GetNext();
	}

	public static NetDestroyItem Make(int player, int itemNetID, int netEventID)
	{
		var evt = _pool.GetNext();
		evt.NetEventID = netEventID;
		evt.Player = player;
		evt.ItemNetID = itemNetID;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		int netID = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		int player = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);

		ItemNetID = netID;
		Player = player;
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetDestroyItem));
		outgoing.AddRange(BitConverter.GetBytes(8));
		outgoing.AddRange(BitConverter.GetBytes(ItemNetID));
		outgoing.AddRange(BitConverter.GetBytes(Player));
	}
}
