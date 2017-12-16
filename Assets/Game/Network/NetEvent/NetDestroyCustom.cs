using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class NetDestroyCustom : NetEvent
{
	private static ObjectPool<NetDestroyCustom> _pool = new ObjectPool<NetDestroyCustom>(10);

	public int CustomIndex;

	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}
	public override void Handle(GameManager game)
	{
		if (!game.TileMap.CustomGatherables.ContainsKey(CustomIndex))
		{
			Debug.LogError("CustomIndex DE NULL " + CustomIndex);
			return;
		}
		var custom = game.TileMap.CustomGatherables[CustomIndex];
		GameObject.Destroy(custom);

	}

	public static NetDestroyCustom Make()
	{
		return _pool.GetNext();
	}

	public static NetDestroyCustom Make(int customIndex)
	{
		var evt = _pool.GetNext();
		evt.CustomIndex = customIndex;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		CustomIndex = BitConverter.ToInt32(byteData, index); index += sizeof(int);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetDestroyCustom));
		outgoing.AddRange(BitConverter.GetBytes(8));
		outgoing.AddRange(BitConverter.GetBytes(CustomIndex));
	}
}
