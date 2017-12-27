using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Crafting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public List<GameObject> Choosens = new List<GameObject>();

	public bool Hovering;
	public void OnPointerExit(PointerEventData eventData)
	{
		Hovering = false;
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		Hovering = true;
	}
	public void Gear()
	{
		ResetAll();
		Choosens[0].SetActive(true);
	}
	public void Boots()
	{
		ResetAll();
		Choosens[1].SetActive(true);
	}
	public void Shield()
	{
		ResetAll();
		Choosens[2].SetActive(true);
	}
	public void Sword()
	{
		ResetAll();
		Choosens[3].SetActive(true);
	}
	public void ResetAll()
	{
		for (int i = 0; i < Choosens.Count; i++)
		{
			Choosens[i].SetActive(false);
		}
	}
}
