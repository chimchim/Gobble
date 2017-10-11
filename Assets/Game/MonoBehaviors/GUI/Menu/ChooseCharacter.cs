using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChooseCharacter : MonoBehaviour, IPointerClickHandler
{
	public string Team;
	public bool Clicked;

	void Start()
	{
	}


	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			string name = GetComponent<Image>().sprite.name;
			Clicked = true;
		}

	}
}
