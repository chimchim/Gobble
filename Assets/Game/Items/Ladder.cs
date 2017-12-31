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
		}
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

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Ladder, position, force));
	}
	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Ladder.Prefab);
		go.transform.position = position;


		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		visible.Item = item;
		visible.Force = force;
		visible.CallBack = (EntityID) =>
		{
			var player = game.Entities.GetComponentOf<Player>(EntityID);
			if (player.Owner)
			{
				var inv = game.Entities.GetComponentOf<InventoryComponent>(EntityID);
				var hasSlot = (inv.MainInventory.CurrenItemsAmount < GameUnity.MainInventorySize) ||
				(inv.InventoryBackpack.CurrenItemsAmount < GameUnity.BackpackInventorySize);
				if (!hasSlot)
					return;
				var holder = game.Entities.GetComponentOf<ItemHolder>(EntityID);
				foreach (Item stackable in holder.Items.Values)
				{
					if (stackable.TryStack(game, item))
					{
						inv.InventoryBackpack.SetQuantity(stackable);
						inv.MainInventory.SetQuantity(stackable);
						HandleNetEventSystem.AddEvent(game, EntityID, NetDestroyWorldItem.Make(item.ItemNetID));
						return;
					}
				}

				HandleNetEventSystem.AddEventIgnoreOwner(game, EntityID, NetItemPickup.Make(EntityID, item.ItemNetID));
				item.OnPickup(game, EntityID, go);
			}
		};

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

	public override void Input(GameManager game, int entity)
	{
		var player = game.Entities.GetComponentOf<Player>(entity);
		var layerMask = (1 << LayerMask.NameToLayer("Collideable"));
		if (!player.Owner)
			return;

		var transform = game.Entities.GetEntity(entity).gameObject.transform;
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var hand = game.Entities.GetComponentOf<ResourcesComponent>(entity).Hand;
		Vector2 pos = transform.position;
		var hit = Physics2D.Raycast(pos, -hand.up, 2.0f, layerMask);
		Debug.DrawLine(pos, pos + (new Vector2(-hand.up.x, -hand.up.y) * 2.0f), Color.blue);
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
				int nextX = x + (Math.Sign(-hand.up.x));
				var cube = game.TileMap.Blocks[nextX, y];
				if (cube == null || cube.GetComponent<GatherableBlock>().IngredientType == TileMap.IngredientType.TreeChunk)
				{
					placeOK = true;
					Placeable.position = new Vector3(hit.transform.position.x + (0.68f * (Math.Sign(-hand.up.x))), hit.transform.position.y, -0.2f);
				}
			}
			if (input.OnLeftDown && placeOK)
			{
				Placeable.gameObject.layer = LayerMask.NameToLayer("Ladder");
				HandleNetEventSystem.AddEventIgnoreOwner(game, entity, NetCreateLadder.Make(Placeable.position));
				Quantity--;
				Placeable = GameObject.Instantiate(game.GameResources.Prefabs.Ladder).transform;
				if (Quantity <= 0)
				{
					game.CallBacks.Add(() =>
					{
						base.ThrowItem(game, entity);
					});
					game.CallBacks.Add(() =>
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

