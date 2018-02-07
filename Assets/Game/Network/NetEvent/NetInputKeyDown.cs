using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using IOnMouseRight = Item.IOnMouseRight;
using IOnMouseLeft = Item.IOnMouseLeft;
using UnityEngine;
using Game.Component;

public class NetInputKeyDown : NetEvent
{
	private static ObjectPool<NetInputKeyDown> _pool = new ObjectPool<NetInputKeyDown>(10);

	public int Player;
	public KeyCode Key;
	public override void Handle(GameManager game)
	{
		var space = (Key == KeyCode.Space);
		var onLeftDown = (Key == KeyCode.Mouse0);
		var onRightDown = (Key == KeyCode.Mouse1);

		var holder = game.Entities.GetComponentOf<ItemHolder>(Player);
		foreach (Item item in holder.ActiveItems)
		{
			IOnMouseLeft mouseLeft = item as IOnMouseLeft;
			if (mouseLeft != null && onLeftDown) mouseLeft.OnMouseLeft(game, Player);
			IOnMouseRight mouseRight = item as IOnMouseRight;
			if (mouseRight != null && onRightDown) mouseRight.OnMouseRight(game, Player);
		}
	}

	public static NetInputKeyDown Make()
	{
		return _pool.GetNext();
	}

	public static NetInputKeyDown Make(int player, KeyCode key)
	{
		var evt = _pool.GetNext();
		evt.Player = player;
		evt.Key = key;
		return evt;
	}

	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}

	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		Player = BitConverter.ToInt32(byteData, index); index += sizeof(int);
		Key = (KeyCode)BitConverter.ToInt32(byteData, index); index += sizeof(int);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetInputKeyDown));
		outgoing.AddRange(BitConverter.GetBytes(8));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes((int)Key));
	}
}
