using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetCreateBase : NetEvent
{
	private static ObjectPool<NetCreateBase> _pool = new ObjectPool<NetCreateBase>(10);

	public Vector2 Position;
	public int Player;
	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}
	public override void Handle(GameManager game)
	{
		var baseBase = GameObject.Instantiate(game.GameResources.AllItems.Base.Prefab).transform;
		baseBase.transform.position = new Vector3(Position.x, Position.y, -0.15f);
		baseBase.gameObject.layer = LayerMask.NameToLayer("Default");
		baseBase.transform.localScale = new Vector3(0.5f, 0.5f, 1);

		var players = game.Entities.GetEntitiesWithComponents(Bitmask.MakeFromComponents<Player>());
		var playerCreator = game.Entities.GetComponentOf<Player>(Player);
		playerCreator.Base = baseBase;
		foreach (int p in players)
		{
			var player = game.Entities.GetComponentOf<Player>(p);
			if (player.Team == playerCreator.Team)
				player.Base = baseBase;
		}
	}

	public static NetCreateBase Make()
	{
		return _pool.GetNext();
	}

	public static NetCreateBase Make(int player, Vector2 position)
	{
		var evt = _pool.GetNext();
		evt.Position = position;
		evt.Player = player;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		Player = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		float posX = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		float posY = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		Position = new Vector2(posX, posY);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetCreateBase));
		outgoing.AddRange(BitConverter.GetBytes(12));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(Position.x));
		outgoing.AddRange(BitConverter.GetBytes(Position.y));

	}
}
