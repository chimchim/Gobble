using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetDestroyWorldItem : NetEvent
{
	private static ObjectPool<NetDestroyWorldItem> _pool = new ObjectPool<NetDestroyWorldItem>(10);
	public int ItemID;
	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}
	public override void Handle(GameManager game)
	{
		var visibles = game.WorldItems;

		for (int i = visibles.Count - 1; i > -1; i--)
		{
			if (visibles[i].Item.ItemNetID == ItemID)
			{
				GameObject.Destroy(visibles[i].Item.CurrentGameObject);
				visibles[i].Item.Recycle();
				visibles.RemoveAt(i);
				break;
			}
		}
	}

	public static NetDestroyWorldItem Make()
	{
		return _pool.GetNext();
	}

	public static NetDestroyWorldItem Make(int itemID)
	{
		var evt = _pool.GetNext();
		evt.ItemID = itemID;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		int netID = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);

		ItemID = netID;
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetDestroyWorldItem));
		outgoing.AddRange(BitConverter.GetBytes(4));
		outgoing.AddRange(BitConverter.GetBytes(ItemID));
	}
}
