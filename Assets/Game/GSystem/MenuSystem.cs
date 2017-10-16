using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;
using Game.Misc;
using System;
using System.Text;
using System.Net.Sockets;

namespace Game.Systems
{
	public class MenuSystem : ISystem
	{
		private readonly Bitmask _playerBitmask = Bitmask.MakeFromComponents<Player>();
		private readonly Bitmask _menuBitmask = Bitmask.MakeFromComponents<MenuComponent>();

		MenuComponent _menu;
		public void Update(GameManager game)
		{
			var monoMenu = _menu.Menu;

			var players = game.Entities.GetEntitiesWithComponents(_playerBitmask);
			foreach (int entity in players)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				if (player.LobbySlot == -1)
				{
					player.LobbySlot = monoMenu.SetSlot(player.Team, entity.ToString(), Characters.Yolanda);
					if (player.IsHost)
					{
						monoMenu.SetHost(player.Team, player.LobbySlot, player.Owner);
					}
				}
				if (player.Owner)
				{
					for (int i = 0; i < monoMenu.Teams.Length; i++)
					{

						if (monoMenu.Teams[i].Clicked && player.Team != i)
						{
							game.Client.SendChangeTeam(entity, i);
						}
					}
					if (player.IsHost)
					{
						if (monoMenu.HostSection.Randomize.Clicked)
						{
							game.Client.SendRandomTeams();

						}
						if (monoMenu.HostSection.StartGame.Clicked)
						{
							game.Client.SendStartGame();

						}
					}
					for (int i = 0; i < monoMenu.CharacterSelection.ChooseCharacters.Length; i++)
					{
						var chooseChar = monoMenu.CharacterSelection.ChooseCharacters[i];
						if (chooseChar.Clicked)
						{
							game.Client.SendChangeCharacter(player.EntityID, chooseChar.Character);
							monoMenu.SetSlotCharacter(player.Team, player.LobbySlot, chooseChar.Character);
							break;
						}
					}
				}		
			}

			#region CheckBytes
			for (int i = 0; i < game.Client._currentByteData.Count; i++)
			{
				byte[] byteData = game.Client._currentByteData[i];
				Data.Command cmd = (Data.Command)byteData[0];
				//Debug.Log("Recieve CMD " + cmd);
				if (cmd == Data.Command.List)
				{
					CheckList(game, _menu, byteData);
				}
				if (cmd == Data.Command.Logout)
				{
					CheckLogout(game, _menu, byteData);
				}
				if (cmd == Data.Command.ChangeTeam)
				{
					CheckChangeTeam(game, _menu, byteData);
				}
				if (cmd == Data.Command.RandomTeam)
				{
					CheckRandomTeams(game, _menu, byteData);
				}
				if (cmd == Data.Command.ChangeChar)
				{
					CheckChangeChar(game, _menu, byteData);
				}
			} 
			#endregion
			game.Client._currentByteData.Clear();
			
			if (monoMenu.leaveLobby.Clicked)
			{
				SendLogout(game);
			}
			if (monoMenu.Local.Clicked)
			{
				monoMenu.gameObject.SetActive(false);
				game.Systems.ChangeState(game, SystemManager.GameState.Game);
				game.CreatePlayer(true);
			}
			if (monoMenu.Join.Clicked)
			{
				string ip = _menu.Menu.IP.text;
				int port = int.Parse(_menu.Menu.Port.text);
				string name = _menu.Menu.Name.text;
				//Debug.Log("MenuSystem: Tryjoin " + ip + " port " + port + " name " + name + " currentid " + IDGiver.NextID);

				game.Client = new Client();
				game.Client.TryJoin(ip, port, name);
				game.Client.BeginToRecieve();
				
			}
		}

		private void SendLogout(GameManager game)
		{
			game.Client.SendLogout();
			_menu.PlayerAmount = 0;
			_menu.IsHost = false;
			var players = game.Entities.GetEntitiesWithComponents(_playerBitmask);
			List<int> playersIds = new List<int>();
			foreach (int entity in players)
			{
				playersIds.Add(entity);
			}
			for (int i = 0; i < playersIds.Count; i++)
			{
				var ent = game.Entities.GetEntity(playersIds[i]);
				game.Entities.RemoveEntity(ent);
			}
			_menu.Menu.UnsetAll();
			_menu.Menu.DeactivateGameLobby();
		}
		private void CheckRandomTeams(GameManager game, MenuComponent menu, byte[] byteData)
		{
			int playerCounts = BitConverter.ToInt32(byteData, 1);
			int currentByteIndex = sizeof(int) + 1;

			menu.Menu.UnsetAll();
			for (int i = 0; i < playerCounts; i++)
			{
				int playerID = BitConverter.ToInt32(byteData, currentByteIndex);
				currentByteIndex += sizeof(int);
				int team = BitConverter.ToInt32(byteData, currentByteIndex);
				currentByteIndex += sizeof(int);
				var player = game.Entities.GetComponentOf<Player>(playerID);
				int newSlot = menu.Menu.SetSlot(team, playerID.ToString(), player.Character);
				player.LobbySlot = newSlot;
				player.Team = team;
				if (player.IsHost)
				{
					menu.Menu.SetHost(team, newSlot, player.Owner);
				}
			}
		}
		private void CheckChangeChar(GameManager game, MenuComponent menu, byte[] byteData)
		{
			int playerID = BitConverter.ToInt32(byteData, 1);
			int currentByteIndex = sizeof(int) + 1;
			Characters character = (Characters)BitConverter.ToInt32(byteData, currentByteIndex);
			var player = game.Entities.GetEntity(playerID);
			var playerComp = player.GetComponent<Player>();
			menu.Menu.SetSlotCharacter(playerComp.Team, playerComp.LobbySlot, character);
			playerComp.Character = character;
			//menu.Menu.UnsetSlot(playerComp.Team, playerComp.LobbySlot);
			//playerComp.Team = team;
			//playerComp.LobbySlot = menu.Menu.SetSlot(team, playerID.ToString(), "Yolanda");
			//if (playerComp.IsHost)
			//{
			//	menu.Menu.SetHost(team, playerComp.LobbySlot, playerComp.Owner);
			//}

		}

