using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuGUI : MonoBehaviour {

	// Use this for initialization
	public LocalGame Local;
	public MultiplayerGame Multiplayer;
	public JoinGame Join;
	public CharacterSelection CharacterSelection;
	public GameLobby GameLobby;
	public LeaveLobby leaveLobby;
	public ChangeTeamButton BlueTeam;
	public ChangeTeamButton GreenTeam;
	[HideInInspector]
	public ChangeTeamButton[] Teams;

	public GameObject MultiPlayerSection;
	public InputField IP;
	public InputField Port;
	public InputField Name;

	void Start ()
	{
		Teams = new ChangeTeamButton[2];
		Teams[0] = BlueTeam;
		Teams[1] = GreenTeam;
	}

	// Update is called once per frame
	public void ActivateGameLobby()
	{
		CharacterSelection.gameObject.SetActive(true);
		GameLobby.gameObject.SetActive(true);
	}
	public void DeactivateGameLobby()
	{
		CharacterSelection.gameObject.SetActive(false);
		GameLobby.gameObject.SetActive(false);
	}

	public int SetSlot(int team, string name, string character)
	{
		var charSprite = CharacterSelection.Sprites[character];
		int gotSlot = Teams[team].SetSlot(name, charSprite);
		return gotSlot;
	}
	public void UnsetSlot(int team, int slot)
	{
		Teams[team].UnsetSlot(slot);
	}
	public void UnsetAll()
	{
		for (int i = 0; i < Teams.Length; i++)
		{
			Teams[i].UnsetAllSlot();
		}
			
	}
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
	}
	private void LateUpdate()
	{
		BlueTeam.Clicked = false;
		GreenTeam.Clicked = false;
		Join.Clicked = false;
		Local.Clicked = false;
		leaveLobby.Clicked = false;
	}
}
