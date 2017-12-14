using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class NetDestroyCube : NetEvent
{
	private static ObjectPool<NetDestroyCube> _pool = new ObjectPool<NetDestroyCube>(10);

	public int X;
	public int Y;

	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}
	public override void Handle(GameManager game)
	{
		if (game.TileMap.Blocks[X, Y] == null)
		{
			Debug.LogError("CUBE DE NULL " + X + " y " + Y);
			return;
		}
		var cube = game.TileMap.Blocks[X, Y];
		var go = GameObject.Instantiate(game.GameResources.Prefabs.Poof);
		go.transform.position = cube.transform.position;

		GameObject.Destroy(go, 0.8f);
		GameObject.Destroy(cube);

	}

	public static NetDestroyCube Make()
	{
		return _pool.GetNext();
	}

	public static NetDestroyCube Make(int x, int y)
	{
		var evt = _pool.GetNext();
		evt.X = x;
		evt.Y = y;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		X = BitConverter.ToInt32(byteData, index); index += sizeof(int);
		Y = BitConverter.ToInt32(byteData, index); index += sizeof(int);

	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetDestroyCube));
		outgoing.AddRange(BitConverter.GetBytes(8));
		outgoing.AddRange(BitConverter.GetBytes(X));
		outgoing.AddRange(BitConverter.GetBytes(Y));
	}
}
