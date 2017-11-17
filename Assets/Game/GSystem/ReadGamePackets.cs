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
						input.GameLogicPackets.Add(gameLogic);
					}
				}
			}

			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);

				var input = game.Entities.GetComponentOf<InputComponent>(e);

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

					// Do Jump
					if (pack.Grounded && pack.InputSpace && !otherInput.NetworkJump)
					{
						otherInput.NetworkJump = true;
					}
					// Set MoveAxis
					otherInput.Axis = new Vector2(pack.InputAxisX, pack.InputAxisY);

					if (otherRightClick && otherMovestate != MovementComponent.MoveState.Roped)
					{
						otherResource.GraphicRope.ThrowRope(game, otherPlayerID, otherMovement, otherInput);
					}
					else if (otherMovement.CurrentState == MovementComponent.MoveState.Roped && pack.InputSpace)
					{
						otherInput.NetworkJump = true;
					}
					else if (otherMovement.CurrentState == MovementComponent.MoveState.Roped && otherMovestate != MovementComponent.MoveState.Roped)
					{
						otherResource.GraphicRope.DeActivate();
						otherMovement.RopeList.Clear();
						otherMovement.CurrentState = MovementComponent.MoveState.Grounded;
					}

					RopeSync(pack, otherEntity, otherMovement, otherInput);
				}
				input.GameLogicPackets.Clear();
				
			}
		}

		private void RopeSync(Client.GameLogicPacket packet, Entity entity, MovementComponent otherMovement, InputComponent input)
		{
			if (packet.RopeConnected.Length > 0)
			{
				var otherTransform = entity.gameObject.transform;
				otherMovement.CurrentState = MovementComponent.MoveState.Roped;
				otherTransform.transform.position = packet.RopeConnected.Position;
				otherMovement.CurrentRoped = new MovementComponent.RopedData()
				{
					RayCastOrigin = packet.RopeConnected.RayCastOrigin,
					origin = packet.RopeConnected.Origin,
					Length = packet.RopeConnected.Length,
					Damp = GameUnity.RopeDamping
				};
				otherMovement.RopeList.Add(otherMovement.CurrentRoped);
			}

			Vector2 diff = packet.Position - new Vector2(entity.gameObject.transform.position.x, entity.gameObject.transform.position.y);
			
			if (otherMovement.CurrentState == MovementComponent.MoveState.Roped && diff.magnitude > 2)
			{
				entity.gameObject.transform.position = packet.Position;

				otherMovement.RopeList.Clear();
				for (int i = 0; i < packet.RopeList.Length; i++)
				{
					var rope = packet.RopeList[i];
					otherMovement.CurrentRoped = rope;
					
					otherMovement.RopeList.Add(otherMovement.CurrentRoped);
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