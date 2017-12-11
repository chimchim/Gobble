using Game;
using Game.Component;
using Game.GEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Ingredient : Item
{

	private static ObjectPool<Ingredient> _pool = new ObjectPool<Ingredient>(10);

	public override void Recycle()
	{
		_pool.Recycle(this);
	}

	public Ingredient()
	{

	}
	public static Ingredient Make()
	{
		Ingredient item = _pool.GetNext();
		item.ID = ItemID.Cubes;
		return item;
	}

	public override void OwnerActivate(GameManager game, int entity)
	{
		base.OwnerActivate(game, entity);
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		base.OwnerDeActivate(game, entity);
	}

	public override void ThrowItem(GameManager game, int entity)
	{
		base.ThrowItem(game, entity);
		var netEvents = game.Entities.GetComponentOf<NetEventComponent>(entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var position = game.Entities.GetEntity(entity).gameObject.transform.position;
		position.y += 0.3f;
		var itemrand = game.CurrentRandom.Next(0, 2);
		var force = input.ScreenDirection * 5;

		netEvents.CurrentEventID++;
		netEvents.NetEvents.Add(NetCreateItem.Make(entity, netEvents.CurrentEventID, Item.ItemID.Cubes, position, force));
	}
	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		//throw
		//{
			var go = GameObject.Instantiate(game.GameResources.AllItems.Gravel.Prefab);
		if (go == null)
		{
			Debug.Log("GO NULL");
		}
			go.transform.position = position;
		//}
		//catch (Exception e)
		//{
		//	Debug.log(e);
		//}

		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		visible.Item = item;
		visible.Force = force;

		visible.CallBack = (EntityID) =>
		{
			var player = game.Entities.GetComponentOf<Player>(EntityID);
			if (player.Owner)
			{
				var netComp = game.Entities.GetComponentOf<NetEventComponent>(EntityID);
				netComp.CurrentEventID++;
				var pickup = NetItemPickup.Make(EntityID, netComp.CurrentEventID, item.ItemNetID);
				pickup.Iterations = 1;
				netComp.NetEvents.Add(pickup);
				item.OnPickup(game, EntityID, go);
			}
		};

		return visible;
	}
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		CheckMain(game, entity, game.GameResources.AllItems.Gravel, gameObject);
	}

	public override void Input(GameManager game, int entity)
	{
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		//resources.FreeArmAnimator.SetBool("Dig", input.LeftDown);

	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

