using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;
using Game.Misc;
using System;
using System.Text;

namespace Game.Systems
{
	public class MenuSystem : ISystem
	{
		private readonly Bitmask _playerBitmask = Bitmask.MakeFromComponents<Player>();
		private readonly Bitmask _menuBitmask = Bitmask.MakeFromComponents<MenuComponent>();

		MenuComponent _menu;
		public void Update(GameManager game)
		{
			for (int i = 0; i < game.Client._currentByteData.Count; i++)
			{
				byte[] byteData = game.Client._currentByteData[i];
				Data.Command cmd = (Data.Command)byteData[0];
				if (cmd == Data.Command.List)
				{
					CheckList(game, _menu, byteData);
				}
			}
			game.Client._currentByteData.Clear();
			var players = game.Entities.GetEntitiesWithComponents(_playerBitmask);
			foreach (int entity in players)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				if (player.Owner)
				{

				}
			}

			if (_menu.Menu.Local.Clicked)
			{
				_menu.Menu.gameObject.SetActive(false);
				game.Systems.ChangeState(game, SystemManager.GameState.Game);
				game.CreatePlayer(true);
			}
			if (_menu.Menu.Join.Clicked)
			{
				string ip = _menu.Menu.IP.text;
				int port = int.Parse(_menu.Menu.Port.text);
				string name = _menu.Menu.Name.text;
				Debug.Log("MenuSystem: Tryjoin " + ip + " port " + port + " name " + name + " currentid " + IDGiver.GetCurrentID());

				game.Client.TryJoin(ip, port, name);
				game.Client.BeginToRecieve();

			}
			
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
			for (int i = menu.PlayerAmount; i < clientCount; i++)
			{

				int nameLen = BitConverter.ToInt32(byteData, currentByteIndex);
				currentByteIndex += sizeof(int);
				var name = Encoding.UTF8.GetString(byteData, currentByteIndex, nameLen);
				currentByteIndex += nameLen;
				bool isOwner = i == (clientCount - 1) && menu.PlayerAmount == 0;
				bool isHost = clientCount == 1;
				menu.IsHost = isHost;
				game.CreateEmptyPlayer(isOwner, name, isHost);
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