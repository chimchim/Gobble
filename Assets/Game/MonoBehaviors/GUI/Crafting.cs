using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Crafting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public List<GameObject> Choosens = new List<GameObject>();
	public bool Hovering;
	public CraftingPanel Panel;
	public AllScriptableItems AllItemsVariables;
	public int Current;
	public void OnPointerExit(PointerEventData eventData)
	{
		Hovering = false;
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		Hovering = true;
	}
	void Start()
	{
		Gear();
	}

	public void SetCurrent()
	{
		Panel.SetItems(AllItemsVariables, (ScriptableItem.ItemCategory)(Current));
	}

	public void Gear()
	{
		Panel.SetItems(AllItemsVariables, ScriptableItem.ItemCategory.Build);
		ResetAll();
		Choosens[0].SetActive(true);
		Current = 0;
	}
	public void Boots()
	{
		Panel.SetItems(AllItemsVariables, ScriptableItem.ItemCategory.Movement);
		ResetAll();
		Choosens[1].SetActive(true);
		Current = 1;
	}
	public void Shield()
	{
		Panel.SetItems(AllItemsVariables, ScriptableItem.ItemCategory.Defence);
		ResetAll();
		Choosens[2].SetActive(true);
		Current = 2;
	}
	public void Sword()
	{
		Panel.SetItems(AllItemsVariables, ScriptableItem.ItemCategory.Weapons);
		ResetAll();
		Choosens[3].SetActive(true);
		Current = 3;
	}
	public void ResetAll()
	{
		for (int i = 0; i < Choosens.Count; i++)
		{
			Choosens[i].SetActive(false);
		}
	}
}
