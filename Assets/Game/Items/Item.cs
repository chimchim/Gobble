using Game.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Item
{
	public enum ItemID
	{
		ItemCreator,
		Rope,
		Pickaxe
	}
	public ItemID ID;

	public GameObject CurrentGameObject;
	public bool Active;
	public Sprite Sprite;
	public int ItemNetID;
	public int Quantity;

	public abstract void OnPickup(Game.GameManager game, int entity, GameObject gameObject);
	public abstract void Input(Game.GameManager game, int entity);
	public abstract void Sync(Game.GameManager game, Client.GameLogicPacket packet, byte[] byteData, ref int currentIndex);
	public abstract void Serialize(Game.GameManager game, int entity, List<byte> byteArray);
	public abstract void Recycle();

	public virtual void CreateItemInSlot(Game.GameManager game, int entity, Sprite sprite)
	{

	}

	public virtual void CheckMain(Game.GameManager game, int entity, ScriptableItem scriptable, GameObject go)
	{
		var inventoryMain = game.Entities.GetComponentOf<InventoryComponent>(entity);
		var holder = game.Entities.GetComponentOf<ItemHolder>(entity);
		var items = inventoryMain.MainInventory.Items;
		int amount = inventoryMain.MainInventory.CurrenItemsAmount;
		holder.Items.Add(this);
		if (amount < GameUnity.MainInventorySize)
		{
			int index = inventoryMain.MainInventory.SetItemInMain(scriptable, this);
			SetInHand(game, entity, go);
			if (inventoryMain.CurrentItemIndex == index)
			{
				Active = true;
			}
			else
			{
				CurrentGameObject.SetActive(false);
			}
		}
		else
		{
			CurrentGameObject.SetActive(false);
		}
	} 

	public virtual void ThrowItem(Game.GameManager game, int entity)
	{
		Debug.Log("ThrowItem");
		var inventoryMain = game.Entities.GetComponentOf<InventoryComponent>(entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var visibleItem = CurrentGameObject.GetComponent<VisibleItem>();
		visibleItem.enabled = true;
		visibleItem.CallBack = (EntityID) =>
		{
			OnPickup(game, EntityID, CurrentGameObject);
			//SetInHand(game, EntityID, CurrentGameObject);
		};
		var force = input.ArmDirection * 5;
		visibleItem.Force = force;
		Active = false;
		CurrentGameObject.transform.parent = null;
		CurrentGameObject.transform.position += new Vector3(input.ArmDirection.x, input.ArmDirection.y, 0)*2;
		inventoryMain.MainInventory.RemoveItem(inventoryMain.CurrentItemIndex);
	}

	public virtual void SetActive()
	{
		Active = true;
		CurrentGameObject.SetActive(true);
	}

	public static void SetInHand(Game.GameManager game, int entity, GameObject item)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		item.transform.parent = resources.Hand.transform;
		item.transform.localPosition = new Vector3(3, -1, 0);
		item.transform.localEulerAngles = new Vector3(0, 0, -90);
		item.transform.localPosition = new Vector3(3, -1, 0);
	}
}

