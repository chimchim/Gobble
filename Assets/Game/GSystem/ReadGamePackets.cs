﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;
using System;

namespace Game.Systems
{
	public class ReadgamePackets : ISystem
	{
		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player, ActionQueue>();
		private readonly Bitmask _animalBitmask = Bitmask.MakeFromComponents<Animal>();

		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);

			if (game.Client != null)
			{
				for (int i = 0; i < game.Client._byteDataBuffer.Count; i++)
				{
					var byteDataRecieve = game.Client._byteDataBuffer[i];
					if ((Data.Command)byteDataRecieve[0] == Data.Command.SendToOthers)
					{
						var gameLogic = Client.CreateGameLogic(byteDataRecieve);
						var input = game.Entities.GetComponentOf<InputComponent>(gameLogic.PlayerID);
						input.MousePos = gameLogic.MousePos;
						input.ArmDirection = gameLogic.ArmDirection;
						input.ScreenDirection = gameLogic.ScreenDirection;
						input.NetworkPosition = gameLogic.Position;
						input.LeftDown = gameLogic.LeftDown;
						input.RightDown = gameLogic.RightDown;
						// Do Jump
						input.Axis = new Vector2(gameLogic.InputAxisX, gameLogic.InputAxisY);

						var itemHolder = game.Entities.GetComponentOf<ItemHolder>(gameLogic.PlayerID);
						int currentIndex = gameLogic.CurrentByteIndex;
						ReadNetEvents(game, gameLogic.PlayerID, byteDataRecieve, ref currentIndex);
						int itemCount = BitConverter.ToInt32(byteDataRecieve, currentIndex);
						currentIndex += sizeof(int);

						for (int j = 0; j < itemCount; j++)
						{
							int itemNetID = BitConverter.ToInt32(byteDataRecieve, currentIndex);
							currentIndex += sizeof(int);
							try
							{
								var item = itemHolder.Items[itemNetID];

								if (!itemHolder.ActiveItems.Contains(item))
								{
									item.ClientActivate(game, gameLogic.PlayerID);
								}
								item.GotUpdated = true;
								item.Sync(game, gameLogic, byteDataRecieve, ref currentIndex);
							}
							catch (Exception e)
							{
								Debug.LogError("EXCEPTION item id " + itemNetID);
								Debug.LogException(e);
							}
							
						}
						for (int k = itemHolder.ActiveItems.Count - 1; k >= 0; k--)
						{
							if (!itemHolder.ActiveItems[k].GotUpdated)
							{
								itemHolder.ActiveItems[k].ClientDeActivate(game, gameLogic.PlayerID);
							}
						}

						foreach (int e in entities)
						{
							var player = game.Entities.GetComponentOf<Player>(e);
							if (!player.Owner || player.IsHost)
								continue;

							ReadAnimals(game, byteDataRecieve, ref currentIndex, delta, player);
						}
					}
				}
			}
		}

		void ReadAnimals(GameManager game, byte[] byteDataRecieve, ref int currentIndex, float delta, Player host)
		{
			var animals = game.Entities.GetEntitiesWithComponents(_animalBitmask);
			foreach (int a in animals)
			{
				var animal = game.Entities.GetComponentOf<Animal>(a);
				int hostStateIndex = BitConverter.ToInt32(byteDataRecieve, currentIndex);
				currentIndex += sizeof(int);
				animal.Health = BitConverter.ToSingle(byteDataRecieve, currentIndex);
				currentIndex += sizeof(float);
				if (hostStateIndex != animal.CurrentState.Index)
				{
					animal.TransitionState(game, game.Entities.GetEntity(a), animal.CurrentState.GetType(), animal.States[hostStateIndex].GetType(), host);
				}
				animal.CurrentState.Deserialize(game, animal, byteDataRecieve, ref currentIndex);
				animal.CurrentState.DeadReckon(game, animal, delta);
			}
		}
		void ReadNetEvents(GameManager game, int entity, byte[] byteData, ref int currentIndex)
		{
			var netComp = game.Entities.GetComponentOf<NetEventComponent>(entity);

			int netEventCount = BitConverter.ToInt32(byteData, currentIndex);
			currentIndex += sizeof(int);

			for (int i = 0; i < netEventCount; i++)
			{
				int netEventID = BitConverter.ToInt32(byteData, currentIndex);
				currentIndex += sizeof(int);
				int netEventTypeID = BitConverter.ToInt32(byteData, currentIndex);
				currentIndex += sizeof(int);
				int netEventByteSize = BitConverter.ToInt32(byteData, currentIndex);
				currentIndex += sizeof(int);

				if (netEventID > netComp.CurrentEventID)
				{
					var netEvent = NetEvent.MakeEmpties[netEventTypeID]();
					netEvent.NetDeserialize(game, byteData, currentIndex);
					netEvent.NetEventID = netEventID;
					netEvent.Handle(game);
					netEvent.Recycle();
					netComp.CurrentEventID = netEventID;
					//Debug.Log("make Event " + netEvent.GetType()  + " netEventID " + netEventID);
				}

				currentIndex += netEventByteSize;
			}

		}
		public void Initiate(GameManager game)
		{
			
		}
	}
}