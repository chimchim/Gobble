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
					float x = UnityEngine.Input.GetAxis("Horizontal");
					float y = UnityEngine.Input.GetAxis("Vertical");
					input.Axis = new Vector2(x, y);
					input.Space = UnityEngine.Input.GetKeyDown(KeyCode.Space);
					input.RightClick = UnityEngine.Input.GetKeyDown(KeyCode.Mouse1);

					if (input.RightClick && input.State != Component.Input.MoveState.Roped)
					{
						Vector2 entityPos = game.Entities.GetEntity(entity).gameObject.transform.position;
						Vector2 mousePos = UnityEngine.Input.mousePosition;
						mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
						Debug.Log("MousePos " + mousePos);
						Vector2 direction = mousePos - entityPos;
						Debug.DrawRay(entityPos, direction, Color.red);
						var layerMask = 1 << LayerMask.NameToLayer("Collideable");
						//var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
						RaycastHit2D hit = Physics2D.Raycast(entityPos, direction.normalized, 200, layerMask);
						if (hit.collider != null)
						{
							input.RopePosistion = hit.point;
							input.RopeLength = (entityPos - hit.point).magnitude;
							input.State = Component.Input.MoveState.Roped;
						}
					}
					else if (input.RightClick && input.State == Component.Input.MoveState.Roped)
					{
						input.State = Component.Input.MoveState.Grounded;
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
