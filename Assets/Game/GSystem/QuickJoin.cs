﻿using UnityEngine;
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
	public class QuickJoin : ISystem
	{
		private readonly Bitmask _playerBitmask = Bitmask.MakeFromComponents<Player>();

		MenuComponent _menu;
		public void Update(GameManager game, float delta)
		{
			var players = game.Entities.GetEntitiesWithComponents(_playerBitmask);
			foreach (int entity in players)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);

			}
			for (int i = game.Client._byteDataBuffer.Count - 1; i >= 0; i--)
			{

				byte[] byteData = game.Client._byteDataBuffer[i];

				Data.Command cmd = (Data.Command)byteData[0];
				
				if (cmd == Data.Command.List)
				{
					Debug.Log("recieve List ");
					CheckList(game, _menu, byteData);
					game.Client._byteDataBuffer.RemoveAt(i);
					continue;
				}
				if (cmd == Data.Command.StartGame)
				{
					CheckStartGame(game, _menu, byteData);
					game.Client._byteDataBuffer.RemoveAt(i);
					continue;
				}
				

			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.G))
			{
				game.Client.SendStartGame();
			}

		}

		private void CheckStartGame(GameManager game, MenuComponent menu, byte[] byteData)
		{
			Debug.Log("game.Systems.CurrentGameState " + game.Systems.CurrentGameState);
			if (game.Systems.CurrentGameState == SystemManager.GameState.Game)
				return;
			int randomSeed = BitConverter.ToInt32(byteData, 1);
			game.CurrentRandom = new System.Random(randomSeed);
			Debug.Log("START GAME randomSeed " + randomSeed);
			game.Systems.ChangeState(game, SystemManager.GameState.Game);
		}

		private void SendLogout(GameManager game)
		{
			game.Client.SendLogout();
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

		}


		private void CheckList(GameManager game, MenuComponent menu, byte[] byteData)
		{
			int clientCount = BitConverter.ToInt32(byteData, 1);
			int currentByteIndex = 1;
			currentByteIndex += sizeof(int);

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
					if (game.Systems.CurrentGameState == SystemManager.GameState.QuickJoin)
						game.CreateEmptyPlayer(isOwner, name, isHost, team, Characters.Yolanda, id);
					else
					{
						game.CreateFullPlayer(isOwner, name, isHost, team, Characters.Yolanda, id);
					}


				}
			}
			menu.PlayerAmount = clientCount;
		}

		private bool _initiated;
		public void Initiate(GameManager game)
		{
			if (_initiated)
			{
				return;
			}

			_initiated = true;
			Entity ent = new Entity();
			game.Entities.addEntity(ent);
			var menu = MenuComponent.Make(ent.ID);
			ent.AddComponent(menu);
			_menu = menu;

			string ip = "193.11.162.110";
			int port = 1112;
			string name = "name";

			game.Client = new Client();
			game.Client.TryJoin(ip, port, name);
			game.Client.BeginToRecieve();
		}


		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}

	}
}