using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClick : MonoBehaviour, IPointerClickHandler
{
	public bool Clicked;

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
}
