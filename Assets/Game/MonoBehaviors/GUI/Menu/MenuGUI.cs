using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuGUI : MonoBehaviour {

	// Use this for initialization
	public LocalGame Local;
	public MultiplayerGame Multiplayer;
	public JoinGame Join;
	public GameObject MultiPlayerSection;
	public InputField IP;
	public InputField Port;
	public InputField Name;
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Multiplayer.Clicked)
		{
			if (MultiPlayerSection.active)
			{
				MultiPlayerSection.SetActive(false);
			}
			else
			{
				MultiPlayerSection.SetActive(true);
			}
			Multiplayer.Clicked = false;
		}
		if (Join.Clicked)
		{
			Join.Clicked = false;
		}
	}
}
