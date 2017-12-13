using Game;
using Game.Component;
using Game.GEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using IngredientType = TileMap.IngredientType;
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
		var netEvents = game.Entities.GetComponentOf<NetEventComponent>(entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = input.ScreenDirection * 5 + ent.PlayerSpeed;

		netEvents.CurrentEventID++;
		netEvents.NetEvents.Add(NetCreateIngredient.Make(entity, netEvents.CurrentEventID, IngredientType, position, force));
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
				var netComp = game.Entities.GetComponentOf<NetEventComponent>(EntityID);
				foreach (Item stackable in holder.Items.Values)
				{
					if (stackable.TryStack(game, item))
					{
						netComp.CurrentEventID++;
						var destroy = NetDestroyWorldItem.Make(item.ItemNetID, netComp.CurrentEventID);
						netComp.NetEvents.Add(destroy);
						return;
					}
				}
				netComp.CurrentEventID++;
				var pickup = NetItemPickup.Make(EntityID, netComp.CurrentEventID, item.ItemNetID);
				pickup.Iterations = 1;
				netComp.NetEvents.Add(pickup);
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

	public override void Input(GameManager game, int entity)
	{
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		//resources.FreeArmAnimator.SetBool("Dig", input.LeftDown);

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

