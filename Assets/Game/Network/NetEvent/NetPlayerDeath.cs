using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;
using Game.Systems;

public class NetPlayerDeath : NetEvent
{
	private static ObjectPool<NetPlayerDeath> _pool = new ObjectPool<NetPlayerDeath>(10);

	public int Player;
	public override void Handle(GameManager game)
	{
		var entity = game.Entities.GetEntity(Player);
		var effect = game.CreateEffect(Effects.Death, entity.gameObject.transform.position, GameUnity.DeathTimer);
		effect.transform.parent = entity.gameObject.transform;
		var input = game.Entities.GetComponentOf<InputComponent>(Player);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(Player);
		var move = game.Entities.GetComponentOf<MovementComponent>(Player);
		move.CurrentState = MovementComponent.MoveState.Grounded;
		input.Axis = Vector2.zero;
		input.LeftDown = false;
		var armAnim = resources.FreeArmAnimator;
		for (int i = 0; i < armAnim.parameterCount; i++)
		{
			string name = armAnim.GetParameter(i).name;
			armAnim.SetBool(name, false);
		}
		for (int i = 0; i < entity.Animator.parameterCount; i++)
		{
			string name = entity.Animator.GetParameter(i).name;
			entity.Animator.SetBool(name, false);
		}
		
		
		entity.gameObject.layer = LayerMask.NameToLayer("Dead");
		resources.LerpCharacter.gameObject.SetActive(false);

		var player = game.Entities.GetComponentOf<Player>(Player);
		if (player.Owner)
		{
			game.AddAction(() =>
			{
				HandleNetEventSystem.AddEventAndHandle(game, Player, NetEventSpawnPlayer.Make(Player));
			}, GameUnity.DeathTimer);
		}
	}

	public static NetPlayerDeath Make()
	{
		return _pool.GetNext();
	}

	public static NetPlayerDeath Make(int player)
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
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetPlayerDeath));
		outgoing.AddRange(BitConverter.GetBytes(4));
		outgoing.AddRange(BitConverter.GetBytes(Player));
	}
}
