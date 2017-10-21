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

		private void SetInput(GameManager game, byte[] byteData)
		{
			int id = BitConverter.ToInt32(byteData, 1);

			var input = game.Entities.GetComponentOf<Game.Component.Input>(id);
			int currentByteIndex = sizeof(int) + 1;

			float xInput = BitConverter.ToSingle(byteData, currentByteIndex);
			
			currentByteIndex += sizeof(float);
			bool spaceInput = BitConverter.ToBoolean(byteData, currentByteIndex);
			currentByteIndex += sizeof(bool);
			int packCounter = BitConverter.ToInt32(byteData, currentByteIndex);
			currentByteIndex += sizeof(bool);
			input.Axis.x = xInput;
			input.Space = spaceInput;
			
		}
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
					input.Axis = new Vector2(x, y);
					if (!input.Space)
					{
						input.Space = UnityEngine.Input.GetKeyDown(KeyCode.Space);
						if (input.Space)
							Debug.Log("fps" + (1 / Time.deltaTime));
					}
					if (!input.RightClick)
					{
						
						input.RightClick = UnityEngine.Input.GetKeyDown(KeyCode.Mouse1);

						if (input.RightClick && movement.CurrentState != Component.Movement.MoveState.Roped)
						{
							resources.GraphicRope.ThrowRope(game, entity, movement);
						}
						else if (input.RightClick && movement.CurrentState == Component.Movement.MoveState.Roped)
						{
							resources.GraphicRope.DeActivate();
							movement.RopeList.Clear();
							movement.RopeIndex = 0;
							movement.CurrentState = Component.Movement.MoveState.Grounded;
						}
					}
					if (game.Client != null)
					{
						_currentByteArray.Clear();
						_currentByteArray.Add((byte)Data.Command.SendToOthers);
						_currentByteArray.AddRange(BitConverter.GetBytes(player.EntityID));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.Axis.x));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.Space));
						_currentByteArray.AddRange(BitConverter.GetBytes(packetCounter));
						packetCounter++;
						var byteData = _currentByteArray.ToArray();
						game.Client.SendInput(player.EntityID, byteData);
						
						for (int i = game.Client._byteDataBuffer.Count-1; i >= 0; i--)
						{
							var byteDataRecieve = game.Client._byteDataBuffer[i];

							int arrayIndex = byteDataRecieve[0];
							if ((Data.Command)byteDataRecieve[0] == Data.Command.SendToOthers)
							{
								SetInput(game, byteDataRecieve);
								game.Client._byteDataBuffer.RemoveAt(i);
							}
							
						}
					}
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
