﻿using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetItemPickup : BaseNetEvent<NetItemPickup>
{
	public int ItemID;
	public int Player;

	public override void Handle(GameManager game)
	{
		var player = game.Entities.GetComponentOf<Player>(Player);
		if (player.Owner)
			return;
		var visibles = game.WorldItems;
		//Debug.Log("Handle " + ItemID);
		for (int i = visibles.Count - 1; i > -1; i--)
		{
			if (visibles[i].Item.ItemNetID == ItemID)
			{
				if (visibles[i].Item.CurrentGameObject == null)
					Debug.Log("Gameobj null");
				var itemHolder = game.Entities.GetComponentOf<ItemHolder>(Player);
				itemHolder.Items.Add(visibles[i].Item);
				Item.SetInHand(game, Player, visibles[i].Item.CurrentGameObject);
				visibles[i].Item.CurrentGameObject.SetActive(true);
				visibles[i].enabled = false;
				//Debug.Log("Pick up ITem id " + ItemID);
				visibles.RemoveAt(i);
				break;
			}
		}
	}


	public static NetItemPickup Make(int player, int netEventID, int itemID)
	{
		var evt = GetNext();
		evt.NetEventID = netEventID;
		evt.ItemID = itemID;
		evt.Player = player;
		return evt;
	}

	protected override void Reset()
	{
		base.Reset();
	}

	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		int netEventID = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		int player = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		int itemID = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		Debug.Log("netEventID " + netEventID + " itemID " + itemID);
		NetEventID = netEventID;
		Player = player;
		ItemID = itemID;
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes(12));
		outgoing.AddRange(BitConverter.GetBytes(NetEventID));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(ItemID));

	}
}
