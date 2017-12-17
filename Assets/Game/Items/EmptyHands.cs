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
public class EmptyHands : Item
{

	private static ObjectPool<EmptyHands> _pool = new ObjectPool<EmptyHands>(10);

	public override void Recycle()
	{
		_pool.Recycle(this);
	}

	public EmptyHands()
	{

	}
	public static EmptyHands Make()
	{
		EmptyHands item = _pool.GetNext();
		item.ID = ItemID.Pickaxe;
		return item;
	}

	public override void OwnerActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.ArmEvents.OnArmHit = () =>
		{
			TryPick(game, entity);
		};
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		itemHolder.ActiveItems.Add(this);
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.ArmEvents.OnArmHit = () =>
		{
			TryPick(game, entity);
		};
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		itemHolder.ActiveItems.Add(this);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		itemHolder.ActiveItems.Remove(this);
		resources.FreeArmAnimator.SetBool("Dig", false);
	}
	public override void ClientDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		itemHolder.ActiveItems.Remove(this);
		resources.FreeArmAnimator.SetBool("Dig", false);
	}

	private void TryPick(GameManager game, int entity)
	{
		var player = game.Entities.GetComponentOf<Player>(entity);
		var hand = game.Entities.GetComponentOf<ResourcesComponent>(entity).Hand;
		var layerMask = (1 << LayerMask.NameToLayer("Collideable")) | (1 << LayerMask.NameToLayer("Gatherable"));

		var hit = Physics2D.Raycast(hand.position, -hand.up, 0.7f, layerMask);
		if (hit.transform == null)
			return;

		var bc = hit.transform.GetComponent<Gatherable>();
		if (bc != null)
		{
			var tryhit = bc.OnHit(game, GatherLevel.Hands);
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
		//var hand = game.Entities.GetComponentOf<ResourcesComponent>(entity).Hand;
		//
		////var hit = Physics2D.Raycast(hand.position, -hand.up, 0.4f, layerMask);
		//Debug.DrawLine(hand.position, hand.position + (-hand.up * 3), Color.green);

	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

