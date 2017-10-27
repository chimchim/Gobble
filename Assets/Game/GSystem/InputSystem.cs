using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class InputSystem : ISystem
	{
		// gör en input translator?
		int packetCounter = 0;
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Game.Component.Input, Player, ActionQueue>();
		List<byte> _currentByteArray = new List<byte>();

		public void Update(GameManager game)
		{
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			
			foreach (int entity in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				if (player.Owner)
				{	
					var input = game.Entities.GetComponentOf<Game.Component.Input>(entity);
					var movement = game.Entities.GetComponentOf<Game.Component.Movement>(entity);
					var resources = game.Entities.GetComponentOf<Game.Component.Resources>(entity);
					var entityTransform = game.Entities.GetEntity(entity).gameObject.transform;
					float x = UnityEngine.Input.GetAxis("Horizontal");
					float y = UnityEngine.Input.GetAxis("Vertical");
					Vector2 mousePos = UnityEngine.Input.mousePosition;
					mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

					input.MousePos = mousePos;
					input.Axis = new Vector2(x, y);
					input.Space = UnityEngine.Input.GetKeyDown(KeyCode.Space) || input.Space;
					input.RightClick = UnityEngine.Input.GetKeyDown(KeyCode.Mouse1) || input.RightClick;

					if (game.Client != null)
					{
						_currentByteArray.Clear();
						_currentByteArray.Add((byte)Data.Command.SendToOthers);
						_currentByteArray.AddRange(BitConverter.GetBytes(packetCounter));
						_currentByteArray.AddRange(BitConverter.GetBytes(player.EntityID));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.Axis.x));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.Axis.y));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.Space));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.RightClick));
						_currentByteArray.AddRange(BitConverter.GetBytes(movement.Grounded));
						_currentByteArray.AddRange(BitConverter.GetBytes(entityTransform.position.x));
						_currentByteArray.AddRange(BitConverter.GetBytes(entityTransform.position.y));
						_currentByteArray.AddRange(BitConverter.GetBytes(((int)movement.CurrentState)));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.MousePos.x));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.MousePos.y));

						bool ropeConnected = input.RopeConnected.Length > 0;
						_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected));
						if (ropeConnected)
						{
							CreateRopeConnected(_currentByteArray, input.RopeConnected);
							input.RopeConnected.Length = 0;
						}

						var byteData = _currentByteArray.ToArray();
						game.Client.SendInput(player.EntityID, byteData);

						packetCounter++;
						for (int i = 0; i < game.Client._byteDataBuffer.Count; i++)
						{
							var byteDataRecieve = game.Client._byteDataBuffer[i];
							if ((Data.Command)byteDataRecieve[0] == Data.Command.SendToOthers)
							{
								var gameLogic = Client.CreateGameLogic(byteDataRecieve);
								input.GameLogicPackets.Add(gameLogic);
								if (gameLogic.RopeConnected.Length > 0)
								{
									var otherMovement = game.Entities.GetComponentOf<Game.Component.Movement>(gameLogic.PlayerID);
									var otherTransform = game.Entities.GetEntity(gameLogic.PlayerID).gameObject.transform;
									otherMovement.CurrentState = Game.Component.Movement.MoveState.Roped;
									otherTransform.transform.position = gameLogic.RopeConnected.Position;
									Debug.Log("Recieve .RopeConnected " + gameLogic.Position.x);
									otherMovement.CurrentRoped = new Game.Component.Movement.RopedData()
									{
										RayCastOrigin = gameLogic.RopeConnected.RayCastOrigin,
										origin = gameLogic.RopeConnected.Origin,
										Length = gameLogic.RopeConnected.Length,
										Damp = GameUnity.RopeDamping
									};
								}
							}
						}
					}
					if (input.RightClick && movement.CurrentState != Component.Movement.MoveState.Roped)
					{
						resources.GraphicRope.ThrowRope(game, entity, movement, input);
					}
					else if (input.RightClick && movement.CurrentState == Component.Movement.MoveState.Roped)
					{
						input.RightClick = false;
						resources.GraphicRope.DeActivate();
						movement.RopeList.Clear();
						movement.RopeIndex = 0;
						movement.CurrentState = Component.Movement.MoveState.Grounded;
					}
				}
			}
		}
		private void CreateRopeConnected(List<byte> currentByteArray, Game.Component.Input.NetworkRopeConnected ropeConnected)
		{
			_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.RayCastOrigin.x));
			_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.RayCastOrigin.y));
			_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Origin.x));
			_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Origin.y));
			_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Position.x));
			_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Position.y));
			_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Length));
		}

		public void Initiate(GameManager game)
		{

		}
        public void SendMessage(GameManager game, int reciever, Message message)
        {

        }
	}
}
