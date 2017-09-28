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

        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Game.Component.Input, Player, ActionQueue>();

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

					float x = UnityEngine.Input.GetAxis("Horizontal");
					float y = UnityEngine.Input.GetAxis("Vertical");
					input.Axis = new Vector2(x, y);
					if (!input.Space)
					{
						input.Space = UnityEngine.Input.GetKeyDown(KeyCode.Space);
					}
					if (!input.RightClick)
					{
						
						input.RightClick = UnityEngine.Input.GetKeyDown(KeyCode.Mouse1);


						if (input.RightClick && movement.State != Component.Movement.MoveState.Roped)
						{

							TryRope(game, entity, movement);
						}
						else if (input.RightClick && movement.State == Component.Movement.MoveState.Roped)
						{
							movement.State = Component.Movement.MoveState.Grounded;
						}
					}		
				}
			}
		}

		public static void TryRope(GameManager game, int entity, Component.Movement movement)
		{
			Vector2 entityPos = game.Entities.GetEntity(entity).gameObject.transform.position;
			Vector2 mousePos = UnityEngine.Input.mousePosition;
			mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

			Vector2 direction = mousePos - entityPos;
			Debug.DrawRay(entityPos, direction, Color.red);
			var layerMask = 1 << LayerMask.NameToLayer("Collideable");

			RaycastHit2D hit = Physics2D.Raycast(entityPos, direction.normalized, GameUnity.RopeLength, layerMask);
			if (hit.collider != null)
			{
				float ropeL = (entityPos - hit.point).magnitude;
				movement.State = Component.Movement.MoveState.Roped;

				movement.CurrentRoped = new Component.Movement.Roped()
				{
					origin = hit.point,
					Length = ropeL,
					Damp = GameUnity.RopeDamping
				};
				//game.Entities.GetEntity(entity).gameObject.transform.position = hit.point + new Vector2(0, -input.RopeLength);
				//movement.CurrentRoped.Angle = Mathf.PI / 4f;
				//Len = (transform.position - bob.position).magnitude;
				//float angle2 = Vector2.Angle((transform.position - bob.position).normalized, (-Vector2.up));
				//if (transform.position.x > bob.position.x)
				//{
				//	angle = (Mathf.PI / 180f) * (180 - angle2);
				//}
				//else
				//{
				//	angle = (Mathf.PI / 180f) * -(180 - angle2);
				//}
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
