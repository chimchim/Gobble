using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingPanel : MonoBehaviour
{
	public int TemplateAmount;
	public GameObject Template;
	[SerializeField]
	public List<CraftButton> CraftButtons = new List<CraftButton>();
	private int _currentIndex;
	public void SetItems(AllScriptableItems allItems, ScriptableItem.ItemCategory category)
	{
		for (int i = 0; i < _currentIndex; i++)
		{
			CraftButtons[i].gameObject.SetActive(false);
		}
		_currentIndex = 0;
		var items = allItems.GetItems();
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].Category == category)
			{
				CraftButtons[_currentIndex].gameObject.SetActive(true);
				CraftButtons[_currentIndex].SetItem(items[i], allItems.IngredientAmount);
				_currentIndex++;
			}
		}
	}
	void OnEnable()
	{
		for (int i = 0; i < TemplateAmount; i++)
		{
			var go = Instantiate(Template);
			go.transform.parent = transform;
			go.GetComponent<RectTransform>().localScale = Vector3.one;
			CraftButtons.Add(go.GetComponent<CraftButton>());
			go.SetActive(false);
		}
		Destroy(Template);
	}
}
