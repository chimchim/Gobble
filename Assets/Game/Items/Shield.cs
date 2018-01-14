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
	private LayerMask enemyLayer = 1 << LayerMask.NameToLayer("EnemyPlayer");
	private LayerMask playerLayer = 1 << LayerMask.NameToLayer("Player");
	public override void Recycle()
	{
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
	}

	public override void ClientActivate(GameManager game, int entity)
	{

		base.ClientActivate(game, entity);
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
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var player = game.Entities.GetComponentOf<Player>(entity);
		var entityTransform = game.Entities.GetEntity(entity).gameObject.transform;

		var pos = entityTransform.position;
		var dir = CurrentGameObject.transform.right;
		var cross = Vector3.Cross(dir, Vector3.forward);

		var up = (Mathf.Abs(dir.y) > Mathf.Abs(dir.x)) ? true : false;


		float rayDist = 1;
		LayerMask mask = player.Owner ? enemyLayer : playerLayer;

		for (int i = 0; i < 3; i++)
		{
			Vector2 rayPos = pos + (cross / 2) - (0.5f * cross * i);
			var hit = Physics2D.Raycast(rayPos, dir, rayDist, mask);
			Debug.DrawRay(rayPos, dir, Color.blue);
			if (hit.collider != null && hit.distance > 0)
			{
				var id = hit.collider.GetComponent<IdHolder>().ID;
				var movement = game.Entities.GetComponentOf<MovementComponent>(id);
				var lengthDiff = rayDist - (hit.point - rayPos).magnitude;
				var diff = (hit.point - rayPos).normalized * lengthDiff;
				movement.ForceVelocity.y += 3 * diff.y;
				movement.ForceVelocity.x += 3 * diff.x;
				//if (up)
				//	movement.CurrentVelocity.y = 0;
				//var lengthDiff = rayDist - (hit.point - rayPos).magnitude;
				//var diff = (hit.point - rayPos).normalized * lengthDiff;
				//HandleNetEventSystem.AddEvent(game, entity, NetAddForce.Make(id, dir * 10));
				//float xOffset = GameUnity.GroundHitBox.x;
				//float yOffset = GameUnity.GroundHitBox.y;
				//
				//bool vertGrounded = false;
				//bool horGrounded = false;
				//
				//Vector3 tempPos = hit.collider.transform.position;
				//var movemask = game.LayerMasks.MappedMasks[movement.CurrentLayer];
				//var tempPos1 = Game.Systems.Movement.HorizontalMovement(tempPos, diff.x, xOffset, yOffset, out horGrounded);
				//tempPos1 = Game.Systems.Movement.VerticalMovement(tempPos1, diff.y, xOffset, yOffset, movemask, out vertGrounded);
				//
				//hit.collider.transform.position = tempPos1;
				//float distance = Mathf.Abs(hitsY[i].point.y - pos.y);
				//float moveAmount = (distance * sign) + ((yoffset) * -sign);
				//movement = new Vector3(pos.x, pos.y + (moveAmount), 0);
				//grounded = true;
				break;
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

