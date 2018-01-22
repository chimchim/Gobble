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

public class Shield : Item
{

	private static ObjectPool<Shield> _pool = new ObjectPool<Shield>(10);
	private LayerMask enemyLayer =  LayerMask.NameToLayer("EnemyShield");
	private LayerMask playerLayer = LayerMask.NameToLayer("PlayerShield");
	public bool IsSet;
	public override void Recycle()
	{
		IsSet = false;
		_pool.Recycle(this);
	}

	public Shield()
	{

	}
	public static Shield Make()
	{
		Shield item = _pool.GetNext();
		item.ID = ItemID.Shield;
		return item;
	}

	public override void OwnerActivate(GameManager game, int entity)
	{
		base.OwnerActivate(game, entity);
		if (IsSet)
			return;
		IsSet = true;
		CurrentGameObject.layer = playerLayer;
		CurrentGameObject.GetComponent<CapsuleCollider2D>().enabled = true;
		CurrentGameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		base.ClientActivate(game, entity);
		if (IsSet)
			return;
		IsSet = true;
		#region Activate Layers
		var entities = game.Entities.GetEntitiesWithComponents(Bitmask.MakeFromComponents<Player>());
		var player = game.Entities.GetComponentOf<Player>(entity);
		foreach (int e in entities)
		{
			var otherPlayer = game.Entities.GetComponentOf<Player>(e);
			if (otherPlayer.Owner)
			{
				CurrentGameObject.GetComponent<CapsuleCollider2D>().enabled = true;
				CurrentGameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
				if (player.Team == otherPlayer.Team)
				{
					CurrentGameObject.layer = playerLayer;
				}
				else
				{
					CurrentGameObject.layer = enemyLayer;
				}
				break;
			}
		} 
		#endregion
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{

		base.OwnerDeActivate(game, entity);
	}

	public override void ThrowItem(GameManager game, int entity)
	{
		base.ThrowItem(game, entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = (input.ScreenDirection * 5) + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Shield, position, force));
	}
	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Shield.Prefab);
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
		CheckMain(game, entity, game.GameResources.AllItems.Shield, gameObject);
	}

	public override void Input(GameManager game, int entity)
	{
		var entityObj = game.Entities.GetEntity(entity);

		CurrentGameObject.transform.localPosition = CurrentGameObject.transform.localPosition;
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		Vector2 middleScreen = new Vector2(Screen.width / 2, Screen.height / 2);
		Vector2 screenDirection = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y) - middleScreen;
		resources.FreeArm.up = -input.ScreenDirection;
		if (entityObj.Animator.transform.eulerAngles.y > 6)
		{
			resources.FreeArm.up = input.ScreenDirection;
			resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
		}
		float rotDir = Math.Sign((resources.FreeArm.up.x * resources.FacingDirection));
		if (resources.FacingDirection > 0)
		{
			if (rotDir > 0)
			{
				var eu = CurrentGameObject.transform.localEulerAngles;
				CurrentGameObject.transform.localEulerAngles = new Vector3(eu.x, 180, eu.z);
			}
			else
			{
				var eu = CurrentGameObject.transform.localEulerAngles;
				CurrentGameObject.transform.localEulerAngles = new Vector3(eu.x, 0, eu.z);
			}
		}
		else
		{
			if (rotDir > 0)
			{
				var eu = CurrentGameObject.transform.localEulerAngles;
				CurrentGameObject.transform.localEulerAngles = new Vector3(eu.x, 0, eu.z);
			}
			else
			{
				var eu = CurrentGameObject.transform.localEulerAngles;
				CurrentGameObject.transform.localEulerAngles = new Vector3(eu.x, 180, eu.z);
			}
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

