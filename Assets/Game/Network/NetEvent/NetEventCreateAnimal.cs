using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetEventCreateAnimal : NetEvent
{
	private static ObjectPool<NetEventCreateAnimal> _pool = new ObjectPool<NetEventCreateAnimal>(10);

	public int Animal;
	public Vector2 Position;

	public override void Handle(GameManager game)
	{
		if (Animal == 0)
		{
			game.AddAction(() =>
			{
				game.CreateRabbit(Position);
			});
		}
	}

	public static NetEventCreateAnimal Make()
	{
		return _pool.GetNext();
	}

	public static NetEventCreateAnimal Make(int animal, Vector2 positon)
	{
		var evt = _pool.GetNext();
		evt.Animal = animal;
		evt.Position = positon;
		return evt;
	}

	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}

	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		int animal = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		float posX = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		float posY = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);

		Position = new Vector2(posX, posY);
		Animal = animal;
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetCreateAnimal));
		outgoing.AddRange(BitConverter.GetBytes(12));
		outgoing.AddRange(BitConverter.GetBytes(Animal));
		outgoing.AddRange(BitConverter.GetBytes(Position.x));
		outgoing.AddRange(BitConverter.GetBytes(Position.y));
	}
}
