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
	public List<Collider2D> CollidedWith = new List<Collider2D>();
	public override void Recycle()
	{
		SpearAnim = null;
		CollidedWith.Clear();
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
		Vector2 grahpicsPos = events.Transform1.position;
		Vector2 dir = pointer.right;

		float l = (pointer.position - pointer3.position).magnitude;
		float l1 = (grahpicsPos - new Vector2(pointer3.position.x, pointer3.position.y)).magnitude;

		var hit = Physics2D.Raycast(pointer.position, dir, l, game.LayerMasks.MappedMasks[3].UpLayers);
		var hit2 = Physics2D.Raycast(grahpicsPos, dir, l1, LayerMask.GetMask("EnemyShield") | LayerMask.GetMask("Collideable"));
		Debug.DrawLine(grahpicsPos, grahpicsPos + (dir * l1), Color.red);
		if (hit2.collider != null)
		{
			var transform = hit2.collider.transform;
			if (hit2.collider.gameObject.layer == LayerMask.NameToLayer("EnemyShield"))
			{
				var itemholder = transform.GetComponent<ItemIdHolder>();
				HandleNetEventSystem.AddEventAndHandle(game, e, NetHitItem.Make(itemholder.Owner, itemholder.ID, SpearScript.Damage, Effects.Ricochet, hit2.point));
			}
			else
			{
				HandleNetEventSystem.AddEventAndHandle(game, e, NetCreateEffect.Make(Effects.Ricochet, hit2.point, dir));
			}
			hitReturn = true;
			goBack = true;
			return;
		}

		var collider = hit.collider;
		if (collider != null && !CollidedWith.Contains(hit.collider))
		{
			CollidedWith.Add(hit.collider);
			var transform = collider.transform;
			if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox"))
			{
				var id = transform.root.GetComponent<IdHolder>().ID;
				Vector2 offsetPoint = hit.point - new Vector2(transform.position.x, transform.position.y) + (dir * 0.3f);
				HandleNetEventSystem.AddEventAndHandle(game, e, NetHitPlayer.Make(id, SpearScript.Damage, Effects.Blood3, offsetPoint));
			}
			if (collider.gameObject.layer == LayerMask.NameToLayer("Animal"))
			{
				var id = transform.GetComponent<IdHolder>().ID;
				Vector2 offsetPoint = hit.point - new Vector2(transform.position.x, transform.position.y) + (dir * 0.7f);
				HandleNetEventSystem.AddEventAndHandle(game, e, NetHitAnimal.Make(id, SpearScript.Damage, Effects.Blood3, offsetPoint));
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

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Spear, position, force, SpearScript.Tier, Health));
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

	bool attacking = false;
	float starX = 0;
	bool goBack = false;
	bool hitReturn = false;
	int attackCounter;
	public override void Input(GameManager game, int entity, float delta)
	{
		if (attacking)
		{
			var player = game.Entities.GetComponentOf<Player>(entity);
			var events = SpearAnim.GetComponent<AnimationEvents>();
			var temp = events.Transform1.transform.localPosition;

			if (!goBack)
			{
				if (!player.Owner) game.Entities.GetComponentOf<ResourcesComponent>(entity).FreeArm.up = game.Entities.GetComponentOf<InputComponent>(entity).ArmDirection;
				if (player.Owner) AttackEvent(game, entity);

				float outSpeed = SpearScript.Length / SpearScript.OutTime;
				float xTranslate = outSpeed * delta;
				float currentL = temp.x - starX;
				if (currentL + xTranslate < SpearScript.Length)
				{
					temp.x += xTranslate;
				}
				else
				{
					temp.x = starX + currentL;
					goBack = true;
				}
			}
			else
			{
				float inSpeed = SpearScript.Length / SpearScript.InTime;
				float xTranslate = -inSpeed * delta;
				float currentL = temp.x - starX;
				if (currentL + xTranslate > 0)
				{
					temp.x += xTranslate;
				}
				else
				{
					temp.x = starX;
					goBack = false;
					attacking = false;
					hitReturn = false;
				}
				float lengthNorm = (1 - Math.Max(0, (currentL + xTranslate) / SpearScript.Length));
				var stats = game.Entities.GetComponentOf<Stats>(entity);
				stats.CharacterStats.ArmRotationSpeed = SpearScript.RotationSpeed * lengthNorm;
				if(lengthNorm < 0.2f && player.Owner && !hitReturn) AttackEvent(game, entity);

			}

			events.Transform1.localPosition = temp;
		}
	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{
		int counter = BitConverter.ToInt32(byteData, currentIndex); currentIndex += sizeof(int);
		if (counter > attackCounter)
		{
			DoAttack(game, pack.PlayerID);
			attackCounter = counter;
		}
		var hitReturn = BitConverter.ToBoolean(byteData, currentIndex); currentIndex += sizeof(bool);
		if (hitReturn)
			goBack = hitReturn;
	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{
		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
		byteArray.AddRange(BitConverter.GetBytes(attackCounter));
		byteArray.AddRange(BitConverter.GetBytes(hitReturn));
	}

	void IOnMouseRight.OnMouseRight(GameManager game, int entity)
	{
		if (!attacking)
		{
			attackCounter++;
			DoAttack(game, entity);
		}
	}

	void IOnMouseLeft.OnMouseLeft(GameManager game, int entity)
	{
		if (!attacking)
		{
			attackCounter++;
			DoAttack(game, entity);
		}
	}

	void DoAttack(GameManager game, int entity)
	{
		var events = SpearAnim.GetComponent<AnimationEvents>();
		var temp = events.Transform1.localPosition;
		var stats = game.Entities.GetComponentOf<Stats>(entity);
		if (!attacking)
		{
			CollidedWith.Clear();
			stats.CharacterStats.ArmRotationSpeed = 0;
			attacking = true;
			starX = temp.x;
			hitReturn = false;
			goBack = false;
		}
	}
}

