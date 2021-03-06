﻿using Game;
using Game.Component;
using Game.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Item
{
	public interface IOnMouseRight{ void OnMouseRight(GameManager game, int entity); }
	public interface IOnMouseLeft { void OnMouseLeft(GameManager game, int entity); }
	public enum ItemID
	{
		Rope,
		Pickaxe,
		Ingredient,
		Ladder,
		Shield,
		Sword,
		Spear,
		Base
	}
	public ItemID ID;
	public ScriptableItem ScrItem;
	public GameObject CurrentGameObject;
	public int ItemNetID;
	public int Quantity = 1;
	public float Health;
	public float GetHpPercent
	{
		get
		{
			return (ScrItem.MaxHp <= 0) ? 0 : (Health / ScrItem.MaxHp);
		}
	}

	public bool Remove;
	public bool GotUpdated;
	
	public virtual void ClientActivate(Game.GameManager game, int entity)
	{
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		var stats = game.Entities.GetComponentOf<Stats>(entity);
		stats.CharacterStats.ArmRotationSpeed = ScrItem.RotationSpeed;
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
		var inv = game.Entities.GetComponentOf<InventoryComponent>(entity);
		
		var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
		var stats = game.Entities.GetComponentOf<Stats>(entity);
		stats.CharacterStats.ArmRotationSpeed = ScrItem.RotationSpeed;
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
	public abstract void Input(Game.GameManager game, int entity, float delta);
	public abstract void Sync(Game.GameManager game, Client.GameLogicPacket packet, byte[] byteData, ref int currentIndex);
	public abstract void Serialize(Game.GameManager game, int entity, List<byte> byteArray);
	public abstract void Recycle();

	public virtual void RotateArm(Game.GameManager game, int e)
	{
		//return;
		//var entity = game.Entities.GetEntity(e);
		//var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
		//var input = game.Entities.GetComponentOf<InputComponent>(e);
		//resources.FreeArm.up = -input.ScreenDirection;
		//if (entity.Animator.transform.eulerAngles.y > 6)
		//{
		//	resources.FreeArm.up = input.ScreenDirection;
		//	resources.FreeArm.eulerAngles = new Vector3(resources.FreeArm.eulerAngles.x, resources.FreeArm.eulerAngles.y, 180 - resources.FreeArm.eulerAngles.z);
		//}
		//if (!CurrentGameObject)
		//	return;
		//float rotDir = Math.Sign((resources.FreeArm.up.x * resources.FacingDirection));
		//var eu = CurrentGameObject.transform.localEulerAngles;
		//if (resources.FacingDirection > 0)
		//{
		//	CurrentGameObject.transform.localEulerAngles = (rotDir > 0) ? new Vector3(eu.x, 180, eu.z) : new Vector3(eu.x, 0, eu.z);
		//}
		//else
		//{
		//	CurrentGameObject.transform.localEulerAngles = (rotDir > 0) ? new Vector3(eu.x, 0, eu.z) : new Vector3(eu.x, 180, eu.z);
		//}
	}
	public virtual bool TryStack(Game.GameManager game, Item item)
	{
		return false;
	}
	public virtual void SetChoosenSlot(Game.GameManager game, int entity)
	{

	}

	public virtual void CheckMain(Game.GameManager game, int entity, GameObject go)
	{
		var inventoryMain = game.Entities.GetComponentOf<InventoryComponent>(entity);
		var holder = game.Entities.GetComponentOf<ItemHolder>(entity);
		int amount = inventoryMain.MainInventory.CurrentCount;
		int backPackAmount = inventoryMain.InventoryBackpack.CurrentCount;
		game.WorldItems.Remove(CurrentGameObject.GetComponent<VisibleItem>());
		if (holder.Items.ContainsKey(ItemNetID))
		{
			Debug.Log("Already contains " + ItemNetID);
			return;
		}
		holder.Items.Add(ItemNetID, this);
		
		if (amount < GameUnity.MainInventorySize)
		{
			int index = inventoryMain.MainInventory.SetItemInMain(ScrItem, this);
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
			SetInHand(game, entity, go);
			inventoryMain.InventoryBackpack.SetItemInMain(ScrItem, this);
			CurrentGameObject.SetActive(false);
		}
	}

	public virtual bool HasSlot(InventoryComponent inv)
	{
		var hasSlot = (inv.MainInventory.CurrentCount < GameUnity.MainInventorySize) ||
		(inv.InventoryBackpack.CurrentCount < GameUnity.BackpackInventorySize);
		return hasSlot;
	}
	public abstract void ThrowItem(GameManager game, int entity);
	public virtual void DestroyItem(GameManager game, int entity)
	{
		var inv = game.Entities.GetComponentOf<InventoryComponent>(entity);
		var holder = game.Entities.GetComponentOf<ItemHolder>(entity);
		holder.Items.Remove(ItemNetID);
		inv.MainInventory.RemoveItem(this);
		inv.InventoryBackpack.RemoveItem(this);
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

	public Action<Item> SetHpInSlot;
	public void DoDamage(GameManager game, float dmg, int owner)
	{
		Health -= dmg;
		if (Health <= 0)
		{
			var go = GameObject.Instantiate(game.GameResources.Prefabs.Poof);
			go.transform.position = CurrentGameObject.transform.position;
			GameObject.Destroy(go, 0.4f);
			DestroyItem(game, owner);
			
		}
		SetHpInSlot.Invoke(this);
	}
}

