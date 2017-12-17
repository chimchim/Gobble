using Game;
using Game.Component;
using Game.GEntity;
using Game.Systems;
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

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = input.ScreenDirection * 5 + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateIngredient.Make(entity, Quantity, IngredientType, position, force));
	}

	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force, TileMap.IngredientType ingredientType)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Ingredient.Prefab);
		if (go == null)
		{
			Debug.Log("GO NULL");
		}
		go.transform.position = position;
		
		go.GetComponent<SpriteRenderer>().sprite = game.GameResources.AllItems.Ingredient.IngredientsTypes[(int)ingredientType];
		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		item.IngredientType = ingredientType;
		visible.Item = item;
		visible.Force = force;

		visible.CallBack = (EntityID) =>
		{
			var player = game.Entities.GetComponentOf<Player>(EntityID);
			if (player.Owner)
			{
				var holder = game.Entities.GetComponentOf<ItemHolder>(EntityID);
				foreach (Item stackable in holder.Items.Values)
				{
					if (stackable.TryStack(game, item))
					{
						var inv = game.Entities.GetComponentOf<InventoryComponent>(EntityID);
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
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		game.GameResources.AllItems.Ingredient.Sprite = game.GameResources.AllItems.Ingredient.IngredientsTypes[(int)IngredientType];
		CheckMain(game, entity, game.GameResources.AllItems.Ingredient, gameObject);
	}
	private void TryPick(GameManager game, int entity)
	{
		var player = game.Entities.GetComponentOf<Player>(entity);
		var hand = game.Entities.GetComponentOf<ResourcesComponent>(entity).Hand;
		var layerMask = (1 << LayerMask.NameToLayer("Collideable")) | (1 << LayerMask.NameToLayer("Gatherable"));
		var offset = -hand.up * 0.4f;
		var hit = Physics2D.Raycast(hand.position + offset, -hand.up, 0.4f, layerMask);
		if (hit.transform == null)
			return;

		var bc = hit.transform.GetComponent<Gatherable>();
		if (bc != null)
		{
			var tryhit = bc.OnHit(game, GatherLevel.Hands);
			if (tryhit && player.Owner)
			{
				HandleNetEventSystem.AddEvent(game, entity, NetEvent.GetGatherableEvent(bc));
				var position = bc.transform.position;
				HandleNetEventSystem.AddEvent(game, entity, NetCreateIngredient.Make(entity, 1, bc.IngredientType, position, Vector2.zero));
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

