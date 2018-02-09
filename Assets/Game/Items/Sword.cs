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
	public Transform Effect;
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

	public void AttackEvent(GameManager game, int e)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
		var input = game.Entities.GetComponentOf<InputComponent>(e);
		Effect = CurrentGameObject.transform.Find("Effect");
		var entity = game.Entities.GetEntity(e);
		Vector2 pos = resources.FreeArmAnimator.transform.position;
		Vector2 dir = -resources.FreeArm.up;

		var hit = Physics2D.Raycast(pos, dir, 1.4f, game.LayerMasks.MappedMasks[3].UpLayers);
		Debug.DrawLine(pos, pos + (dir * 1.4f), Color.red);
		var collider = hit.collider;
		if (collider != null)
		{
			var transform = collider.transform;
			if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyShield") || collider.gameObject.layer == LayerMask.NameToLayer("Collideable"))
			{
				var normal = transform.right;
				float dot = Vector2.Dot(CurrentGameObject.transform.right, normal);
				if (dot < 0)
				{
					var itemholder = transform.GetComponent<ItemIdHolder>();
					HandleNetEventSystem.AddEventAndHandle(game, e, NetHitItem.Make(itemholder.Owner, itemholder.ID, 20, Effects.Ricochet, hit.point));
				}
			}
			if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox"))
			{
				var id = transform.root.GetComponent<IdHolder>().ID;
				Vector2 offsetPoint = hit.point - new Vector2(transform.position.x, transform.position.y) + (dir * 0.3f);
				HandleNetEventSystem.AddEventAndHandle(game, e, NetHitPlayer.Make(id, 100, Effects.Blood3, offsetPoint));
			}
			if (collider.gameObject.layer == LayerMask.NameToLayer("Animal"))
			{
				var id = transform.GetComponent<IdHolder>().ID;
				Vector2 offsetPoint = hit.point - new Vector2(transform.position.x, transform.position.y) + (dir * 0.7f);
				HandleNetEventSystem.AddEventAndHandle(game, e, NetHitAnimal.Make(id, 100, Effects.Blood3, offsetPoint));
			}
		}

		game.CreateEffect(Effects.Slice2, Effect.position, Effect.right, 0.5f);
	}

	public override void OwnerActivate(GameManager game, int e)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
		Effect = CurrentGameObject.transform.Find("Effect");
		resources.ArmEvents.Attackable = () =>
		{
			AttackEvent(game, e);
		};

		base.OwnerActivate(game, e);
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		Effect = CurrentGameObject.transform.Find("Effect");
		resources.ArmEvents.Attackable = () =>
		{
			game.CreateEffect(Effects.Slice2, Effect.position, Effect.right, 0.5f);
		};
		resources.ArmEvents.NotAttackable = () =>
		{
		};
		base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Sword", false);
		base.OwnerDeActivate(game, entity);
	}
	public override void ClientDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Sword", false);
		base.ClientDeActivate(game, entity);
	}

	public override void ThrowItem(GameManager game, int entity)
	{
		DestroyItem(game, entity);
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
		item.ScrItem = game.GameResources.AllItems.Sword;
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
		CheckMain(game, entity, gameObject);
	}
	public override void Input(GameManager game, int e, float delta)
	{
		var entity = game.Entities.GetEntity(e);
		var input = game.Entities.GetComponentOf<InputComponent>(e);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
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

