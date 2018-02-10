using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour 
{
	public Image ItemImage;
	public Text AmountString;
	public int CurrentAmount;
	public Action<ScriptableItem> SetMaterials;
	public Image Chosen;

	public Crafting Crafting;
	private ScriptableItem Item;
	private int[] Ingredients;
	public bool IsSelected;

	public bool CollectionButton;
	public void SetItem(ScriptableItem item, int[] ingredients)
	{
		Ingredients = ingredients;
		Item = item;
		int amount = -1;
		for (int i = 0; i < item.IngredientsNeeded.Count; i++)
		{
			var recipePart = item.IngredientsNeeded[i];
			int currentAmount = ingredients[(int)recipePart.Ingredient] / recipePart.AmountNeeded;
			if (amount == -1)
				amount = currentAmount;
			if (currentAmount < amount)
				amount = currentAmount;
		}
		if(amount > 0)
			AmountString.text = amount.ToString();
		else
			AmountString.text = "";
		CurrentAmount = amount;
		ItemImage.sprite = item.Sprite;
	}

	public void SetCurrent()
	{
		Chosen.enabled = true;
		IsSelected = true;
		if (!CollectionButton)
			SetMaterials.Invoke(Item);
		else
		{
			Debug.Log("CollectionClick return");
			OnCollectionClick();
		}
	}
	public void OnClick()
	{
		if (CollectionButton)
			return;
		if (GameUnity.DebugMode)
		{
			Item.MakeItem.Invoke();
			return;
		}
		Crafting.SetCurrentButton(this);
		SetCurrent();
		if (CurrentAmount > 0)
		{
			CurrentAmount--;
			Item.MakeItem.Invoke();
		}
		
	}

	public void OnCollectionClick()
	{
		if (!CollectionButton)
			return;
		Crafting.Panel.DisableCollection();
		Crafting.Panel.SetCollection((ScriptableItemCollection)Item, Ingredients);
	}
}
