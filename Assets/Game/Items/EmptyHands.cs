using Game;
using Game.Component;
using Game.GEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
		//base.OwnerActivate(game, entity);
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.ArmEvents.OnArmHit = () =>
		{
			TryPick(game, entity);
		};
		//base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Dig", false);
		base.OwnerDeActivate(game, entity);
	}

	private void TryPick(GameManager game, int entity)
	{
		var trans = game.Entities.GetEntity(entity).gameObject.transform;
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var player = game.Entities.GetComponentOf<Player>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		var hand = resources.Hand;
		var layerMask = 1 << LayerMask.NameToLayer("Collideable");
		Debug.DrawLine(hand.position, hand.position + (hand.right * 1.4f), Color.blue);
		var hit = Physics2D.Raycast(hand.position, hand.right, 0.2f, layerMask);
		if (hit.transform == null)
			return;

		var bc = hit.transform.GetComponent<BlockComponent>();
		if (bc != null && !bc.Destroyed)
		{
			bc.HitsTaken++;
			var diff = bc.HitsTaken / bc.Mod;
			if (diff > 3 && player.Owner)
			{
				bc.Destroyed = true;
				var netComp = game.Entities.GetComponentOf<NetEventComponent>(entity);
				netComp.CurrentEventID++;
				var destroy = NetDestroyCube.Make(bc.X, bc.Y, netComp.CurrentEventID);
				netComp.NetEvents.Add(destroy);
				var position = bc.transform.position;
				netComp.CurrentEventID++;
				netComp.NetEvents.Add(NetCreateIngredient.Make(entity, netComp.CurrentEventID, bc.IngredientType, position, Vector2.zero));
			}
			bc.StartCoroutine(bc.Shake());
			int mod = bc.HitsTaken % bc.Mod;

			if (mod == 0)
			{
				bc.SetResource(game);
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

	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

