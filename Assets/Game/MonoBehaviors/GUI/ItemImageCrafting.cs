using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemImageCrafting : MonoBehaviour
{

	public Image Chosen;
	public Image Image;
	public Text Quantity;
	public bool IsSet;

	public ScriptableItem.Recipe Recipe;
	public void SetQuantity(int i)
	{
		if (i > 1)
			Quantity.text = i.ToString();
		else
		{
			Quantity.text = "";
		}
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
	public void DisableImages()
	{
		IsSet = false;
		GetComponent<Image>().enabled = false;
		Image.enabled = false;
		Quantity.text = "";
	}
	public void EnableImages()
	{
		IsSet = true;
		GetComponent<Image>().enabled = true;
		Image.enabled = true;
	}
}
