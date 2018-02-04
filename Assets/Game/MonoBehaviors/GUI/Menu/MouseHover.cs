using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public RealTimeVariables RealTime;

	public void OnPointerExit(PointerEventData eventData)
	{
		RealTime.HoveringUI = false;
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		RealTime.HoveringUI = true;
	}
}
