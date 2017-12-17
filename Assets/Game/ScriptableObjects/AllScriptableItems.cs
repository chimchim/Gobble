using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/AllScriptableItems", order = 1)]
public class AllScriptableItems : ScriptableObject
{
	[Header("Items")]
	public PickAxeScriptable PickAxe;
	public RopeScriptable Rope;
	public IngredientScriptable Ingredient;

	[Header("Gatherable ")]
	public GatherableScriptable Gravel;
	public GatherableScriptable Tree;
	public GatherableScriptable TreeTwig;
}