﻿using Game;
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

public class Base : Item
{

	private static ObjectPool<Base> _pool = new ObjectPool<Base>(10);

	Transform Placeable;
	public override void Recycle()
	{
		Placeable = null;
		_pool.Recycle(this);
	}

	public Base()
	{

	}
	public static Base Make()
	{
		Base item = _pool.GetNext();
		item.ID = ItemID.Base;
		return item;
	}

	public override void OwnerActivate(GameManager game, int entity)
	{
		if (Placeable == null)
		{

			Placeable = GameObject.Instantiate(game.GameResources.AllItems.Base.Prefab).transform;
			Placeable.gameObject.layer = LayerMask.NameToLayer("Default");
			Placeable.transform.localScale = new Vector3(0.5f, 0.5f, 1);
		}
		base.OwnerActivate(game, entity);
	}

	public override void ClientActivate(GameManager game, int entity)
	{

		base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		if (Placeable)
			Placeable.position = new Vector3(0, 0, 0);
		base.OwnerDeActivate(game, entity);
	}

	public override void ThrowItem(GameManager game, int entity)
	{
		DestroyItem(game, entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = (input.ScreenDirection * 5) + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Base, position, force, 0, Health));
	}
	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Base.Prefab);
		go.transform.position = position;


		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		item.ScrItem = game.GameResources.AllItems.Base;
		item.Health = item.ScrItem.MaxHp;
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

	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		CheckMain(game, entity, gameObject);
	}

	public override void Input(GameManager game, int entity, float delta)
	{
		var player = game.Entities.GetComponentOf<Player>(entity);
		var layerMask = (1 << LayerMask.NameToLayer("Collideable"));
		if (!player.Owner)
			return;

		var transform = game.Entities.GetEntity(entity).gameObject.transform;
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		Vector2 placeDirection = input.ScreenDirection;
		Vector2 pos = transform.position;
		var hit = Physics2D.Raycast(pos, placeDirection, 2.0f, layerMask);
		if (Placeable == null)
			return;
		Placeable.position = new Vector3(0, 0, 0);
		if (hit.transform != null)
		{
			bool placeOK = false;
			if (hit.normal.y == 1)
			{
				placeOK = true;
				Placeable.position = hit.point + new Vector2(0, 0.64f);
				Placeable.position += new Vector3(0, 0, -0.15f);
			}
			if (input.OnLeftDown && placeOK)
			{
				HandleNetEventSystem.AddEventAndHandle(game, entity, NetCreateBase.Make(entity, Placeable.position));
				GameObject.Destroy(Placeable.gameObject);
				game.AddAction(() =>
				{
					DestroyItem(game, entity);
				});
				game.AddAction(() =>
				{
					var holder = game.Entities.GetComponentOf<ItemHolder>(entity);
					holder.Hands.OwnerActivate(game, entity);
				});
			}
		}
	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

