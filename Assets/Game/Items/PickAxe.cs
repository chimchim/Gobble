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
		{
			//var entityPos = game.Entities.GetEntity(entity).gameObject.transform.position;
			//var newDir = HitPointer.position - entityPos;
			//hit = Physics2D.Raycast(entityPos, newDir, 1.2f, layerMask);
			//if(hit.transform == null)
				return;
		}
		var bc = hit.transform.GetComponent<Gatherable>();
		if (bc != null)
		{
			var tryhit = bc.OnHit(game, GatherLevel.Pickaxe);
			if (tryhit && player.Owner)
			{
				if (!bc.GatherScript.CreateFromGatherable)
				{
					HandleNetEventSystem.AddEvent(game, entity, NetEvent.GetGatherableEvent(bc));
					var position = bc.transform.position;
					HandleNetEventSystem.AddEvent(game, entity, NetCreateIngredient.Make(entity, 1, bc.IngredientType, position, bc.GetForce()));
				}
				else
				{
					GatherableCustom custom = bc as GatherableCustom;
					HandleNetEventSystem.AddEvent(game, entity, NetIngredientFromGatherable.Make(entity, 1, custom.CustomIndex, bc.IngredientType, bc.GetForce()));
				}
			}
		}
		
	}
	public override void ThrowItem(GameManager game, int entity)
	{
		DestroyItem(game, entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = (input.ScreenDirection * 5) + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Pickaxe, position, force, 0, Health));
	}
	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.PickAxe.Prefab);
		go.transform.position = position;

		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		item.ScrItem = game.GameResources.AllItems.PickAxe;
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
		base.RotateArm(game, entity);
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

