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
					var input = game.Entities.GetComponentOf<InputComponent>(e);
					bool jumped = false;
					//Debug.Log("input.GameLogicPackets.Count " + input.GameLogicPackets.Count);
					for (int i = 0; i < input.GameLogicPackets.Count; i++)
					{
						var pack = input.GameLogicPackets[i];
						int otherPlayerID = pack.PlayerID;
						var otherEntity = game.Entities.GetEntity(otherPlayerID);
						var otherTransform = otherEntity.gameObject.transform;
						var otherPlayerPos = new Vector2(otherTransform.position.x, otherTransform.position.y);
						var otherInput = game.Entities.GetComponentOf<InputComponent>(otherPlayerID);
						var otherResource = game.Entities.GetComponentOf<ResourcesComponent>(otherPlayerID);
						var otherMovement = game.Entities.GetComponentOf<MovementComponent>(otherPlayerID);

						var otherPacketPosition = pack.Position;
						var otherRightClick = pack.RightClick;
						var otherMousePos = pack.MousePos;
						var otherMovestate = (MovementComponent.MoveState)pack.MovementState;
						var diff = otherPacketPosition - new Vector2(otherTransform.position.x, otherTransform.position.y);

						otherInput.MousePos = otherMousePos;
						otherInput.NetworkPosition = otherPacketPosition;
						//Vector2 positionXDiff = otherPlayerPos - otherPacketPosition;
						//if (diff.magnitude > 0.3f)
						//{
						//	Debug.Log("snap ");
						//	otherTransform.position = otherPacketPosition;
						//}
						// Do Jump
						if (pack.Grounded && pack.InputSpace && !jumped)
						{
							jumped = true;
							otherInput.NetworkJump = true;
						}
						// Set MoveAxis
						otherInput.Axis = new Vector2(pack.InputAxisX, pack.InputAxisY);

						if (otherRightClick && otherMovestate != MovementComponent.MoveState.Roped)
						{
							otherResource.GraphicRope.ThrowRope(game, otherPlayerID, otherMovement, otherInput);
						}
						else if (otherRightClick && otherMovestate == MovementComponent.MoveState.Roped)
						{
							otherResource.GraphicRope.DeActivate();
							otherMovement.RopeList.Clear();
							otherMovement.RopeIndex = 0;
							otherMovement.CurrentState = MovementComponent.MoveState.Grounded;
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