using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBackpackParent : MonoBehaviour {

	public GameObject BackpackTemplate;
	public int InventorySize;
	public int SlotPerLine;
	void Start ()
	{
		int lines = (InventorySize / SlotPerLine) + 1;
		int restSlots = InventorySize % SlotPerLine;

		BackpackTemplate.SetActive(true);
		for (int i = 0; i < lines; i++)
		{
			var go = Instantiate(BackpackTemplate);
			go.transform.parent = transform;
			go.GetComponent<RectTransform>().localScale = Vector3.one;
			if (i == lines - 1)
			{
				go.GetComponent<InventoryBackpack>().SetMainInventorySlots(restSlots);
			}
			else
			{
				go.GetComponent<InventoryBackpack>().SetMainInventorySlots(SlotPerLine);
			}
		}
		BackpackTemplate.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
