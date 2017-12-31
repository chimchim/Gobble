using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMatsNeeded : MonoBehaviour 
{
	public AllScriptableItems AllItems;
	public IngredientScriptable Ingredients;
	public int TemplateAmount;
	public GameObject Template;
	[HideInInspector]
	public List<ItemImage> ItemImages = new List<ItemImage>();

	private int _currentIndex;
	private bool enabled;

	public void Reset()
	{
		for (int i = 0; i < ItemImages.Count; i++)
		{
			ItemImages[i].DisableImages();
		}
	}
	public void SetMatsAmount()
	{
		for (int i = 0; i < ItemImages.Count; i++)
		{
			if (ItemImages[i].IsSet)
			{
				var currentAmount = AllItems.IngredientAmount[(int)ItemImages[i].Recipe.Ingredient];
				ItemImages[i].Chosen.enabled = (currentAmount >= ItemImages[i].Recipe.AmountNeeded);
			}
		}
	}

	public void SetMatsNeeded(ScriptableItem item)
	{
		Debug.Log("SET MATS NEEDE " + item.WhatItem);
		Reset();
		for (int i = 0; i < ItemImages.Count; i++)
		{
			ItemImages[i].DisableImages();
		}

		float counter = 0;
		int sign = 1;
		int start = (ItemImages.Count - 1) / 2;
		for (int i = 0; i < item.IngredientsNeeded.Count; i++)
		{
			int index = (int)(start + (counter * sign));
			var recipe = item.IngredientsNeeded[i];
			var sprite = Ingredients.InventorySprite[(int)recipe.Ingredient];
			var currentAmount = AllItems.IngredientAmount[(int)recipe.Ingredient];
			ItemImages[index].Recipe = recipe;
			ItemImages[index].EnableImages();
			ItemImages[index].SetImage(sprite);
			ItemImages[index].Chosen.enabled = (currentAmount >= recipe.AmountNeeded);
			ItemImages[index].SetQuantity(recipe.AmountNeeded);
			counter += 0.5f;
			sign *= -1;
		}
	}
	void OnEnable()
	{
		if (enabled)
			return;
		enabled = true;
		for (int i = 0; i < TemplateAmount; i++)
		{
			var go = Instantiate(Template);
			go.transform.parent = transform;
			go.GetComponent<RectTransform>().localScale = Vector3.one;
			ItemImages.Add(go.GetComponent<ItemImage>());
			go.GetComponent<ItemImage>().DisableImages();
		}
		Destroy(Template);
	}
}
