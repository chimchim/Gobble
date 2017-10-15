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
					player.LobbySlot = monoMenu.SetSlot(player.Team, player.PlayerName, "Yolanda");
				}
				if (player.Owner)
				{
					for (int i = 0; i < monoMenu.Teams.Length; i++)
					{

						if (monoMenu.Teams[i].Clicked)
						{
							game.Client.SendChangeTeam(entity, i);
						}
					}
				}
			}

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
			}
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
				Debug.Log("MenuSystem: Tryjoin " + ip + " port " + port + " name " + name + " currentid " + IDGiver.NextID);

				if (game.Client.epServer == null)
				{
					game.Client.TryJoin(ip, port, name);
					game.Client.BeginToRecieve();

				}
				else
				{
					game.Client = new Client();
					game.Client.TryJoin(ip, port, name);
					game.Client.BeginToRecieve();
				}
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
				Debug.Log("REMOVE entity " + playersIds[i]);
			}
			_menu.Menu.UnsetAll();
			_menu.Menu.DeactivateGameLobby();
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
			
			menu.Menu.SetSlot(team, playerComp.PlayerName, "Yolanda");
		}

		private void CheckLogout(GameManager game, MenuComponent menu, byte[] byteData)
		{
			int leftID = BitConverter.ToInt32(byteData, 1);
			int currentByteIndex = sizeof(int) + 1;
			int currentHostID = BitConverter.ToInt32(byteData, currentByteIndex);

			var currenHostEntity = game.Entities.GetEntity(currentHostID);
			var currenHostPlayer = currenHostEntity.GetComponent<Player>();
			currenHostPlayer.IsHost = true;

			var leftEntity = game.Entities.GetEntity(leftID);
			var leftPlayer = leftEntity.GetComponent<Player>();
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
				if (i >= menu.PlayerAmount)
				{
					Debug.Log("name " + name + " id " + id + " ishost " + isHost + " team " + team);
					bool isOwner = i == (clientCount - 1) && menu.PlayerAmount == 0;
					menu.IsHost = isHost;
					game.CreateEmptyPlayer(isOwner, name, isHost, team, id);
					

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