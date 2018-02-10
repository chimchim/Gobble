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

		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);
				var input = game.Entities.GetComponentOf<InputComponent>(e);
				var movement = game.Entities.GetComponentOf<MovementComponent>(e);
				var entity = game.Entities.GetEntity(e);
				var otherTransform = entity.gameObject.transform;
				Vector2 otherPosition = otherTransform.position;
				Vector2 networkPosition = input.NetworkPosition;
				Debug.DrawLine(otherPosition, networkPosition, Color.green);

				Vector2 diff = networkPosition - otherPosition;
				if (!player.Owner && movement.CurrentState != MovementComponent.MoveState.Roped)
				{

					Vector2 translate = diff.normalized * GameUnity.NetworkLerpSpeed * delta;
					translate = translate.magnitude > diff.magnitude ? diff : translate;
					Vector2 translatePos = otherPosition + translate;
					//movement.Body.MovePosition(translatePos);
					otherTransform.position = translatePos;
					Vector2 newPos = otherTransform.position;
					Vector2 newPosDiff = newPos - otherPosition;
					float dot = Vector2.Dot(newPosDiff.normalized, diff.normalized);
					if (dot < 0)
					{
						otherTransform.position = networkPosition;
					}
					if (diff.magnitude > 3)
					{
						otherTransform.position = networkPosition;
					}
				}
			}
		}

		public void Initiate(GameManager game)
		{

		}
	}
}