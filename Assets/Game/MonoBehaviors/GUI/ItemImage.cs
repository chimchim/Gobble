using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemImage : MonoBehaviour {

	public Image Image;
	public bool IsSet;
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
