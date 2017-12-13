using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemImage : MonoBehaviour {

	public Image Chosen;
	public Image Image;
	public Text Quantity;
	public bool IsSet;

	public void SetQuantity(int i)
	{
		if(i > 1)
			Quantity.text = i.ToString();
	}

	public void SetImage(Sprite sprite)
	{
		IsSet = true;
		Image.enabled = true;
		Image.sprite = sprite;
	}
	public void UnsetImage()
	{
		IsSet = false;
		Image.enabled = false;
	}
}
