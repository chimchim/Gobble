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
						var otherEntity = game.Entities.GetEntity(otherPlayerID);
						var otherTransform = otherEntity.gameObject.transform;
						var otherInput = game.Entities.GetComponentOf<Game.Component.Input>(otherPlayerID);
						var otherResource = game.Entities.GetComponentOf<Game.Component.Resources>(otherPlayerID);
						var otherMovement = game.Entities.GetComponentOf<Game.Component.Movement>(otherPlayerID);

						var otherPacketPosition = pack.Position;
						var otherRightClick = pack.RightClick;
						var otherMousePos = pack.MousePos;
						var otherMovestate = (Component.Movement.MoveState)pack.MovementState;
						var diff = otherPacketPosition - new Vector2(otherTransform.position.x, otherTransform.position.y);

						otherInput.MousePos = otherMousePos;
						// Snap Position
						if (diff.magnitude > 1)
						{
							otherTransform.position = otherPacketPosition;
						}
						// Do Jump
						if (pack.Grounded && pack.InputSpace && !jumped)
						{
							jumped = true;
							Game.Systems.Movement.DoJump(game, otherPlayerID);
						}
						// Set MoveAxis
						otherInput.Axis = new Vector2(pack.InputAxisX, pack.InputAxisY);

						if (otherRightClick && otherMovestate != Component.Movement.MoveState.Roped)
						{
							otherResource.GraphicRope.ThrowRope(game, otherPlayerID, otherMovement, otherInput);
						}
						else if (otherRightClick && otherMovestate == Component.Movement.MoveState.Roped)
						{
							otherResource.GraphicRope.DeActivate();
							otherMovement.RopeList.Clear();
							otherMovement.RopeIndex = 0;
							otherMovement.CurrentState = Component.Movement.MoveState.Grounded;
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