using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMain : MonoBehaviour {

	public GameObject Template;
	private List<ItemImage> ItemImage = new List<ItemImage>();

	public int CurrentCount
	{
		get
		{
			int count = 0;
			foreach (ItemImage img in ItemImage)
			{
				if (img.Item != null)
					count++;
			}
			return count;
		}
	}

	void Start ()
	{
		Template.SetActive(true);
		SetMainInventorySlots(GameUnity.MainInventorySize);
	}

	public void SetQuantity(Item item)
	{
		for (int i = 0; i < ItemImage.Count; i++)
		{
			if (ItemImage[i].Item == item)
			{
				ItemImage[i].SetQuantity(item.Quantity);
				break;
			}
		}
	}

	public int SetItemInMain(ScriptableItem scriptable, Item item)
	{
		int index = 0;
		for (int i = 0; i < ItemImage.Count; i++)
		{
			if (ItemImage[i].Item == null)
			{
				ItemImage[i].Item = item;
				ItemImage[i].SetImage(scriptable.Sprite);
				ItemImage[i].SetQuantity(item.Quantity);
				if(scriptable.MaxHp > 0)
				{
					item.SetHpInSlot = ItemImage[i].SetHp;
					ItemImage[i].SetHp(item.GetHpPercent);
				}
				index = i;
				break;
			}
		}
		return index;
	}
	// Update is called once per frame
	public Item GetItem(int index)
	{
		return ItemImage[index].Item;
	}

	public void RemoveItem(Item item)
	{
		for (int i = 0; i < ItemImage.Count; i++)
		{
			if (item == ItemImage[i].Item)
			{
				ItemImage[i].Item = null;
				ItemImage[i].UnsetImage();
				ItemImage[i].Quantity.text = "";
				ItemImage[i].HpBar.DisableImage();
				break;
			}
		}
	}

	public void SetMainInventorySlots(int slots)
	{
		for (int i = 0; i < slots; i++)
		{
			var go = Instantiate(Template);
			go.transform.SetParent(transform);
			go.GetComponent<RectTransform>().localScale = Vector3.one;
			go.GetComponent<ItemImage>().Type = Game.Inventory.Main;
			go.GetComponent<ItemImage>().Index = i;
			ItemImage.Add(go.GetComponent<ItemImage>());
		}
		ItemImage[0].Chosen.enabled = true;
		Destroy(Template);
	}
	public void ResetAll()
	{
		for (int i = 0; i < ItemImage.Count; i++)
		{
			ItemImage[i].Chosen.enabled = false;
		}
	}

	public void SetChoosen(int index)
	{
		ItemImage[index].Chosen.enabled = true;
	}
}
