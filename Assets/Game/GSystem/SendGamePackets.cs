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
		private readonly Bitmask _animalBitmask = Bitmask.MakeFromComponents<Animal>();
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
					var netEvents = game.Entities.GetComponentOf<NetEventComponent>(entity);
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
						_currentByteArray.AddRange(BitConverter.GetBytes(input.LeftDown));
						_currentByteArray.AddRange(BitConverter.GetBytes(movement.Grounded));
						_currentByteArray.AddRange(BitConverter.GetBytes(entityTransform.position.x));
						_currentByteArray.AddRange(BitConverter.GetBytes(entityTransform.position.y));
						_currentByteArray.AddRange(BitConverter.GetBytes(((int)movement.CurrentState)));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.MousePos.x));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.MousePos.y));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.ArmDirection.x));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.ArmDirection.y));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.ScreenDirection.x));
						_currentByteArray.AddRange(BitConverter.GetBytes(input.ScreenDirection.y));
						_currentByteArray.AddRange(BitConverter.GetBytes(netEvents.NetEvents.Count));
						//_currentByteArray.AddRange(BitConverter.GetBytes(netEvents.CurrentEventID));
						foreach (NetEvent netevent in netEvents.NetEvents)
						{
							netevent.NetSerialize(game, _currentByteArray);
						}
						_currentByteArray.AddRange(BitConverter.GetBytes(itemHolder.ActiveItems.Count));
						foreach (Item item in itemHolder.ActiveItems)
						{
							item.Serialize(game, player.EntityID, _currentByteArray);
						}
						if (player.IsHost)
						{
							var animals = game.Entities.GetEntitiesWithComponents(_animalBitmask);
							foreach (int a in animals)
							{
								var animal = game.Entities.GetComponentOf<Animal>(a);
								animal.CurrentState.Serialize(game, animal, _currentByteArray);
							}
						}
						if (_currentByteArray.Count > 900)
						{
							Debug.LogError("GETTING BIG " + _currentByteArray.Count);
						}
						var byteData = _currentByteArray.ToArray();
						game.Client.SendInput(player.EntityID, byteData);

						packetCounter++;
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
