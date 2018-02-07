using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChooseCharacter : MonoBehaviour, IPointerClickHandler
{
	public bool Clicked;
	public Characters Character;
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
