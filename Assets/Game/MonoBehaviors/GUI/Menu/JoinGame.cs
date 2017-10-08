using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoinGame : MonoBehaviour, IPointerClickHandler
{
	public bool Clicked;
	void Start()
	{

	}


	public void OnPointerClick(PointerEventData eventData) // 3
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			Clicked = true;
		}

	}

}
