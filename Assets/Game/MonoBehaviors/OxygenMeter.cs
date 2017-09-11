using Game.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenMeter : MonoBehaviour
{

	// Use this for initialization
	public Scrollbar Meter;
	public GameObject Bubble;
	public Stats PlayerStats;

	private bool _activated;
	void Start()
	{
		Meter.gameObject.SetActive(false);
		Bubble.SetActive(false);
	}

	// Update is called once per frame<Input> 

	void Update()
	{
		float size = (PlayerStats.OxygenSeconds / PlayerStats.MaxOxygenSeconds);
		if (size < 1)
		{
			if (!_activated)
			{
				Meter.gameObject.SetActive(true);
				Bubble.SetActive(true);
				_activated = true;
			}
			Meter.size = size;
		}
		else
		{
			if(_activated)
			{
				Meter.gameObject.SetActive(false);
				Bubble.SetActive(false);
				_activated = false;
			}
		}
	}
}