		private void CheckChangeTeam(GameManager game, MenuComponent menu, byte[] byteData)
		{
			int playerID = BitConverter.ToInt32(byteData, 1);
			int currentByteIndex = sizeof(int) + 1;
			int team = BitConverter.ToInt32(byteData, currentByteIndex);
			var player = game.Entities.GetEntity(playerID);
			var playerComp = player.GetComponent<Player>();
			menu.Menu.UnsetSlot(playerComp.Team, playerComp.LobbySlot);
			playerComp.Team = team;
			playerComp.LobbySlot = menu.Menu.SetSlot(team, playerID.ToString(), playerComp.Character);
			if (playerComp.IsHost)
			{
				menu.Menu.SetHost(team, playerComp.LobbySlot, playerComp.Owner);
			}
			
		}

		private void CheckLogout(GameManager game, MenuComponent menu, byte[] byteData)
		{
			
			int leftID = BitConverter.ToInt32(byteData, 1);

			int currentByteIndex = sizeof(int) + 1;
			int currentHostID = BitConverter.ToInt32(byteData, currentByteIndex);
			var leftEntity = game.Entities.GetEntity(leftID);
			var leftPlayer = leftEntity.GetComponent<Player>();

			var currenHostEntity = game.Entities.GetEntity(currentHostID);
			var currenHostPlayer = currenHostEntity.GetComponent<Player>();
			currenHostPlayer.IsHost = true;
			if (leftPlayer.IsHost)
			{
				menu.Menu.SetHost(currenHostPlayer.Team, currenHostPlayer.LobbySlot, currenHostPlayer.Owner);
			}
			
			menu.Menu.UnsetSlot(leftPlayer.Team, leftPlayer.LobbySlot);
			Debug.Log("leftPlayer " + leftPlayer.PlayerName + " currenHostPlayer " + currenHostPlayer.PlayerName + " left ID " + leftID);
			game.Entities.RemoveEntity(leftEntity);
			menu.PlayerAmount--;
		}

		private void CheckList(GameManager game, MenuComponent menu, byte[] byteData)
		{
			int clientCount = BitConverter.ToInt32(byteData, 1);
			int currentByteIndex = 1;
			currentByteIndex += sizeof(int);
			if (menu.PlayerAmount == 0)
			{
				menu.Menu.ActivateGameLobby();
			}
			for (int i = 0; i < clientCount; i++)
			{

				int nameLen = BitConverter.ToInt32(byteData, currentByteIndex);
				currentByteIndex += sizeof(int);
				var name = Encoding.UTF8.GetString(byteData, currentByteIndex, nameLen);
				currentByteIndex += nameLen;
				int id = BitConverter.ToInt32(byteData, currentByteIndex);
				currentByteIndex += sizeof(int);
				bool isHost = BitConverter.ToBoolean(byteData, currentByteIndex);
				currentByteIndex += sizeof(bool);
				int team = BitConverter.ToInt32(byteData, currentByteIndex);
				currentByteIndex += sizeof(int);
				int charLen = BitConverter.ToInt32(byteData, currentByteIndex);
				currentByteIndex += sizeof(int);
				var character = Encoding.UTF8.GetString(byteData, currentByteIndex, nameLen);
				currentByteIndex += charLen;
				if (i >= menu.PlayerAmount)
				{
					bool isOwner = i == (clientCount - 1) && menu.PlayerAmount == 0;
					menu.IsHost = isHost;
					game.CreateEmptyPlayer(isOwner, name, isHost, team, Characters.Yolanda, id);
					

				}
			}
			menu.PlayerAmount = clientCount;
		}

		public void Initiate(GameManager game)
		{
			Entity ent = new Entity();
			game.Entities.addEntity(ent);
			var menu = MenuComponent.Make(ent.ID);
			ent.AddComponent(menu);
			menu.Menu = GameObject.FindObjectOfType<MenuGUI>();
			_menu = menu;
			game.Client = new Client();
		}


		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}

	}
}