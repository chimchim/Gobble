﻿using Game;
using Game.Component;
using Game.GEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PickAxe : Item
{

	private static ObjectPool<PickAxe> _pool = new ObjectPool<PickAxe>(10);
	public override void Recycle()
	{
		_pool.Recycle(this);
	}

	public PickAxe()
	{

	}
	public static PickAxe Make()
	{
		PickAxe item = _pool.GetNext();
		item.Active = false;
		return item;
	}

	public static void MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.PickAxe.Prefab);
		go.transform.position = position;

		go.AddComponent<VisibleItem>().CallBack = (EntityID) =>
		{
			var itemHolder = game.Entities.GetComponentOf<ItemHolder>(EntityID);
			var item = itemHolder.Items[(int)Item.ItemID.Pickaxe];
			item.OnPickup(game, EntityID, go);
			SetInHand(game, EntityID, go);
		};
		go.GetComponent<VisibleItem>().Force = force;
	}
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		if (CurrentGameObject == null)
		{
			CurrentGameObject = gameObject;
		}
		else
		{
			GameObject.Destroy(gameObject);
		}
		var player = game.Entities.GetComponentOf<Player>(entity);
		if (player.Owner)
		{
			SetInInventory(game, entity, game.GameResources.AllItems.PickAxe);
		}
		Active = true;
	}

	public override void Input(GameManager game, int entity)
	{
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var movement = game.Entities.GetComponentOf<MovementComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);

		resources.FreeArmAnimator.SetBool("Dig", input.LeftClick);

	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{

		int entity = pack.PlayerID;
		var player = game.Entities.GetComponentOf<Player>(entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var movement = game.Entities.GetComponentOf<MovementComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);

	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var movement = game.Entities.GetComponentOf<MovementComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		int id = (int)Item.ItemID.ItemCreator;
		byteArray.AddRange(BitConverter.GetBytes(id));
	}
}

