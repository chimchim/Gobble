using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetJump : NetEvent
{
	private static ObjectPool<NetJump> _pool = new ObjectPool<NetJump>(10);

	public int Player;

	public override void Handle(GameManager game)
	{
		var animator = game.Entities.GetEntity(Player).Animator;
		var movement = game.Entities.GetComponentOf<MovementComponent>(Player);

		movement.CurrentVelocity.y = GameUnity.JumpSpeed;
		animator.SetBool("Jump", true);
	}

	public static NetJump Make()
	{
		return _pool.GetNext();
	}

	public static NetJump Make(int player)
	{
		var evt = _pool.GetNext();
		evt.Player = player;
		return evt;
	}

	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}

	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		int player = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);

		Player = player;
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetJump));
		outgoing.AddRange(BitConverter.GetBytes(4));
		outgoing.AddRange(BitConverter.GetBytes(Player));

	}
}
