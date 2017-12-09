using UnityEngine;
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
		bool[,] foundTile;

		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			int fullWidhth = GameUnity.FullWidth;
			int fullHeight = GameUnity.FullHeight;

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
						input.NetworkPosition = gameLogic.Position;
						input.LeftDown = gameLogic.LeftDown;
						// Do Jump
						if (gameLogic.Grounded && gameLogic.InputSpace && !input.NetworkJump)
						{
							input.NetworkJump = true;
						}
						input.Axis = new Vector2(gameLogic.InputAxisX, gameLogic.InputAxisY);

						var itemHolder = game.Entities.GetComponentOf<ItemHolder>(gameLogic.PlayerID);
						int currentIndex = gameLogic.CurrentByteIndex;
						ReadNetEvents(game, gameLogic.PlayerID, byteDataRecieve, ref currentIndex);
						int itemCount = BitConverter.ToInt32(byteDataRecieve, currentIndex);
						currentIndex += sizeof(int);
						for (int j = 0; j < itemCount; j++)
						{
							int itemID = BitConverter.ToInt32(byteDataRecieve, currentIndex);
							currentIndex += sizeof(int);
							itemHolder.ActiveItems[j].Sync(game, gameLogic, byteDataRecieve, ref currentIndex);
						}
					}
				}
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
					Debug.Log("New netevent " + netEvent.GetType() + " Id " + netEventID);
				}

				currentIndex += netEventByteSize;
			}

		}
		public void Initiate(GameManager game)
		{
			
		}



		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}

	}
}