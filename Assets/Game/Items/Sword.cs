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
	public bool Attacking;
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

	public override void OwnerActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		Effect = CurrentGameObject.transform.Find("Effect");
		resources.ArmEvents.Attackable = () =>
		{
			game.CreateEffect(E.Effects.Slice2, Effect.position, Effect.right, 0.5f);
			Attacking = true;
		};
		resources.ArmEvents.NotAttackable = () =>
		{
			Attacking = false;
		};
		base.OwnerActivate(game, entity);
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		Effect = CurrentGameObject.transform.Find("Effect");
		resources.ArmEvents.Attackable = () =>
		{
			game.CreateEffect(E.Effects.Slice2, Effect.position, Effect.right, 0.5f);
			Attacking = true;
		};
		resources.ArmEvents.NotAttackable = () =>
		{
			Attacking = false;
		};
		base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Sword", false);
		Attacking = false;
		base.OwnerDeActivate(game, entity);
	}
	public override void ClientDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Sword", false);
		Attacking = false;
		base.ClientDeActivate(game, entity);
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
		var input = game.Entities.GetComponentOf<InputComponent>(e);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
		resources.FreeArmAnimator.SetBool("Sword", input.LeftDown);
		if (!input.LeftDown)
		{
			Attacking = false;
		}
		if (Attacking)
		{
			Vector2 handPos = CurrentGameObject.transform.position;
			for (int i = 0; i < 5; i++)
			{
				Vector2 offset = CurrentGameObject.transform.up * (1.2f - (i * 0.2f));
				Vector2 pos = (handPos + offset);
				var hit = Physics2D.Raycast(pos, CurrentGameObject.transform.right, 0.4f, game.LayerMasks.MappedMasks[3].UpLayers);
				var collider = hit.collider;
				if (collider != null)
				{
					var transform = collider.transform;
					if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyShield"))
					{
						var normal = transform.right;
						float dot = Vector2.Dot(CurrentGameObject.transform.right, normal);
						if (dot < 0)
						{
							var itemholder = transform.GetComponent<ItemIdHolder>();
							
							HandleNetEventSystem.AddEventAndHandle(game, e, NetHitItem.Make(itemholder.Owner, itemholder.ID, 20, E.Effects.Ricochet, hit.point));
							Attacking = false;
							resources.FreeArmAnimator.SetBool("Sword", false);
							break;
						}
					}
					if (collider.gameObject.layer == LayerMask.NameToLayer("PlayerEnemy") || collider.gameObject.layer == LayerMask.NameToLayer("PlayerEnemyPlatform"))
					{
						var id = transform.GetComponent<IdHolder>().ID;
						Vector2 offsetPoint = hit.point - new Vector2(transform.position.x, transform.position.y);
						HandleNetEventSystem.AddEventAndHandle(game, e, NetHitPlayer.Make(id, 20, E.Effects.Blood3, offsetPoint));
						Attacking = false;
						resources.FreeArmAnimator.SetBool("Sword", false);
						break;
					}
				}
			}
		}

		var entity = game.Entities.GetEntity(e);
		resources.FreeArm.up = -input.ScreenDirection;
		if (entity.Animator.transform.eulerAngles.y > 6)
		{
			resources.FreeArm.up = input.ScreenDirection;
			resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
		}
	}


	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

