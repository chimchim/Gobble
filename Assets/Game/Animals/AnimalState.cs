using Game;
using Game.Component;
using Game.GEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Movement
{
	public abstract class AnimalState
	{
		public int Index;
		public abstract void EnterState(GameManager game, Animal animal, Entity entity, Player host);
		public abstract void Update(GameManager game, Animal animal, Entity entity, Player host, float delta);
		public abstract void LeaveState(GameManager game, Animal animal, Entity entity, Player host);

		public AnimalState(int index)
		{
			Index = index;
		}
		public Vector3 ClosestPlayer(GameManager game, Vector3 myPos)
		{
			var entities = game.Entities.GetEntitiesWithComponents(Bitmask.MakeFromComponents<InputComponent>());
			Vector3 closest = new Vector3(100000, 10000, 0);
			foreach (int e in entities)
			{
				var playerPos = game.Entities.GetEntity(e).gameObject.transform.position;
				if ((playerPos - myPos).magnitude < (closest - myPos).magnitude)
				{
					closest = playerPos;
				}
			}
			return closest;
		}
		public abstract void InnerSerialize(Game.GameManager game, Animal animal, List<byte> byteArray);
		public abstract void InnerDeSerialize(Game.GameManager game, Animal animal, byte[] byteData, ref int index);
		public void Serialize(Game.GameManager game, Animal animal, List<byte> byteArray)
		{
			var transform = game.Entities.GetEntity(animal.EntityID).gameObject.transform;
			byteArray.AddRange(BitConverter.GetBytes(animal.CurrentState.Index));
			byteArray.AddRange(BitConverter.GetBytes(animal.Dead));
			byteArray.AddRange(BitConverter.GetBytes(transform.position.x));
			byteArray.AddRange(BitConverter.GetBytes(transform.position.y));
			byteArray.AddRange(BitConverter.GetBytes(animal.CurrentVelocity.x));
			byteArray.AddRange(BitConverter.GetBytes(animal.CurrentVelocity.y));
			InnerSerialize(game, animal, byteArray);
		}
		public void Deserialize(Game.GameManager game, Animal animal, byte[] byteData, ref int index)
		{
			var posX = BitConverter.ToSingle(byteData, index); index += sizeof(float);
			var posY = BitConverter.ToSingle(byteData, index); index += sizeof(float);
			var velX = BitConverter.ToSingle(byteData, index); index += sizeof(float);
			var velY = BitConverter.ToSingle(byteData, index); index += sizeof(float);

			animal.HostPosition = new Vector2(posX, posY);
			animal.HostVelocity = new Vector2(velX, velY);
			InnerDeSerialize(game, animal, byteData, ref index);
		}
		public void DeadReckon(GameManager game, Animal animal, float delta)
		{
			var transform = game.Entities.GetEntity(animal.EntityID).gameObject.transform;
			Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
			Vector2 diff = animal.HostPosition - currentPos;
			float speed = GameUnity.NetworkLerpSpeed + (GameUnity.NetworkLerpSpeed * (diff.magnitude / GameUnity.NetworkLerpSpeed));
			Vector2 translate = diff.normalized * GameUnity.NetworkLerpSpeed * delta;
			if (translate.magnitude > diff.magnitude)
			{
				translate = diff;
			}

			float stepX = translate.x;// (diff.normalized * step).x;
			float stepY = translate.y;//(diff.normalized * step).y;
			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;
			bool vertGrounded = false;
			bool horGrounded = false;

			Vector3 tempPos = transform.position;
			int layer = (int)Systems.Movement.LayerMaskEnum.Grounded;
			MappedMasks masks = game.LayerMasks.MappedMasks[layer];
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, stepY, xOffset, yOffset, masks, out vertGrounded);
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, stepX, xOffset, yOffset, out horGrounded);
			bool vertHorGrounded = vertGrounded || horGrounded;

			tempPos = transform.position;
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, stepX, xOffset, yOffset, out horGrounded);
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, stepY, xOffset, yOffset, masks, out vertGrounded);
			bool horVertGrounded = vertGrounded || horGrounded;

			if (diff.magnitude > 4)
			{
				transform.position = animal.HostPosition;
			}
			if ((vertHorGrounded || horVertGrounded) && diff.magnitude > 1)
			{
				transform.position = animal.HostPosition;
			}
			else
			{
				transform.position += new Vector3(stepX, stepY, 0);
			}
		}
	}
}
