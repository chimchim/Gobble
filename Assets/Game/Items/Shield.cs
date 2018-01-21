using Game;
using Game.Component;
using Game.GEntity;
using Game.Systems;
using Gatherables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GatherLevel = GatherableScriptable.GatherLevel;

public class Shield : Item
{

	private static ObjectPool<Shield> _pool = new ObjectPool<Shield>(10);
	private LayerMask enemyLayer =  LayerMask.NameToLayer("EnemyShield");
	private LayerMask playerLayer = LayerMask.NameToLayer("PlayerShield");
	public override void Recycle()
	{
		_pool.Recycle(this);
	}

	public Shield()
	{

	}
	public static Shield Make()
	{
		Shield item = _pool.GetNext();
		item.ID = ItemID.Shield;
		return item;
	}

	public override void OwnerActivate(GameManager game, int entity)
	{
		base.OwnerActivate(game, entity);
		CurrentGameObject.layer = playerLayer;
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		base.ClientActivate(game, entity);
		#region Activate Layers
		var entities = game.Entities.GetEntitiesWithComponents(Bitmask.MakeFromComponents<Player>());
		var player = game.Entities.GetComponentOf<Player>(entity);
		foreach (int e in entities)
		{
			var otherPlayer = game.Entities.GetComponentOf<Player>(e);
			if (player.Owner)
			{
				if (player.Team == otherPlayer.Team)
				{
					CurrentGameObject.layer = playerLayer;
				}
				else
				{
					CurrentGameObject.layer = enemyLayer;
				}
				break;
			}
		} 
		#endregion
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{

		base.OwnerDeActivate(game, entity);
	}

	public override void ThrowItem(GameManager game, int entity)
	{
		base.ThrowItem(game, entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = (input.ScreenDirection * 5) + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Shield, position, force));
	}
	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Shield.Prefab);
		go.transform.position = position;


		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		visible.Item = item;
		visible.Force = force;
		var entities = game.Entities.GetEntitiesWithComponents(Bitmask.MakeFromComponents<Player>());
		foreach (int e in entities)
		{
			var player = game.Entities.GetComponentOf<Player>(e);
			if (player.IsHost && player.Owner)
			{
				visible.CallBack = (EntityID) =>
				{
					HandleNetEventSystem.AddEventAndHandle(game, e, NetItemPickup.Make(EntityID, item.ItemNetID));
				};
				break;
			}
		}

		return visible;
	}
	public override bool TryStack(GameManager game, Item item)
	{
		return false;
	}
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		CheckMain(game, entity, game.GameResources.AllItems.Shield, gameObject);
	}

	public override void Input(GameManager game, int entity)
	{
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var player = game.Entities.GetComponentOf<Player>(entity);
		var entityTransform = game.Entities.GetEntity(entity).gameObject.transform;

		var pos = entityTransform.position;
		var dir = CurrentGameObject.transform.right;
		var cross = Vector3.Cross(dir, Vector3.forward);

		var up = (Mathf.Abs(dir.y) > Mathf.Abs(dir.x)) ? true : false;



	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

