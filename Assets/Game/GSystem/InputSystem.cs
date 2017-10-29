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
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<InputComponent, Player, ActionQueue>();

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
						for (int i = 0; i < game.Client._byteDataBuffer.Count; i++)
						{
							var byteDataRecieve = game.Client._byteDataBuffer[i];
							if ((Data.Command)byteDataRecieve[0] == Data.Command.SendToOthers)
							{
								var gameLogic = Client.CreateGameLogic(byteDataRecieve);
								input.GameLogicPackets.Add(gameLogic);
								if (gameLogic.RopeConnected.Length > 0)
								{
									var otherMovement = game.Entities.GetComponentOf<MovementComponent>(gameLogic.PlayerID);
									var otherTransform = game.Entities.GetEntity(gameLogic.PlayerID).gameObject.transform;
									otherMovement.CurrentState = MovementComponent.MoveState.Roped;
									otherTransform.transform.position = gameLogic.RopeConnected.Position;
									otherMovement.CurrentRoped = new MovementComponent.RopedData()
									{
										RayCastOrigin = gameLogic.RopeConnected.RayCastOrigin,
										origin = gameLogic.RopeConnected.Origin,
										Length = gameLogic.RopeConnected.Length,
										Damp = GameUnity.RopeDamping
									};
									otherMovement.RopeList.Add(otherMovement.CurrentRoped);
								}
							}
						}
					}
					if (input.RightClick && movement.CurrentState != MovementComponent.MoveState.Roped)
					{
						resources.GraphicRope.ThrowRope(game, entity, movement, input);
					}
					else if (input.RightClick && movement.CurrentState == MovementComponent.MoveState.Roped && !input.NetworkRopeKill)
					{
						input.NetworkRopeKill = true;
						input.RightClick = false;
						resources.GraphicRope.DeActivate();
						movement.RopeList.Clear();
						movement.RopeIndex = 0;
						movement.CurrentState = MovementComponent.MoveState.Grounded;
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
