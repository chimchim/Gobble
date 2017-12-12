using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/ScriptableItems/Ingredient", order = 1)]
public class IngredientScriptable : ScriptableItem
{
	public TileMap.IngredientType Types;
	public Sprite[] IngredientsTypes;
}
