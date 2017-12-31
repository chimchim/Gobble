using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingPanel : MonoBehaviour
{
	public CraftingMatsNeeded MatsNeededPanel;
	public int TemplateAmount;
	public GameObject Template;
	[HideInInspector]
	public List<CraftButton> CraftButtons = new List<CraftButton>();

	private int _currentIndex;
	private bool enabled;

	public void SetItems(AllScriptableItems allItems, ScriptableItem.ItemCategory category)
	{
		for (int i = 0; i < _currentIndex; i++)
		{
			CraftButtons[i].gameObject.SetActive(false);
		}
		_currentIndex = 0;
		for (int i = 0; i < allItems.AllItemsList.Count; i++)
		{
			if (allItems.AllItemsList[i].Category == category)
			{
				CraftButtons[_currentIndex].gameObject.SetActive(true);
				CraftButtons[_currentIndex].SetItem(allItems.AllItemsList[i], allItems.IngredientAmount);
				MatsNeededPanel.SetMatsAmount();
				_currentIndex++;
			}
		}
		
	}
	public void SetSelected()
	{
		for (int i = 0; i < CraftButtons.Count; i++)
		{
			CraftButtons[i].Chosen.enabled = false;
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
			CraftButtons.Add(go.GetComponent<CraftButton>());
			go.GetComponent<CraftButton>().SetMaterials = (Scriptable) =>
			{
				MatsNeededPanel.SetMatsNeeded(Scriptable);
			};
			go.SetActive(false);
		}
		Destroy(Template);
	}
}
