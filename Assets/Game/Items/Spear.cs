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
using IOnMouseRight =  Item.IOnMouseRight;
using IOnMouseLeft = Item.IOnMouseLeft;
using Effects = Game.Effects;
using GatherLevel = GatherableScriptable.GatherLevel;

public class Spear : Item, IOnMouseRight, IOnMouseLeft
{

	private static ObjectPool<Spear> _pool = new ObjectPool<Spear>(10);
	public Animator SpearAnim;
	public bool Attack;

	public SpearScriptable SpearScript;
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
		var hit2 = Physics2D.Raycast(events.Transform1.position, pointer.right, pointer3.localPosition.x, LayerMask.NameToLayer("EnemyShield"));

		if (hit2.collider != null)
		{
			var transform = hit2.collider.transform;
			var itemholder = transform.GetComponent<ItemIdHolder>();
			HandleNetEventSystem.AddEventAndHandle(game, e, NetHitItem.Make(itemholder.Owner, itemholder.ID, 20, Effects.Ricochet, hit.point));
		}
		Debug.DrawLine(events.Transform1.position, pointer3.position, Color.red);
		var collider = hit.collider;
		if (collider != null)
		{
			var transform = collider.transform;
			if (collider.gameObject.layer == LayerMask.NameToLayer("PlayerEnemy"))
			{
				var id = transform.GetComponent<IdHolder>().ID;
				Vector2 offsetPoint = hit.point - new Vector2(transform.position.x, transform.position.y) + (dir * 0.3f);
				HandleNetEventSystem.AddEventAndHandle(game, e, NetHitPlayer.Make(id, 10, Effects.Blood3, offsetPoint));
			}
			if (collider.gameObject.layer == LayerMask.NameToLayer("Animal"))
			{
				var id = transform.GetComponent<IdHolder>().ID;
				Vector2 offsetPoint = hit.point - new Vector2(transform.position.x, transform.position.y) + (dir * 0.7f);
				HandleNetEventSystem.AddEventAndHandle(game, e, NetHitAnimal.Make(id, 10, Effects.Blood3, offsetPoint));
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

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Spear, position, force, SpearScript.Tier));
	}

	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force, int tier)
	{
		var spear = ((ScriptableItemCollection)game.GameResources.AllItems.Spear).Collection[tier] as SpearScriptable;
		
		var go = GameObject.Instantiate(spear.Prefab);
		go.transform.position = position;


		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		item.SpearScript = spear;
		item.ScrItem = spear;
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

	bool attacking = false;
	float currentTime = 0;
	float starX = 0;
	Vector2 attackDir;
	public override void Input(GameManager game, int entity, float delta)
	{
		if (attacking)
		{
			var events = SpearAnim.GetComponent<AnimationEvents>();
			var temp = events.transform.localPosition;
			currentTime += delta;
			float time = (currentTime / SpearScript.OutTime);
			if (time >= 1)
			{
				time = ((currentTime - SpearScript.OutTime) / (SpearScript.InTime));
				temp.x = Mathf.Lerp(SpearScript.Length, 0, (time)) + starX;
				attacking = (time < 1);
	
				var stats = game.Entities.GetComponentOf<Stats>(entity);
				stats.CharacterStats.ArmRotationSpeed = SpearScript.RotationSpeed * time;
				
			}
			else
			{
				currentTime += delta;
				time = (currentTime / SpearScript.OutTime);
				temp.x = Mathf.Lerp(0, SpearScript.Length, (time)) + starX;
				AttackEvent(game, entity);
			}
			events.Transform1.localPosition = temp;
		}
	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{
		float x = BitConverter.ToSingle(byteData, currentIndex); currentIndex += sizeof(float);
		float y = BitConverter.ToSingle(byteData, currentIndex); currentIndex += sizeof(float);
		attackDir = new Vector2(x, y);
	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{
		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));

		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		byteArray.AddRange(BitConverter.GetBytes(resources.FreeArm.up.x));
		byteArray.AddRange(BitConverter.GetBytes(resources.FreeArm.up.y));
	}

	void IOnMouseRight.OnMouseRight(GameManager game, int entity)
	{
		var events = SpearAnim.GetComponent<AnimationEvents>();
		var temp = events.Transform1.localPosition;
		var stats = game.Entities.GetComponentOf<Stats>(entity);
		if (!attacking)
		{
			stats.CharacterStats.ArmRotationSpeed = 0;
			attacking = true;
			starX = temp.x;
			currentTime = 0;
		}
	}

	void IOnMouseLeft.OnMouseLeft(GameManager game, int entity)
	{
		var events = SpearAnim.GetComponent<AnimationEvents>();
		var temp = events.Transform1.localPosition;

		var stats = game.Entities.GetComponentOf<Stats>(entity);
		if (!attacking)
		{
			stats.CharacterStats.ArmRotationSpeed = 0;
			attacking = true;
			starX = temp.x;
			currentTime = 0;
		}
	}
}

