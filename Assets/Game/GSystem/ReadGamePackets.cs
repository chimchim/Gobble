using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;

namespace Game.Systems
{
	public class ReadgamePackets : ISystem
	{
		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player, ActionQueue>();
		bool[,] foundTile;

		public void Update(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			int fullWidhth = GameUnity.FullWidth;
			int fullHeight = GameUnity.FullHeight;

			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);
				if (player.Owner)
				{
					var input = game.Entities.GetComponentOf<Game.Component.Input>(e);
					bool jumped = false;
					//Debug.Log("input.GameLogicPackets.Count " + input.GameLogicPackets.Count);
					for (int i = 0; i < input.GameLogicPackets.Count; i++)
					{
						var pack = input.GameLogicPackets[i];
						int otherPlayerID = pack.PlayerID;
						var otherInput = game.Entities.GetComponentOf<Game.Component.Input>(otherPlayerID);
						otherInput.Axis = new Vector2(pack.InputAxisX, pack.InputAxisY);

						if (pack.Grounded && pack.InputSpace && !jumped)
						{
							jumped = true;
							Game.Systems.Movement.DoJump(game, otherPlayerID);
						}
					}
					input.GameLogicPackets.Clear();
				}
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