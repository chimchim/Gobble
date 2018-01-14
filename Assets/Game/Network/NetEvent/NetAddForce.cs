using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetAddForce : NetEvent
{
	private static ObjectPool<NetAddForce> _pool = new ObjectPool<NetAddForce>(10);

	public int Player;
	public Vector2 Force;
	public override void Handle(GameManager game)
	{
		var movement = game.Entities.GetComponentOf<MovementComponent>(Player);
		movement.ForceVelocity += Force;
	}

	public static NetAddForce Make()
	{
		return _pool.GetNext();
	}

	public static NetAddForce Make(int player, Vector2 force)
	{
		var evt = _pool.GetNext();
		evt.Player = player;
		evt.Force = force;
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

		var x = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		var y = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		Force = new Vector2(x, y);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetAddForce));
		outgoing.AddRange(BitConverter.GetBytes(4));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(Force.x));
		outgoing.AddRange(BitConverter.GetBytes(Force.y));
	}
}
