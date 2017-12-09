using Game;
using Game.Component;
using Game.GEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemCreator : Item
{

	private static ObjectPool<ItemCreator> _pool = new ObjectPool<ItemCreator>(10);
	public override void Recycle()
	{
		_pool.Recycle(this);
	}

	public ItemCreator()
	{

	}
	public static ItemCreator Make()
	{
		ItemCreator item = _pool.GetNext();
		return item;
	}

	public static void MakeItem(GameManager game, Vector3 position)
	{
		var go = GameObject.Instantiate(game.GameResources.Prefabs.Pickaxe);
		go.transform.position = position;

		go.AddComponent<VisibleItem>().CallBack = (EntityID) =>
		{
			var itemHolder = game.Entities.GetComponentOf<ItemHolder>(EntityID);
			itemHolder.Items[(int)Item.ItemID.ItemCreator].OnPickup(game, EntityID, go);
		};
	}
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
	}
	public override void Input(GameManager game, int entity)
	{
		var position = game.Entities.GetEntity(entity).gameObject.transform.position;
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var netEvents = game.Entities.GetComponentOf<NetEventComponent>(entity);

		if (input.E)
		{
			position += new Vector3(0, 2, 0);
			int forceX = game.CurrentRandom.Next(0, 11);
			int forceXNeg = game.CurrentRandom.Next(0, 2);
			forceX = forceXNeg == 1 ? -forceX : forceX;
			int forceY = game.CurrentRandom.Next(0, 10);
			var force = new Vector2(forceX, forceY);
			var itemrand = game.CurrentRandom.Next(0, 2);
			force = input.ArmDirection * 5;
			if (itemrand == 0)
			{
				netEvents.CurrentEventID++;
				netEvents.NetEvents.Add(NetCreateItem.Make(entity, netEvents.CurrentEventID, ItemID.Pickaxe, position, force));

			}
			if (itemrand == 1)
			{
				netEvents.CurrentEventID++;
				netEvents.NetEvents.Add(NetCreateItem.Make(entity, netEvents.CurrentEventID, ItemID.Rope, position, force));
			} 
		}

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

