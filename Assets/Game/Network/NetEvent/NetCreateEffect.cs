using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class NetCreateEffect : NetEvent
{
	private static ObjectPool<NetCreateEffect> _pool = new ObjectPool<NetCreateEffect>(10);

	public Effects Effect;
	public Vector2 Position;
	public Vector2 LookDirection;

	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}
	public override void Handle(GameManager game)
	{
		game.CreateEffect(Effect, Position, 0.5f);
	}

	public static NetCreateEffect Make()
	{
		return _pool.GetNext();
	}

	public static NetCreateEffect Make(Effects effect, Vector2 pos, Vector2 dir)
	{
		var evt = _pool.GetNext();
		evt.Effect = effect;
		evt.Position = pos;
		evt.LookDirection = dir;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		Effect = (Effects)BitConverter.ToInt32(byteData, index); index += sizeof(int);
		float posX = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float posY = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float dirX = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float dirY = BitConverter.ToSingle(byteData, index); index += sizeof(float);

		Position = new Vector2(posX, posY);
		LookDirection = new Vector2(dirX, dirY);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetCreateEffect));
		outgoing.AddRange(BitConverter.GetBytes(20));
		outgoing.AddRange(BitConverter.GetBytes((int)Effect));
		outgoing.AddRange(BitConverter.GetBytes(Position.x));
		outgoing.AddRange(BitConverter.GetBytes(Position.y));
		outgoing.AddRange(BitConverter.GetBytes(LookDirection.x));
		outgoing.AddRange(BitConverter.GetBytes(LookDirection.y));
	}
}
