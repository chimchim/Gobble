﻿using System;
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
	public ScriptableItem Item;
	
	public void SetItem(ScriptableItem item, int[] ingredients)
	{
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
	public void OnClick()
	{
		SetMaterials.Invoke(Item);
		if (CurrentAmount > 0)
		{
			CurrentAmount--;
			Item.MakeItem.Invoke();
		}
	}
}
