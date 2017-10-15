using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeTeamButton : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
	public string Team;
	public bool Clicked;
	public GameObject HoverObject;
	public GameObject Members;
	private bool _activated;

	[HideInInspector]
	private SlotData[] Slots = new SlotData[4];
	void Start()
	{
		var slots = Members.GetComponentsInChildren<SlotData>();
		Slots = slots;
	}

	public int SetSlot(string name, Sprite character)
	{
		int gotSlot = 0;
		for (int i = 0; i < 4; i++)
		{
			var slot = Slots[i];
			slot.SlotNumber = i;
			if (!slot.IsSet)
			{	
				slot.SetSlot(name, character);
				gotSlot = i;
				break;
			}
		}
		return gotSlot;
	}

	public void SetSlotCharacter(int slot, Sprite character)
	{
		Slots[slot].SetSlotCharacter(character);
	}

	public void SetHost(int slot)
	{
		Slots[slot].SetHost();
	}

	public void UnsetSlot(int slot)
	{
		Slots[slot].UnSet();
	}
	public void UnsetAllSlot()
	{
		for (int i = 0; i < 4; i++)
		{
			var slot = Slots[i];
			slot.IsSet = false;
			slot.UnSet();
		}
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
