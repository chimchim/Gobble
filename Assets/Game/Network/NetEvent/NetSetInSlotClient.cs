using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;
using Game.Systems;

public class NetSetInSlotClient : NetEvent
{
	private static ObjectPool<NetSetInSlotClient> _pool = new ObjectPool<NetSetInSlotClient>(10);

	public int ItemID;
	public int Player;
	public bool PickedUp;
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

		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(Player);

		if (!PickedUp)
		{
			item.Force = new Vector2(0, 8);
			item.enabled = true;
			item.Item.CurrentGameObject.GetComponent<Collider2D>().enabled = true;
			item.GetComponent<BoxCollider2D>().isTrigger = false;
			item.StartCoroutine(item.TriggerTime());
			return;
		}
		else if(!player.Owner)
		{
			item.enabled = true;
			itemHolder.Items.Add(ItemID, item.Item);
			Item.SetInHand(game, Player, item.Item.CurrentGameObject);
			item.Item.CurrentGameObject.SetActive(false);
			item.Item.CurrentGameObject.GetComponent<Collider2D>().enabled = false;
			item.enabled = false;
			visibles.Remove(item);
		}
	}

	public static NetSetInSlotClient Make()
	{
		return _pool.GetNext();
	}

	public static NetSetInSlotClient Make(int player, int itemID, bool pickedUp)
	{
		var evt = _pool.GetNext();
		evt.ItemID = itemID;
		evt.Player = player;
		evt.PickedUp = pickedUp;
		return evt;
	}

	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}

	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		Player = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		ItemID = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		PickedUp = BitConverter.ToBoolean(byteData, index);
		index += sizeof(bool);

	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetSetInSlotClient));
		outgoing.AddRange(BitConverter.GetBytes(9));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(ItemID));
		outgoing.AddRange(BitConverter.GetBytes(PickedUp));
	}
}
