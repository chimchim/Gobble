using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMain : MonoBehaviour {

	// Use this for initialization
	public int CurrenItemsAmount;
	public GameObject Template;
	public Item[] Items = new Item[GameUnity.MainInventorySize];
	private List<ItemImage> ItemImage = new List<ItemImage>();


	void Start ()
	{
		Template.SetActive(true);
		SetMainInventorySlots(GameUnity.MainInventorySize);
	}

	public void SetQuantity(Item item)
	{
		for (int i = 0; i < Items.Length; i++)
		{
			if (Items[i] == item)
			{
				ItemImage[i].SetQuantity(item.Quantity);
				break;
			}
		}
	}

	public int SetItemInMain(ScriptableItem scriptable, Item item)
	{
		int index = 0;
		for (int i = 0; i < Items.Length; i++)
		{
			if (Items[i] == null)
			{
				CurrenItemsAmount++;
				Items[i] = item;
				ItemImage[i].SetImage(scriptable.Sprite);
				ItemImage[i].SetQuantity(item.Quantity);
				index = i;
				break;
			}
		}
		return index;
	}
	// Update is called once per frame
	public Item GetItem(int index)
	{
		if (index < Items.Length)
		{
			return Items[index];
		}
		return null;
	}

	public void RemoveItem(Item item)
	{
		for (int i = 0; i < Items.Length; i++)
		{
			if (item == Items[i])
			{
				Debug.Log("Remove item at " + i);
				Items[i] = null;
				ItemImage[i].UnsetImage();
				ItemImage[i].Quantity.text = "";
				CurrenItemsAmount--;
				break;
			}
		}
	}

	public void RemoveItem(int index)
	{
		CurrenItemsAmount--;
		Items[index] = null;
		ItemImage[index].UnsetImage();
		ItemImage[index].Quantity.text = "";
	}

	public void SetMainInventorySlots(int slots)
	{
		for (int i = 0; i < slots; i++)
		{
			var go = Instantiate(Template);
			go.transform.parent = transform;
			go.GetComponent<RectTransform>().localScale = Vector3.one;
			ItemImage.Add(go.GetComponent<ItemImage>());
		}
		ItemImage[0].Chosen.enabled = true;
		Destroy(Template);
	}
	public void ResetAll()
	{
		for (int i = 0; i < Items.Length; i++)
		{
			ItemImage[i].Chosen.enabled = false;
		}
	}

	public void SetChoosen(int index)
	{
		ItemImage[index].Chosen.enabled = true;
	}
}
