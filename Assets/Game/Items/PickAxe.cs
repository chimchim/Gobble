using Game;
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
		item.ID = ItemID.Pickaxe;
		return item;
	}

	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.PickAxe.Prefab);
		go.transform.position = position;

		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		visible.Item = item;
		visible.Force = force;

		visible.CallBack = (EntityID) =>
		{
			var player = game.Entities.GetComponentOf<Player>(EntityID);
			if (player.Owner)
			{		
				item.OnPickup(game, EntityID, go);
				var netComp = game.Entities.GetComponentOf<NetEventComponent>(EntityID);
				var pickup = NetItemPickup.Make(EntityID, netComp.CurrentEventID, item.ItemNetID);
				netComp.CurrentEventID++;
				netComp.NetEvents.Add(pickup);
			}
		};
		
		return go.GetComponent<VisibleItem>();
	}
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		var player = game.Entities.GetComponentOf<Player>(entity);
		CheckMain(game, entity, game.GameResources.AllItems.PickAxe, gameObject);
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
		int id = (int)Item.ItemID.Pickaxe;
		byteArray.AddRange(BitConverter.GetBytes(id));
	}
}

