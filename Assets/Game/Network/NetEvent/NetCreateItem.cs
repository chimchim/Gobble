﻿using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class NetCreateItem : NetEvent
{
	private static ObjectPool<NetCreateItem> _pool = new ObjectPool<NetCreateItem>(10);

	public Item.ItemID ItemID;
	public Vector2 Force;
	public Vector2 Position;
	public int Creator;
	public int ItemTier;
	public float Health;
	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
    }
	public override void Handle(GameManager game)
	{
		int itemNetID = (Creator * 200000) + NetEventID;
		VisibleItem visible = null;
		if (ItemID == Item.ItemID.Pickaxe)
		{
			visible = PickAxe.MakeItem(game, Position, Force);
		}
		if (ItemID == Item.ItemID.Rope)
		{
			visible = Rope.MakeItem(game, Position, Force);
		}
		if (ItemID == Item.ItemID.Ladder)
		{
			visible = Ladder.MakeItem(game, Position, Force);
		}
		if (ItemID == Item.ItemID.Shield)
		{
			visible = Shield.MakeItem(game, Position, Force);
		}
		if (ItemID == Item.ItemID.Sword)
		{
			visible = Sword.MakeItem(game, Position, Force);
		}
		if (ItemID == Item.ItemID.Spear)
		{
			visible = Spear.MakeItem(game, Position, Force, ItemTier);
		}
		if (ItemID == Item.ItemID.Base)
		{
			visible = Base.MakeItem(game, Position, Force);
		}
		visible.Item.Health = Health;
		visible.StartCoroutine(visible.TriggerTime());
		visible.Item.ItemNetID = itemNetID;
		visible.Item.CurrentGameObject = visible.gameObject;
		game.WorldItems.Add(visible);
	}

	public static NetCreateItem Make()
	{
		return _pool.GetNext();
	}

    public static NetCreateItem Make(int creator, Item.ItemID itemID, Vector3 position, Vector2 force, int itemTier = 0, float health = 0)
	{
		var evt = _pool.GetNext();
		evt.ItemID = itemID;
		evt.Force = force;
		evt.Position = position;
		evt.Creator = creator;
		evt.ItemTier = itemTier;
		evt.Health = health;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		int id = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		Creator = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		float posX = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		float posY = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		float forceX = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		float forceY = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		ItemTier = BitConverter.ToInt32(byteData, index); index += sizeof(int);
		Health = BitConverter.ToSingle(byteData, index); index += sizeof(float);

		ItemID = (Item.ItemID)id;
		Position = new Vector2(posX, posY);
		Force = new Vector2(forceX, forceY);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetCreateItem));
		outgoing.AddRange(BitConverter.GetBytes(32));
		outgoing.AddRange(BitConverter.GetBytes((int)ItemID));
		outgoing.AddRange(BitConverter.GetBytes(Creator));
		outgoing.AddRange(BitConverter.GetBytes(Position.x));
		outgoing.AddRange(BitConverter.GetBytes(Position.y));
		outgoing.AddRange(BitConverter.GetBytes(Force.x));
		outgoing.AddRange(BitConverter.GetBytes(Force.y));
		outgoing.AddRange(BitConverter.GetBytes((int)ItemTier));
		outgoing.AddRange(BitConverter.GetBytes(Health));
	}
}
