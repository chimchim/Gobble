using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChooseCharacter : MonoBehaviour, IPointerClickHandler
{
	public string Name;
	public bool Clicked;

	void Start()
	{
		Name = GetComponent<Image>().sprite.name;
	}


	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			Name = GetComponent<Image>().sprite.name;
			Clicked = true;
		}

	}
}
