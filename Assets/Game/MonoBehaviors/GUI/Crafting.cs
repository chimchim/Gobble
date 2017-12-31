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
	public ScriptableItem.ItemCategory Current;
	[HideInInspector]
	public CraftButton[] CurrentButton;
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
		CurrentButton = new CraftButton[4];
		Gear();
	}

	public void SetCurrentButton(CraftButton button)
	{
		CurrentButton[(int)Current] = button;
	}
	public void SetCurrent()
	{
		Debug.Log("Set Current Crafting");
		Panel.ResetChoosenItems();
		Panel.SetItems(AllItemsVariables, Current);
		if (CurrentButton[(int)Current])
			CurrentButton[(int)Current].SetCurrent();
	}

	public void Gear()
	{
		Current = ScriptableItem.ItemCategory.Build;
		SetCurrent();
		ResetAll();
		Choosens[0].SetActive(true);
	}
	public void Boots()
	{
		Current = ScriptableItem.ItemCategory.Movement;
		SetCurrent();
		ResetAll();
		Choosens[1].SetActive(true);
	}
	public void Shield()
	{
		Current = ScriptableItem.ItemCategory.Defence;
		SetCurrent();
		ResetAll();
		Choosens[2].SetActive(true);
	}
	public void Sword()
	{
		Current = ScriptableItem.ItemCategory.Weapons;
		SetCurrent();
		ResetAll();
		Choosens[3].SetActive(true);
	}
	public void ResetAll()
	{
		for (int i = 0; i < Choosens.Count; i++)
		{
			Choosens[i].SetActive(false);
		}
	}
}
