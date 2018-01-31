using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Game.Component;
using Game.Systems;

public class NetHitAnimal : NetEvent
{
	private static ObjectPool<NetHitAnimal> _pool = new ObjectPool<NetHitAnimal>(10);

	public int Animal;
	public float Damage;
	public Vector2 EffectOffset;
	public E.Effects Effect;
	public override void Handle(GameManager game)
	{
		var entity = game.Entities.GetEntity(Animal);
		Vector3 pos = entity.gameObject.transform.position + new Vector3(EffectOffset.x, EffectOffset.y, 0.2f);
		game.CreateEffect(Effect, pos, 0.5f);
		var animal = game.Entities.GetComponentOf<Animal>(Animal);

		animal.Health -= Damage;
		if (animal.Health <= 0)
		{
			game.CreateEffect(E.Effects.BloodDeath, entity.gameObject.transform.position, 0.3f);
		}

		int hostid = Utility.IsHost(game);

		game.AddAction(() =>
		{
			if (hostid != -1)
			{
				var ingredientHolder = entity.gameObject.GetComponent<IngredientHolder>();
				foreach (ScriptableItem.Recipe r in ingredientHolder.Ingredients)
				{
					float randomnedAngle = UnityEngine.Random.Range(-30, 30);
					float force = UnityEngine.Random.Range(8, 14);
					var vec = Utility.Rotate(Vector2.up, randomnedAngle) * force;
					HandleNetEventSystem.AddEvent(game, hostid, NetCreateIngredient.Make(hostid, r.AmountNeeded, r.Ingredient, entity.gameObject.transform.position, vec));
				}
			}
			GameObject.Destroy(entity.gameObject);
			game.Entities.RemoveEntity(entity);
		});
	}

	public static NetHitAnimal Make()
	{
		return _pool.GetNext();
	}

	public static NetHitAnimal Make(int player, float damage, E.Effects effect, Vector2 effectOffset)
	{
		var evt = _pool.GetNext();
		evt.Animal = player;
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
		Animal = BitConverter.ToInt32(byteData, index); index += sizeof(int);
		Damage = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float x = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float y = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		Effect = (E.Effects)BitConverter.ToInt32(byteData, index); index += sizeof(int);
		EffectOffset = new Vector2(x, y);
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetHitAnimal));
		outgoing.AddRange(BitConverter.GetBytes(20));
		outgoing.AddRange(BitConverter.GetBytes(Animal));
		outgoing.AddRange(BitConverter.GetBytes(Damage));
		outgoing.AddRange(BitConverter.GetBytes(EffectOffset.x));
		outgoing.AddRange(BitConverter.GetBytes(EffectOffset.y));
		outgoing.AddRange(BitConverter.GetBytes((int)Effect));
	}
}
