using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeaveLobby : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
	public bool Clicked;
	public GameObject HoverObject;

	private bool _activated;
	void Start()
	{

	}


	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			Clicked = true;
		}

	}
	public void OnPointerDown(PointerEventData eventData)
	{

		if (!_activated)
		{
			_activated = true;
			HoverObject.SetActive(true);
		}

	}
	public void OnPointerUp(PointerEventData eventData)
	{

		if (_activated)
		{
			_activated = false;
			HoverObject.SetActive(false);
		}

	}
}
