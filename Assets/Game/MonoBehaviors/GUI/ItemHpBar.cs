using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHpBar : MonoBehaviour
{
	float startWidhtX;

	void Start ()
	{
		startWidhtX = GetComponent<RectTransform>().rect.width;
	}

	public void DisableImage()
	{
		var image = GetComponent<UnityEngine.UI.Image>();
		image.enabled = false;
	}
	public void SetHp(float p)
	{
		if (p <= 0)
		{
			GetComponent<UnityEngine.UI.Image>().enabled = false;
			return;
		}
		var image = GetComponent<UnityEngine.UI.Image>();
		if (!image.isActiveAndEnabled)
			image.enabled = true;


		float scale = GetComponent<RectTransform>().rect.width;
		var pos = GetComponent<RectTransform>().localPosition;
		float newWidht = startWidhtX * p;
		scale = newWidht;
		pos.x = (-startWidhtX / 2) * (1- p);
		GetComponent<RectTransform>().localPosition = pos;
		GetComponent<RectTransform>().localScale = new Vector3(p, 1, 1);

	}
}
