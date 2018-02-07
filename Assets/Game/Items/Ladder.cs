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

public class Ladder : Item
{

	private static ObjectPool<Ladder> _pool = new ObjectPool<Ladder>(10);

	Transform Placeable;
	public override void Recycle()
	{
		Placeable = null;
		_pool.Recycle(this);
	}

	public Ladder()
	{

	}
	public static Ladder Make()
	{
		Ladder item = _pool.GetNext();
		item.ID = ItemID.Ladder;
		return item;
	}

	public override void OwnerActivate(GameManager game, int entity)
	{
		if (Placeable == null)
		{
			
			Placeable = GameObject.Instantiate(game.GameResources.Prefabs.Ladder).transform;
			Placeable.gameObject.layer = LayerMask.NameToLayer("Default");
		}
		base.OwnerActivate(game, entity);
	}

	public override void ClientActivate(GameManager game, int entity)
	{

		base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		if(Placeable)
			Placeable.position = new Vector3(0, 0, 0);
		base.OwnerDeActivate(game, entity);
	}

	public override void ThrowItem(GameManager game, int entity)
	{
		DestroyItem(game, entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = (input.ScreenDirection * 5) + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Ladder, position, force));
	}
	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Ladder.Prefab);
		go.transform.position = position;


		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		item.ScrItem = game.GameResources.AllItems.Ladder;
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
		Ladder ladder = item as Ladder;
		if (ladder != null)
		{
			Quantity += ladder.Quantity;
			return true;	
		}

		return false;
	}
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		CheckMain(game, entity, game.GameResources.AllItems.Ladder, gameObject);
	}

	public override void Input(GameManager game, int entity, float delta)
	{
		base.RotateArm(game, entity);
		var player = game.Entities.GetComponentOf<Player>(entity);
		var layerMask = (1 << LayerMask.NameToLayer("Collideable"));
		if (!player.Owner)
			return;

		var transform = game.Entities.GetEntity(entity).gameObject.transform;
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var hand = game.Entities.GetComponentOf<ResourcesComponent>(entity).Hand;
		Vector2 placeDirection = input.ScreenDirection;
		Vector2 pos = transform.position;
		var hit = Physics2D.Raycast(pos, placeDirection, 2.0f, layerMask);
		Debug.DrawLine(pos, pos + (new Vector2(placeDirection.x, placeDirection.y) * 2.0f), Color.blue);
		Placeable.position = new Vector3(0, 0, 0);
		if (hit.transform != null)
		{
			bool placeOK = false;
			if (hit.normal.y == 0)
			{
				placeOK = true;
				Placeable.position = new Vector3(hit.transform.position.x + (hit.normal.x * 0.68f), hit.transform.position.y, -0.2f);
			}
			else if (hit.normal.x == 0)
			{
				var coord = hit.transform.position / 1.28f;
				int x = (int)coord.x;
				int y = (int)coord.y;
				int nextX = x + (Math.Sign(placeDirection.x));
				var cube = game.TileMap.Blocks[nextX, y];
				if (cube == null || cube.GetComponent<GatherableBlock>().IngredientType == TileMap.IngredientType.TreeChunk)
				{
					placeOK = true;
					Placeable.position = new Vector3(hit.transform.position.x + (0.68f * (Math.Sign(placeDirection.x))), hit.transform.position.y, -0.2f);
				}
			}
			if (input.OnLeftDown && placeOK)
			{
				HandleNetEventSystem.AddEventAndHandle(game, entity, NetCreateLadder.Make(Placeable.position));
				Quantity--;
				if (Quantity <= 0)
				{
					game.AddAction(() =>
					{
						DestroyItem(game, entity);
					});
					game.AddAction(() =>
					{
						var holder = game.Entities.GetComponentOf<ItemHolder>(entity);
						holder.Hands.OwnerActivate(game, entity);
					});
					return;
				}
				var inv = game.Entities.GetComponentOf<InventoryComponent>(entity);
				
				inv.MainInventory.SetQuantity(this);
			}
			return;
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

