using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/AllScriptableItems", order = 1)]
public class AllScriptableItems : ScriptableObject
{
	[Header("Items")]
	public PickAxeScriptable PickAxe;
	public RopeScriptable Rope;
	public IngredientScriptable Ingredient;
	public ScriptableItem Ladder;
	public ScriptableItem Shield;
	public ScriptableItem Sword;
	public ScriptableItem Spear;

	public List<ScriptableItem> AllItemsList = new List<ScriptableItem>();
	public CharacterScriptable[] CharactersScriptables;
	[SerializeField]
	Characters CharacterOrder;
	[Header("Gatherable ")]
	public GatherableScriptable Gravel;
	public GatherableScriptable Tree;
	public GatherableScriptable TreeTwig;

	[HideInInspector]
	public int[] IngredientAmount = new int[7];

	public List<ScriptableItem> GetItems()
	{
		var list = new List<ScriptableItem>();
		list.Add(PickAxe);
		list.Add(Rope);
		list.Add(Ingredient);
		list.Add(Ladder);
		return list;
	}
}