using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class NetCreateBase : NetEvent
{
	private static ObjectPool<NetCreateBase> _pool = new ObjectPool<NetCreateBase>(10);

	public Vector2 Position;
	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}
	public override void Handle(GameManager game)
	{
		var ladder = GameObject.Instantiate(game.GameResources.Prefabs.Ladder).transform;
		ladder.transform.position = new Vector3(Position.x, Position.y, -0.2f);
		ladder.gameObject.layer = LayerMask.NameToLayer("Ladder");
	}

	public static NetCreateBase Make()
	{
		return _pool.GetNext();
	}

	public static NetCreateBase Make(Vector2 position)
	{
		var evt = _pool.GetNext();
		evt.Position = position;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		float posX = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		float posY = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		Position = new Vector2(posX, posY);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetCreateBase));
		outgoing.AddRange(BitConverter.GetBytes(8));
		outgoing.AddRange(BitConverter.GetBytes(Position.x));
		outgoing.AddRange(BitConverter.GetBytes(Position.y));

	}
}
