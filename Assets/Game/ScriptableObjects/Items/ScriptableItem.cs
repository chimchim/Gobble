using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/ScriptableItems", order = 1)]
public class ScriptableItem : ScriptableObject
{
	public enum ItemCategory
	{ 
		Build,
		Movement,
		Defence,
		Weapons,
		Ingredient
	}
	public GameObject Prefab;
	public Sprite Sprite;
	public List<Recipe> IngredientsNeeded;
	public ItemCategory Category;
	[Serializable]
	public struct Recipe
	{
		public TileMap.IngredientType Ingredient;
		public int AmountNeeded;
	}
}
