using Game.Component;
using Game.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Item
{
	public enum ItemID
	{
		Rope,
		Pickaxe,
		Ingredient,
		Ladder,
		Shield
	}
	public ItemID ID;

	public GameObject CurrentGameObject;
	public int ItemNetID;
	public int Quantity = 1;

	public bool Remove;
	public bool GotUpdated;

	public virtual void ClientActivate(Game.GameManager game, int entity)
	{
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		if (!itemHolder.ActiveItems.Contains(this))
		{
			itemHolder.ActiveItems.Add(this);
		}
		CurrentGameObject.SetActive(true);
	}
	public virtual void ClientDeActivate(Game.GameManager game, int entity)
	{
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		if (itemHolder.ActiveItems.Contains(this))
		{
			itemHolder.ActiveItems.Remove(this);
		}
		CurrentGameObject.SetActive(false); 
	}
	public virtual void OwnerActivate(Game.GameManager game, int entity)
	{
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		if (!itemHolder.ActiveItems.Contains(this))
		{
			itemHolder.ActiveItems.Add(this);
		}
		CurrentGameObject.SetActive(true);
	}
	public virtual void OwnerDeActivate(Game.GameManager game, int entity)
	{
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		itemHolder.ActiveItems.Remove(this);
		CurrentGameObject.SetActive(false);
	}


	public abstract void OnPickup(Game.GameManager game, int entity, GameObject gameObject);
	public abstract void Input(Game.GameManager game, int entity);
	public abstract void Sync(Game.GameManager game, Client.GameLogicPacket packet, byte[] byteData, ref int currentIndex);
	public abstract void Serialize(Game.GameManager game, int entity, List<byte> byteArray);
	public abstract void Recycle();

	public virtual void RotateArm(Game.GameManager game, int e)
	{
		var entity = game.Entities.GetEntity(e);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
		var input = game.Entities.GetComponentOf<InputComponent>(e);
		resources.FreeArm.up = -input.ScreenDirection;
		if (entity.Animator.transform.eulerAngles.y > 6)
		{
			resources.FreeArm.up = input.ScreenDirection;
			resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
		}
		if (!CurrentGameObject)
			return;
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
	public virtual bool TryStack(Game.GameManager game, Item item)
	{
		return false;
	}
	public virtual void SetChoosenSlot(Game.GameManager game, int entity)
	{

	}

	public virtual void CheckMain(Game.GameManager game, int entity, ScriptableItem scriptable, GameObject go)
	{
		var inventoryMain = game.Entities.GetComponentOf<InventoryComponent>(entity);
		var holder = game.Entities.GetComponentOf<ItemHolder>(entity);
		var items = inventoryMain.MainInventory.Items;
		int amount = inventoryMain.MainInventory.CurrenItemsAmount;
		int backPackAmount = inventoryMain.InventoryBackpack.CurrenItemsAmount;
		game.WorldItems.Remove(CurrentGameObject.GetComponent<VisibleItem>());
		if (holder.Items.ContainsKey(ItemNetID))
		{
			Debug.Log("Already contains " + ItemNetID);
			return;
		}
		holder.Items.Add(ItemNetID, this);
		
		if (amount < GameUnity.MainInventorySize)
		{
			int index = inventoryMain.MainInventory.SetItemInMain(scriptable, this);
			SetInHand(game, entity, go);
			if (inventoryMain.CurrentItemIndex == index)
			{
				OwnerActivate(game, entity);
				holder.Hands.OwnerDeActivate(game, entity);
			}
			else
			{
				CurrentGameObject.SetActive(false);
			}
		}
		else if (backPackAmount < GameUnity.BackpackInventorySize)
		{
			inventoryMain.InventoryBackpack.SetItemInMain(scriptable, this);
			CurrentGameObject.SetActive(false);
		}
	}

	public virtual bool HasSlot(InventoryComponent inv)
	{
		var hasSlot = (inv.MainInventory.CurrenItemsAmount < GameUnity.MainInventorySize) ||
		(inv.InventoryBackpack.CurrenItemsAmount < GameUnity.BackpackInventorySize);
		return hasSlot;
	}
	public virtual void ThrowItem(Game.GameManager game, int entity)
	{
		var inventoryMain = game.Entities.GetComponentOf<InventoryComponent>(entity);
		var holder = game.Entities.GetComponentOf<ItemHolder>(entity);
		holder.Items.Remove(ItemNetID);
		inventoryMain.MainInventory.RemoveItem(inventoryMain.CurrentItemIndex);
		OwnerDeActivate(game, entity);
		GameObject.Destroy(CurrentGameObject);
		Recycle();

		HandleNetEventSystem.AddEventIgnoreOwner(game, entity, NetDestroyItem.Make(entity, ItemNetID));
	}

	public static void SetInHand(Game.GameManager game, int entity, GameObject item)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		item.transform.parent = resources.Hand.transform;

		item.transform.localEulerAngles = new Vector3(0, 0, -90);
		item.transform.localPosition = new Vector3(0, 0, 0);
	}
}

