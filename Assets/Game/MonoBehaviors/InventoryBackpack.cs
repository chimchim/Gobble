using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBackpack : MonoBehaviour
{

	// Use this for initialization
	public int InventorySize;
	public GameObject Template;
	private List<Image> _items = new List<Image>();
	private int _selected;
	void Start()
	{
		SetMainInventorySlots(InventorySize);
	}

	// Update is called once per frame
	public void SetMainInventorySlots(int slots)
	{
		Template.SetActive(true);
		for (int i = 0; i < slots; i++)
		{
			var go = Instantiate(Template);
			go.transform.parent = transform;
			go.GetComponent<RectTransform>().localScale = Vector3.one;
			_items.Add(go.GetComponent<Image>());
		}
		Template.SetActive(false);
	}
	void Update()
	{

	}

	private void ResetOther()
	{
		for (int i = 0; i < _items.Count; i++)
		{
			if (i == _selected)
				continue;
			_items[i].GetComponent<Image>().color = Color.white;
		}
	}
}
