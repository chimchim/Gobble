using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetHitItem : NetEvent
{
	private static ObjectPool<NetHitItem> _pool = new ObjectPool<NetHitItem>(10);

	public int ItemID;
	public int Player;
	public float Damage;
	public Vector2 EffectPosition;
	public Effects Effect;
	public override void Handle(GameManager game)
	{
		game.CreateEffect(Effect, EffectPosition, 0.5f);
		var player = game.Entities.GetComponentOf<Player>(Player);
		if (!player.Owner)
			return;

		var itemholder = game.Entities.GetComponentOf<ItemHolder>(Player);
		if (itemholder.Items.ContainsKey(ItemID))
		{
			var item = itemholder.Items[ItemID];
			if(item.ScrItem.MaxHp > 0)
				item.DoDamage(game, Damage, Player);
			return;
		}
		Debug.Log("DIDT CONTAIN " + ItemID);

	}

	public static NetHitItem Make()
	{
		return _pool.GetNext();
	}

	public static NetHitItem Make(int player, int itemID, float damage, Effects effect, Vector2 effectPosition)
	{
		var evt = _pool.GetNext();
		evt.ItemID = itemID;
		evt.Player = player;
		evt.Damage = damage;
		evt.EffectPosition = effectPosition;
		evt.Effect = effect;
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
		ItemID = BitConverter.ToInt32(byteData, index); index += sizeof(int);
		Damage = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float x = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float y = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		Effect = (Effects)BitConverter.ToInt32(byteData, index); index += sizeof(int);
		EffectPosition = new Vector2(x, y);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetHitItem));
		outgoing.AddRange(BitConverter.GetBytes(24));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(ItemID));
		outgoing.AddRange(BitConverter.GetBytes(Damage));
		outgoing.AddRange(BitConverter.GetBytes(EffectPosition.x));
		outgoing.AddRange(BitConverter.GetBytes(EffectPosition.y));
		outgoing.AddRange(BitConverter.GetBytes((int)Effect));
	}
}
