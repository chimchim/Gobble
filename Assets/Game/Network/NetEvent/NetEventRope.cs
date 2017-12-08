using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetEventRope : NetEvent
{
	private static ObjectPool<NetEventRope> _pool = new ObjectPool<NetEventRope>(10);

	public int ItemID;
	public int Player;
	public bool Activate;

	public override void Handle(GameManager game)
	{
		var input = game.Entities.GetComponentOf<InputComponent>(Player);
		var movement = game.Entities.GetComponentOf<MovementComponent>(Player);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(Player);

		if (Activate)
		{
			resources.GraphicRope.ThrowRope(game, Player, movement, input);
		}
		else
		{
			input.RightClick = false;
			resources.GraphicRope.DeActivate();
			movement.RopeList.Clear();
			movement.CurrentState = MovementComponent.MoveState.Grounded;
		}
	}

	public static NetEventRope Make()
	{
		return _pool.GetNext();
	}

	public static NetEventRope Make(int player, int netEventID, int itemID, bool activate)
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
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetItemPickup));
		outgoing.AddRange(BitConverter.GetBytes(9));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(ItemID));
		outgoing.AddRange(BitConverter.GetBytes(Activate));
	}
}
