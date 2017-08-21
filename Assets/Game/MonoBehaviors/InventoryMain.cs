using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMain : MonoBehaviour {

	// Use this for initialization
	public int InventorySize;
	public Image BackGround;
	public GameObject Template;
	private List<Image> _items = new List<Image>();
	private int _selected;
	void Start ()
	{
		Template.SetActive(true);
		SetMainInventorySlots(InventorySize);
	}

	// Update is called once per frame
	public void SetMainInventorySlots(int slots)
	{
		for (int i = 0; i < slots; i++)
		{
			var go = Instantiate(Template);
			go.transform.parent = transform;
			go.GetComponent<RectTransform>().localScale = Vector3.one;
			_items.Add(go.GetComponent<Image>());
		}
		Template.SetActive(false);
	}
	void Update () {

		for (int i = 49; i < (InventorySize+ 49); i++)
		{
			KeyCode e = (KeyCode)i;
			if (Input.GetKeyDown(e))
			{
				int current = i - 49;
				_items[current].color = Color.red;
				_selected = current;
				ResetOther();
			}
		}
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
