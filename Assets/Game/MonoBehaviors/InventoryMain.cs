using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMain : MonoBehaviour {

	// Use this for initialization
	public int InventorySize;
	public GameObject Template;
	public List<Item> Items = new List<Item>();
	public List<ItemImage> ItemImage = new List<ItemImage>();
	private int _selected;
	void Start ()
	{

		Template.SetActive(true);
		SetMainInventorySlots(GameUnity.MainInventorySize);
	}

	public void SetItemInMain(ScriptableItem scriptable, Item item)
	{
		if (Items.Count < GameUnity.MainInventorySize)
		{
			ItemImage[Items.Count].SetImage(scriptable.Sprite);
			Items.Add(item);
		}
	}
	// Update is called once per frame
	public void SetMainInventorySlots(int slots)
	{
		for (int i = 0; i < slots; i++)
		{
			var go = Instantiate(Template);
			go.transform.parent = transform;
			go.GetComponent<RectTransform>().localScale = Vector3.one;
			ItemImage.Add(go.GetComponent<ItemImage>());
		}
		Destroy(Template);
		//Template.SetActive(false);
	}
	void Update()
	{

	}
}
