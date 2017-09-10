using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenMeter : MonoBehaviour
{

	// Use this for initialization
	public Scrollbar Meter;
	public GameObject Template;

	public void SetMeter(float hp)
	{
		Template.SetActive(true);
	}

	void Start()
	{
		Template.SetActive(false);
	}

	// Update is called once per frame

	void Update()
	{

	}

}
