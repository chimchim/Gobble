using Game;
using Game.Component;
using Game.GEntity;
using Game.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PickAxe : Item
{

	private static ObjectPool<PickAxe> _pool = new ObjectPool<PickAxe>(10);

	Transform HitPointer;
	public override void Recycle()
	{
		HitPointer = null;
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

	public override void OwnerActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		HitPointer = CurrentGameObject.transform.Find("point");
		resources.ArmEvents.OnArmHit = () =>
		{
			TryPick(game, entity);
		};
		base.OwnerActivate(game, entity);
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		HitPointer = CurrentGameObject.transform.Find("point");
		resources.ArmEvents.OnArmHit = () =>
		{
			TryPick(game, entity);
		};
		base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Dig", false);
		base.OwnerDeActivate(game, entity);
	}

	private void TryPick(GameManager game, int entity)
	{
		var player = game.Entities.GetComponentOf<Player>(entity);
		var layerMask = (1 << LayerMask.NameToLayer("Collideable")) | (1 << LayerMask.NameToLayer("Gatherable"));
		
		var hit = Physics2D.Raycast(HitPointer.position, HitPointer.right, 0.2f, layerMask);
		if (hit.transform == null)
			return;

		var bc = hit.transform.GetComponent<Gatherable>();
		if (bc != null)
		{
			bc.HitsTaken++;
			var diff = bc.HitsTaken / bc.Mod;
			if (diff > 3 && player.Owner)
			{
				HandleNetEventSystem.AddEvent(game, entity, NetEvent.GetGatherableEvent(bc));
				var position = bc.transform.position;
				HandleNetEventSystem.AddEvent(game, entity, NetCreateIngredient.Make(entity, 1, bc.IngredientType, position, Vector2.zero));
			}
			bc.OnHit();
			int mod = bc.HitsTaken % bc.Mod;
			
			if (mod == 0)
			{
				bc.SetResource(game);
			}
		}
		
	}
	public override void ThrowItem(GameManager game, int entity)
	{
		base.ThrowItem(game, entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = (input.ScreenDirection * 5) + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Pickaxe, position, force));
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
				HandleNetEventSystem.AddEventIgnoreOwner(game, EntityID, NetItemPickup.Make(EntityID, item.ItemNetID));
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
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Dig", input.LeftDown);
		
	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

