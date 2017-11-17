using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;

namespace Game.Systems
{
	public class DeadReckoning : ISystem
	{
		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player, ActionQueue>();
		bool[,] foundTile;

		public float lerpStep = 0.05f;
		public float LerpSpeed;
		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;
			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);
				var input = game.Entities.GetComponentOf<InputComponent>(e);
				var movement = game.Entities.GetComponentOf<MovementComponent>(e);
				var otherTransform = game.Entities.GetEntity(e).gameObject.transform;
				var otherPosition = new Vector2(otherTransform.position.x, otherTransform.position.y);
				var networkPosition = input.NetworkPosition;
				Debug.DrawLine(otherPosition, networkPosition, Color.green);

				Vector2 diff = networkPosition - otherPosition;
				if (!player.Owner && movement.CurrentState != MovementComponent.MoveState.Roped)
				{
					
					Vector2 translate = diff.normalized * GameUnity.NetworkLerpSpeed * delta;
					if (translate.magnitude > diff.magnitude)
					{
						translate = diff;
					}

					float stepX = translate.x;// (diff.normalized * step).x;
					float stepY = translate.y;//(diff.normalized * step).y;

					bool vertGrounded = false;
					bool horGrounded = false;

					Vector3 tempPos = otherTransform.position;
					tempPos = Game.Systems.Movement.VerticalMovement(tempPos, stepY, xOffset, yOffset, out vertGrounded);
					tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, stepX, xOffset, yOffset, out horGrounded);
					bool vertHorGrounded = vertGrounded || horGrounded;

					tempPos = otherTransform.position;
					tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, stepX, xOffset, yOffset, out horGrounded);
					tempPos = Game.Systems.Movement.VerticalMovement(tempPos, stepY, xOffset, yOffset, out vertGrounded);
					bool horVertGrounded = vertGrounded || horGrounded;

					if ((vertHorGrounded && horVertGrounded) || diff.magnitude > 2)
					{
						otherTransform.position = networkPosition;
					}
					else
					{
						otherTransform.position += new Vector3(stepX, stepY, 0);
					}
				}
				if (!player.Owner && movement.CurrentState == MovementComponent.MoveState.Roped)
				{
					//if (diff.magnitude > 1)
					//{
					//	movement.RopeList.Clear();
					//	movement.RopeList.AddRange(input.RopeList);
					//	input.RopeList.Clear();
					//	otherTransform.position = networkPosition;
					//}
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