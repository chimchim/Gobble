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

	public void SetMatsNeeded(ScriptableItem item)
	{
		for (int i = 0; i < ItemImages.Count; i++)
		{
			ItemImages[i].DisableImages();
		}

		int counter = 0;
		int sign = 1;
		int start = (ItemImages.Count - 1) / 2;
		for (int i = 0; i < item.IngredientsNeeded.Count; i++)
		{
			int index = (start + (counter * sign));
			var recipe = item.IngredientsNeeded[i];
			var sprite = Ingredients.InventorySprite[(int)recipe.Ingredient];
			var currentAmount = AllItems.IngredientAmount[(int)recipe.Ingredient];
			ItemImages[index].EnableImages();
			ItemImages[index].SetImage(sprite);
			ItemImages[index].Chosen.enabled = (currentAmount >= recipe.AmountNeeded);
			ItemImages[index].SetQuantity(recipe.AmountNeeded);
			counter++;
			index *= -1;
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
