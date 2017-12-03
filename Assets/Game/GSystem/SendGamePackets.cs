using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class SendGamePackets : ISystem
	{
		// gör en input translator?
		int packetCounter = 0;
		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<InputComponent, Player, ActionQueue>();
		List<byte> _currentByteArray = new List<byte>();

		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);

			foreach (int entity in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				if (player.Owner)
				{
					var input = game.Entities.GetComponentOf<InputComponent>(entity);
					var movement = game.Entities.GetComponentOf<MovementComponent>(entity);
					var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
					var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
					var entityTransform = game.Entities.GetEntity(entity).gameObject.transform;

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
						_currentByteArray.AddRange(BitConverter.GetBytes(input.ArmDirection.x));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.ArmDirection.y));
						foreach (Item item in itemHolder.Items)
						{
							if (!item.Active)
								continue;
							item.Serialize(game, player.EntityID, _currentByteArray);
						}
						//bool ropeConnected = input.RopeConnected.Length > 0;
						//_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected));
						//if (ropeConnected)
						//{
						//	CreateRopeConnected(_currentByteArray, input.RopeConnected);
						//	input.RopeConnected.Length = 0;
						//}
						//if (movement.CurrentState == MovementComponent.MoveState.Roped)
						//{
						//	SyncRope(_currentByteArray, movement);
						//}
						var byteData = _currentByteArray.ToArray();
						game.Client.SendInput(player.EntityID, byteData);

						packetCounter++;
					}
				}
			}
		}
		//private void SyncRope(List<byte> currentByteArray, MovementComponent movement)
		//{
		//	_currentByteArray.AddRange(BitConverter.GetBytes(movement.RopeList.Count));
		//	_currentByteArray.AddRange(BitConverter.GetBytes(movement.CurrentRoped.Vel));
		//	_currentByteArray.AddRange(BitConverter.GetBytes(movement.CurrentRoped.Angle));
		//	for (int i = 0; i < movement.RopeList.Count; i++)
		//	{
		//		var ropeData = movement.RopeList[i];
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.Vel));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.Angle));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.origin.x));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.origin.y));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.RayCastOrigin.x));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.RayCastOrigin.y));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.RayCastCollideOldPos.x));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.RayCastCollideOldPos.y));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.OldRopeCollidePos.x));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.OldRopeCollidePos.y));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.NewRopeIsLeft));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.Length));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.FirstAngle));
		//		_currentByteArray.AddRange(BitConverter.GetBytes(ropeData.Damp));
		//	}
		//}
		//
		//private void CreateRopeConnected(List<byte> currentByteArray, InputComponent.NetworkRopeConnected ropeConnected)
		//{
		//	_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.RayCastOrigin.x));
		//	_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.RayCastOrigin.y));
		//	_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Origin.x));
		//	_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Origin.y));
		//	_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Position.x));
		//	_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Position.y));
		//	_currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Length));
		//}

		public void Initiate(GameManager game)
		{

		}
		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
	}
}
