using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPanel : MonoBehaviour
{
	public CraftingMatsNeeded MatsNeededPanel;
	public int TemplateAmount;
	public GameObject Template;
	[HideInInspector]
	public List<CraftButton> CraftButtons = new List<CraftButton>();
	public CraftingPanel Collection;
	//private int _currentIndex;
	private bool enabled;
	ScriptableItemCollection currentCollection;
	public void SetItems(AllScriptableItems allItems, ScriptableItem.ItemCategory category)
	{
		MatsNeededPanel.Reset();
		for (int i = 0; i < CraftButtons.Count; i++)
		{
			CraftButtons[i].gameObject.SetActive(false);
		}
		//_currentIndex = 0;
		for (int i = 0; i < allItems.AllItemsList.Count; i++)
		{
			if (allItems.AllItemsList[i].Category == category)
			{
				CraftButtons[i].gameObject.SetActive(true);
				if (allItems.AllItemsList[i].GetType() == typeof(ScriptableItemCollection))
				{
					if (!CraftButtons[i].CollectionIsSet)
					{
						CraftButtons[i].CollectionIsSet = true;
						CraftButtons[i].GetComponent<Button>().onClick.RemoveListener(CraftButtons[i].OnClick);
						CraftButtons[i].GetComponent<Button>().onClick.AddListener(CraftButtons[i].OnCollectionClick);
					}
					if (currentCollection == allItems.AllItemsList[i])
					{
						SetCollection(currentCollection, allItems.IngredientAmount);
					}
				}
				CraftButtons[i].SetItem(allItems.AllItemsList[i], allItems.IngredientAmount);
				MatsNeededPanel.SetMatsAmount();

			}
		}
		
	}
	public void DisableCollection()
	{
		for (int i = 0; i < Collection.CraftButtons.Count; i++)
		{
			Collection.CraftButtons[i].gameObject.SetActive(false);
		}
		currentCollection = null;
	}

	public void SetCollection(ScriptableItemCollection collection, int[] ingredientAmount)
	{
		currentCollection = collection;
		for (int i = 0; i < Collection.CraftButtons.Count; i++)
		{
			Collection.CraftButtons[i].gameObject.SetActive(false);
		}

		Debug.Log("collection.Collection L " + collection.Collection.Length);
		for (int i = 0; i < collection.Collection.Length; i++)
		{
			Collection.CraftButtons[i].gameObject.SetActive(true);
			Collection.CraftButtons[i].SetItem(collection.Collection[i], ingredientAmount);
			MatsNeededPanel.SetMatsAmount();
			
		}
	}

	public void ResetChoosenItems()
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
