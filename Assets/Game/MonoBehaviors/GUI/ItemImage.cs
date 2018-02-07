using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemImage : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

	public Image Chosen;
	public Image Image;
	public Text Quantity;

	public RealTimeVariables RealTime;
	public Item Item;
	public Inventory Type;
	public int Index;
	public void OnPointerExit(PointerEventData eventData)
	{
		RealTime.CurrentSwitch2 = null;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		RealTime.CurrentSwitch2 = this;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (Image.enabled && eventData.button == PointerEventData.InputButton.Left)
		{
			GameUnity.FollowMouse.transform.position = Input.mousePosition + new Vector3(0, 35f, 0);
			GameUnity.FollowMouse.gameObject.SetActive(true);
			GameUnity.FollowMouse.sprite = Image.sprite;
			RealTime.ChangingItem = true;
			RealTime.CurrentSwitch = this;
		}
	}

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
		Image.enabled = true;
		Image.sprite = sprite;
	}
	public void UnsetImage()
	{
		Image.sprite = null;
		Image.enabled = false;
	}
	public void DisableImages()
	{
		GetComponent<Image>().enabled = false;
		Image.enabled = false;
		Quantity.text = "";
	}
	public void EnableImages()
	{
		GetComponent<Image>().enabled = true;
		Image.enabled = true;
	}

	
}
