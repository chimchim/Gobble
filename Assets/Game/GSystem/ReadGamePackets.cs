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
						input.NetworkPosition = gameLogic.Position;

						// Do Jump
						if (gameLogic.Grounded && gameLogic.InputSpace && !input.NetworkJump)
						{
							input.NetworkJump = true;
						}
						input.Axis = new Vector2(gameLogic.InputAxisX, gameLogic.InputAxisY);

						var itemHolder = game.Entities.GetComponentOf<ItemHolder>(gameLogic.PlayerID);
						int currentIndex = gameLogic.CurrentByteIndex;
						for (int j = 0; j < ItemHolder.ActiveItemsCount; j++)
						{
							int itemID = BitConverter.ToInt32(byteDataRecieve, currentIndex); currentIndex += sizeof(int);
							itemHolder.Items[itemID].Sync(game, gameLogic, byteDataRecieve, ref currentIndex);
						}
					}
				}
			}

			//foreach (int e in entities)
			//{
			//	var player = game.Entities.GetComponentOf<Player>(e);
			//
			//	var input = game.Entities.GetComponentOf<InputComponent>(e);
			//
			//	for (int i = 0; i < input.GameLogicPackets.Count; i++)
			//	{
			//		var pack = input.GameLogicPackets[i];
			//		var otherInput = game.Entities.GetComponentOf<InputComponent>(pack.PlayerID);
			//
			//		otherInput.MousePos = pack.MousePos;
			//		otherInput.NetworkPosition = pack.Position;
			//
			//		// Do Jump
			//		if (pack.Grounded && pack.InputSpace && !otherInput.NetworkJump)
			//		{
			//			otherInput.NetworkJump = true;
			//		}
			//		// Set MoveAxis
			//		otherInput.Axis = new Vector2(pack.InputAxisX, pack.InputAxisY);
			//
			//	}
			//	input.GameLogicPackets.Clear();
			//	
			//}
		}

		public void Initiate(GameManager game)
		{
			
		}



		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}

	}
}