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
	public GameObject CurrentGameObject;
	public bool Active;
	public ScriptableItem Scriptable;

	public abstract void OnPickup(Game.GameManager game, int entity, GameObject gameObject);
	public abstract void Input(Game.GameManager game, int entity);
	public abstract void Sync(Game.GameManager game, Client.GameLogicPacket packet, byte[] byteData, ref int currentIndex);
	public abstract void Serialize(Game.GameManager game, int entity, List<byte> byteArray);
	public abstract void Recycle();

	public virtual void SetInInventory(Game.GameManager game, int entity, ScriptableItem itemscript)
	{
		var inventory = game.Entities.GetComponentOf<InventoryComponent>(entity);
		var items = inventory.MainInventory.Items;

		inventory.MainInventory.SetItemInMain(itemscript, this);
		
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

