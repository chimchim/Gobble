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

		public float lerpStep = 0.1f;
		public void Update(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;
			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);
				var input = game.Entities.GetComponentOf<InputComponent>(e);

				if (!player.Owner)
				{
					var otherTransform = game.Entities.GetEntity(e).gameObject.transform;
					var otherPosition = new Vector2(otherTransform.position.x, otherTransform.position.y);
					var networkPosition = input.NetworkPosition;

					Vector2 diff = networkPosition - otherPosition;
					float step = Mathf.Min(lerpStep, diff.magnitude);
					if (step < lerpStep)
						continue;
					float stepX = (diff.normalized * step).x;
					float stepY = (diff.normalized * step).y;

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