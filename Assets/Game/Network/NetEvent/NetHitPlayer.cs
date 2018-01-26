using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;
using Game.Systems;

public class NetHitPlayer : NetEvent
{
	private static ObjectPool<NetHitPlayer> _pool = new ObjectPool<NetHitPlayer>(10);

	public int Player;
	public float Damage;
	public Vector2 EffectOffset;
	public E.Effects Effect;
	public override void Handle(GameManager game)
	{
		var entity = game.Entities.GetEntity(Player);
		Vector3 pos = entity.gameObject.transform.position + new Vector3(EffectOffset.x, EffectOffset.y, 0.2f);
		game.CreateEffect(Effect, pos, 0.5f);
		var player = game.Entities.GetComponentOf<Player>(Player);
		if (!player.Owner)
			return;

		player.Health -= Damage;
		if (player.Health <= 0)
		{
			game.AddAction(() =>
			{
				HandleNetEventSystem.AddEventAndHandle(game, Player, NetPlayerDeath.Make(Player));
			});
		}
		Debug.Log("player.Health " + player.Health);

	}

	public static NetHitPlayer Make()
	{
		return _pool.GetNext();
	}

	public static NetHitPlayer Make(int player, float damage, E.Effects effect, Vector2 effectOffset)
	{
		var evt = _pool.GetNext();
		evt.Player = player;
		evt.Damage = damage;
		evt.EffectOffset = effectOffset;
		evt.Effect = effect;
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
		Damage = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float x = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float y = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		Effect = (E.Effects)BitConverter.ToInt32(byteData, index); index += sizeof(int);
		EffectOffset = new Vector2(x, y);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetHitPlayer));
		outgoing.AddRange(BitConverter.GetBytes(20));
		outgoing.AddRange(BitConverter.GetBytes(Player));
		outgoing.AddRange(BitConverter.GetBytes(Damage));
		outgoing.AddRange(BitConverter.GetBytes(EffectOffset.x));
		outgoing.AddRange(BitConverter.GetBytes(EffectOffset.y));
		outgoing.AddRange(BitConverter.GetBytes((int)Effect));
	}
}
