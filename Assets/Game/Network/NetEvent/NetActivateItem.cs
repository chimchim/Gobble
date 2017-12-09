using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetActivateItem : NetEvent
{
	private static ObjectPool<NetActivateItem> _pool = new ObjectPool<NetActivateItem>(10);

	public int ItemID;
	public int Player;
	public bool Activate;

	public override void Handle(GameManager game)
	{
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(Player);
		//if (!Activate)
		//{
		//	foreach (Item item in itemHolder.ActiveItems.Values)
		//	{
		//		item.DeActivate(game, Player);
		//	}
		//	return;
		//}
		//foreach (Item item in itemHolder.Items)
		//{
		//	if (item.ItemNetID == ItemID)
		//	{
		//		if (Activate)
		//		{
		//			for (int j = 0; j < itemHolder.ActiveItems.Count; j++)
		//			{
		//				itemHolder.ActiveItems[j].DeActivate(game, Player);
		//			}
		//			item.Activate(game, Player);
		//		}
		//	}
		//}
	}

	public static NetActivateItem Make()
	{
		return _pool.GetNext();
	}

	public static NetActivateItem Make(int player, int netEventID, int itemID, bool activate)
	{
		var evt = _pool.GetNext();
		evt.NetEventID = netEventID;
		evt.ItemID = itemID;
		evt.Player = player;
		evt.Activate = activate;
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
		bool activate = BitConverter.ToBoolean(byteData, index);
		index += sizeof(bool);

		Player = player;
		ItemID = itemID;
		Activate = activate;
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetActivateItem));
		outgoing.AddRange(BitConverter.GetBytes(9));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(ItemID));
		outgoing.AddRange(BitConverter.GetBytes(Activate));
	}
}
