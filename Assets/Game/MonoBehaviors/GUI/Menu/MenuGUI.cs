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
	public HostSection HostSection;
	public GameObject MultiPlayerSection;
	public InputField IP;
	public InputField Port;
	public InputField Name;

	public GameObject BigTitle;
	public GameObject SmallTitle;
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
		BigTitle.SetActive(false);
		SmallTitle.SetActive(true);
		MultiPlayerSection.gameObject.SetActive(false);
		Debug.Log("SetActiveTrue FALSE");
	}
	public void DeactivateGameLobby()
	{
		CharacterSelection.gameObject.SetActive(false);
		GameLobby.gameObject.SetActive(false);
		BigTitle.SetActive(true);
		SmallTitle.SetActive(false);
		HostSection.gameObject.SetActive(false);
	}

	public void SetHost(int team, int slot, bool isOwner)
	{
		if(isOwner)
			HostSection.gameObject.SetActive(true);

		Teams[team].SetHost(slot);
	}

	public int SetSlot(int team, string name, Characters character)
	{
		var charSprite = CharacterSelection.Sprites[character];
		int gotSlot = Teams[team].SetSlot(name, charSprite);
		return gotSlot;
	}

	public void SetSlotCharacter(int team, int slot, Characters character)
	{
		var charSprite = CharacterSelection.Sprites[character];
		Teams[team].SetSlotCharacter(slot, charSprite);
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
			MultiPlayerSection.SetActive(true);
			Multiplayer.Clicked = false;
		}
		BlueTeam.Clicked = false;
		GreenTeam.Clicked = false;
		Join.Clicked = false;
		Local.Clicked = false;
		leaveLobby.Clicked = false;
		Multiplayer.Clicked = false;
	}
}
