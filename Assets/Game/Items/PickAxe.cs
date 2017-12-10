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
		item.ID = ItemID.Pickaxe;
		return item;
	}
	public override void OwnerDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Dig", false);
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
		netEvents.NetEvents.Add(NetCreateItem.Make(entity, netEvents.CurrentEventID, Item.ItemID.Pickaxe, position, force));
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
		CheckMain(game, entity, game.GameResources.AllItems.PickAxe, gameObject);
	}

	public override void Input(GameManager game, int entity)
	{
		var trans = game.Entities.GetEntity(entity).gameObject.transform;
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Dig", input.LeftDown);
		Vector3 screendir3d = new Vector3(input.ScreenDirection.x, input.ScreenDirection.y, 0);
		Debug.DrawLine(trans.position, trans.position + (screendir3d * 5), Color.blue);

	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

