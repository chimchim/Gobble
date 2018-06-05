using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;
using Game.Movement;
using MoveState = Game.Component.MovementComponent.MoveState;

public class NetEventSpawnPlayer : NetEvent
{
	private static ObjectPool<NetEventSpawnPlayer> _pool = new ObjectPool<NetEventSpawnPlayer>(10);

	public int Player;
	public override void Handle(GameManager game)
	{
		var entity = game.Entities.GetEntity(Player);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(Player);
		var movecomp = game.Entities.GetComponentOf<MovementComponent>(Player);
		var player = game.Entities.GetComponentOf<Player>(Player);
		player.Health = 100;
		entity.gameObject.transform.position = player.Base.position + Vector3.up;
		entity.gameObject.layer = ((Grounded)movecomp.States[(int)MoveState.Grounded]).PlayerLayer;
		resources.LerpCharacter.gameObject.SetActive(true);
	}

	public static NetEventSpawnPlayer Make()
	{
		return _pool.GetNext();
	}

	public static NetEventSpawnPlayer Make(int player)
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
		Player = BitConverter.ToInt32(byteData, index); index += sizeof(int);

	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetEventSpawnPlayer));
		outgoing.AddRange(BitConverter.GetBytes(4));
		outgoing.AddRange(BitConverter.GetBytes(Player));
	}
}
