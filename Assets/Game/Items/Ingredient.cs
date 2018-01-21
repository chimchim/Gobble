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

public class Ingredient : Item
{

	private static ObjectPool<Ingredient> _pool = new ObjectPool<Ingredient>(10);
	public TileMap.IngredientType IngredientType;
	public override void Recycle()
	{
		_pool.Recycle(this);
	}

	public Ingredient()
	{

	}
	public static Ingredient Make()
	{
		Ingredient item = _pool.GetNext();
		item.ID = ItemID.Ingredient;
		return item;
	}

	public override void OwnerActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.ArmEvents.OnArmHit = () =>
		{
			TryPick(game, entity);
		};
		base.OwnerActivate(game, entity);
	}

	public override void ClientActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.ArmEvents.OnArmHit = () =>
		{
			TryPick(game, entity);
		};
		base.ClientActivate(game, entity);
	}

	public override void OwnerDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Dig", false);
		base.OwnerDeActivate(game, entity);
	}
	public override void ClientDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Dig", false);
		base.ClientDeActivate(game, entity);
	}

	public override void ThrowItem(GameManager game, int entity)
	{
		base.ThrowItem(game, entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var inventory = game.Entities.GetComponentOf<InventoryComponent>(entity);
		game.GameResources.AllItems.IngredientAmount[(int)IngredientType] -= Quantity;
		inventory.Crafting.SetCurrent();

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = input.ScreenDirection * 5 + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateIngredient.Make(entity, Quantity, IngredientType, position, force));
	}
	public static VisibleItem MakeFromGatherable(GameManager game, GameObject go, Vector2 force, TileMap.IngredientType ingredientType, int creator)
	{
		var platform = go.transform.Find("platform");
		if (platform != null)
			GameObject.Destroy(platform.gameObject);

		go.layer = LayerMask.NameToLayer("Item");
		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		item.IngredientType = ingredientType;
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
					HandleNetEventSystem.AddEventAndHandle(game, e, NetIngredientPickup.Make(EntityID, item.ItemNetID));
				};
				break;
			}
		}

		return visible;
	}
	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force, TileMap.IngredientType ingredientType, int creator)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Ingredient.IngredientsPrefabs[(int)ingredientType]);
		if (go == null)
		{
			Debug.Log("GO NULL");
		}
		go.transform.position = position;
		
		//go.GetComponent<SpriteRenderer>().sprite = game.GameResources.AllItems.Ingredient.IngredientsTypes[(int)ingredientType];
		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		item.IngredientType = ingredientType;
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
					HandleNetEventSystem.AddEventAndHandle(game, e, NetIngredientPickup.Make(EntityID, item.ItemNetID));
				};
				break;
			}
		}

		return visible;
	}
	public static void SetCraftingData(GameManager game, int entity, int ingredientType)
	{
		var inventory = game.Entities.GetComponentOf<InventoryComponent>(entity);
		inventory.Crafting.SetCurrent();
	}
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		game.GameResources.AllItems.Ingredient.Sprite = game.GameResources.AllItems.Ingredient.InventorySprite[(int)IngredientType];
		CheckMain(game, entity, game.GameResources.AllItems.Ingredient, gameObject);
	}
	private void TryPick(GameManager game, int entity)
	{
		var player = game.Entities.GetComponentOf<Player>(entity);
		var hand = game.Entities.GetComponentOf<ResourcesComponent>(entity).Hand;
		var layerMask = (1 << LayerMask.NameToLayer("Collideable")) | (1 << LayerMask.NameToLayer("Gatherable"));
		var hit = Physics2D.Raycast(hand.position, -hand.up, 0.4f, layerMask);
		if (hit.transform == null)
		{
			var entityPos = game.Entities.GetEntity(entity).gameObject.transform.position;
			var newDir = hand.position - entityPos;
			hit = Physics2D.Raycast(entityPos, newDir, 1.2f, layerMask);
			if (hit.transform == null)
				return;
		}

		var bc = hit.transform.GetComponent<Gatherable>();
		if (bc != null)
		{
			var tryhit = bc.OnHit(game, GatherLevel.Hands);
			if (tryhit && player.Owner)
			{
				if (!bc.GatherScript.CreateFromGatherable)
				{
					HandleNetEventSystem.AddEvent(game, entity, NetEvent.GetGatherableEvent(bc));
					var position = bc.transform.position;
					HandleNetEventSystem.AddEvent(game, entity, NetCreateIngredient.Make(entity, 1, bc.IngredientType, position, bc.GetForce()));
				}
				else
				{
					GatherableCustom custom = bc as GatherableCustom;
					HandleNetEventSystem.AddEvent(game, entity, NetIngredientFromGatherable.Make(entity, 1, custom.CustomIndex, bc.IngredientType, bc.GetForce()));
				}
			}
		}

	}
	public override void Input(GameManager game, int entity)
	{
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		resources.FreeArmAnimator.SetBool("Dig", input.LeftDown);

	}

	public override bool TryStack(GameManager game, Item item)
	{
		Ingredient ingre = item as Ingredient;
		if (ingre != null)
		{
			if (ingre.IngredientType == IngredientType)
			{
				Quantity += ingre.Quantity;
				game.GameResources.AllItems.IngredientAmount[(int)IngredientType] = Quantity;
				return true;
			}
        }

		return false;
	}
	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{


	}


	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
	}
}

