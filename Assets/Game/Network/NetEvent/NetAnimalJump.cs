using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;

public class NetAnimalJump : NetEvent
{
	private static ObjectPool<NetAnimalJump> _pool = new ObjectPool<NetAnimalJump>(10);

	public int AnimalID;
	public int JumpValue;
	public override void Handle(GameManager game)
	{
		var animator = game.Entities.GetEntity(AnimalID).Animator;
		var animal = game.Entities.GetComponentOf<Animal>(AnimalID);
		animal.CurrentVelocity.y = JumpValue;
		animator.SetBool("Jump", true);
		animator.SetBool("Walk", false);
	}

	public static NetAnimalJump Make()
	{
		return _pool.GetNext();
	}

	public static NetAnimalJump Make(int id, int jumpValue)
	{
		var evt = _pool.GetNext();
		evt.AnimalID = id;
		evt.JumpValue = jumpValue;
		return evt;
	}

	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}

	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		AnimalID = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		JumpValue = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetAnimalJump));
		outgoing.AddRange(BitConverter.GetBytes(8));
		outgoing.AddRange(BitConverter.GetBytes(AnimalID));
		outgoing.AddRange(BitConverter.GetBytes(JumpValue));
	}
}
