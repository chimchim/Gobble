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
using Effects = Game.E.Effects;
using GatherLevel = GatherableScriptable.GatherLevel;
public class Spear : Item
{

	private static ObjectPool<Spear> _pool = new ObjectPool<Spear>(10);
	public Animator SpearAnim;
	public bool Attack;
	public override void Recycle()
	{
		_pool.Recycle(this);
	}

	public Spear()
	{

	}
	public static Spear Make()
	{
		Spear item = _pool.GetNext();
		item.ID = ItemID.Spear;
		return item;
	}

	public override void OwnerActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		SpearAnim = CurrentGameObject.GetComponent<Animator>();
		SpearAnim.enabled = true;
		var events = SpearAnim.GetComponent<AnimationEvents>();
		events.Transform1.localPosition = new Vector3(events.Transform2.localPosition.x, events.Transform1.localPosition.y, events.Transform1.localPosition.z);
		events.Attackable = () =>
		{
			AttackEvent(game, entity);
			Attack = true;
		};
		events.NotAttackable = () =>
		{
			Attack = false;
		};
		base.OwnerActivate(game, entity);
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		itemHolder.ActiveItems.Add(this);
		SpearAnim = CurrentGameObject.GetComponent<Animator>();
		SpearAnim.enabled = true;
		base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		base.OwnerDeActivate(game, entity);
	}
	public override void ClientDeActivate(GameManager game, int entity)
	{
		base.ClientDeActivate(game, entity);
	}

	public void AttackEvent(GameManager game, int e)
	{
		var events = SpearAnim.GetComponent<AnimationEvents>();
		var pointer = events.Transform3;
		var pointer3 = events.Transform4;

		Vector2 dir = pointer.right;
		float l = Mathf.Abs(pointer.localPosition.x - pointer3.localPosition.x);
		var hit = Physics2D.Raycast(pointer.position, pointer.right, l, game.LayerMasks.MappedMasks[3].UpLayers);
		Debug.DrawLine(pointer.position, pointer.position + (pointer.right * l), Color.red);
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
					HandleNetEventSystem.AddEventAndHandle(game, e, NetHitItem.Make(itemholder.Owner, itemholder.ID, 20, E.Effects.Ricochet, hit.point));
				}
			}
			if (collider.gameObject.layer == LayerMask.NameToLayer("PlayerEnemy") || collider.gameObject.layer == LayerMask.NameToLayer("PlayerEnemyPlatform"))
			{
				var id = transform.GetComponent<IdHolder>().ID;
				Vector2 offsetPoint = hit.point - new Vector2(transform.position.x, transform.position.y) + (dir * 0.3f);
				HandleNetEventSystem.AddEventAndHandle(game, e, NetHitPlayer.Make(id, 10, E.Effects.Blood3, offsetPoint));
			}
			if (collider.gameObject.layer == LayerMask.NameToLayer("Animal"))
			{
				var id = transform.GetComponent<IdHolder>().ID;
				Vector2 offsetPoint = hit.point - new Vector2(transform.position.x, transform.position.y) + (dir * 0.7f);
				HandleNetEventSystem.AddEventAndHandle(game, e, NetHitAnimal.Make(id, 10, E.Effects.Blood3, offsetPoint));
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

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Spear, position, force));
	}

	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Spear.Prefab);
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
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		CheckMain(game, entity, game.GameResources.AllItems.PickAxe, gameObject);
	}
	int SpearCounter = 0;
	public override void Input(GameManager game, int entity, float delta)
	{
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		if (input.OnLeftDown)
		{
			SpearCounter++;
			SpearAnim.SetTrigger("AttackTrigger");
		}
		SpearAnim.SetBool("AttackLoop", input.RightDown);
		//if (Attack)
		//{
		//	AttackEvent(game, entity);
		//}
		base.RotateArm(game, entity);

	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{
		int spearCounter = BitConverter.ToInt32(byteData, currentIndex); currentIndex += sizeof(int);
		if (spearCounter > SpearCounter)
		{
			SpearCounter = spearCounter;
			SpearAnim.SetTrigger("AttackTrigger");
		}
	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
		byteArray.AddRange(BitConverter.GetBytes(SpearCounter));
	}
}

