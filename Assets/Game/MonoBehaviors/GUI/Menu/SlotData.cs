using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotData : MonoBehaviour
{

	// Use this for initialization
	public int SlotNumber;
	public bool IsSet;
	public GameObject ClosedSlot;
	public GameObject OpenSlot;
	public Image SlotCharacter;
	public Text ClosedSlotText;
	public GameObject Host;
	//public 

	public void SetSlot(string name, Sprite character)
	{
		ClosedSlotText.text = name;
		SlotCharacter.sprite = character;
		ClosedSlot.SetActive(true);
		OpenSlot.SetActive(false);
		IsSet = true;

	}
	public void UnSet()
	{
		ClosedSlotText.text = "";
		SlotCharacter.sprite = null;
		ClosedSlot.SetActive(false);
		OpenSlot.SetActive(true);
		IsSet = false;
		Host.SetActive(false);
	}
	public void SetHost()
	{
		Host.SetActive(true);
	}
}
