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

public class Sword : Item
{

	private static ObjectPool<Sword> _pool = new ObjectPool<Sword>(10);

	public override void Recycle()
	{
		_pool.Recycle(this);
	}

	public Sword()
	{

	}
	public static Sword Make()
	{
		Sword item = _pool.GetNext();
		item.ID = ItemID.Sword;
		return item;
	}

	public override void OwnerActivate(GameManager game, int entity)
	{
		base.OwnerActivate(game, entity);
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Sword", false);
		base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Sword", false);
		base.OwnerDeActivate(game, entity);
	}

	public override void ThrowItem(GameManager game, int entity)
	{
		base.ThrowItem(game, entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = (input.ScreenDirection * 5) + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Sword, position, force));
	}
	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Sword.Prefab);
		go.transform.position = position;


		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
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
	public override bool TryStack(GameManager game, Item item)
	{
		return false;
	}
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		CheckMain(game, entity, game.GameResources.AllItems.Sword, gameObject);
	}
	public override void Input(GameManager game, int e, float delta)
	{
		var entity = game.Entities.GetEntity(e);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
		var input = game.Entities.GetComponentOf<InputComponent>(e);
		resources.FreeArm.up = -input.ScreenDirection;
		if (entity.Animator.transform.eulerAngles.y > 6)
		{
			resources.FreeArm.up = input.ScreenDirection;
			resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
		}
		if (!CurrentGameObject)
			return;
		float rotDir = Math.Sign((resources.FreeArm.up.x * resources.FacingDirection));
		var eu = CurrentGameObject.transform.localEulerAngles;
		if (resources.FacingDirection > 0)
		{
			CurrentGameObject.transform.localEulerAngles = (rotDir > 0) ? new Vector3(eu.x, 180, eu.z) : new Vector3(eu.x, 0, eu.z);
		}
		else
		{
			CurrentGameObject.transform.localEulerAngles = (rotDir > 0) ? new Vector3(eu.x, 0, eu.z) : new Vector3(eu.x, 180, eu.z);
		}

		resources.FreeArmAnimator.SetBool("Sword", input.LeftDown);
	}


	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

