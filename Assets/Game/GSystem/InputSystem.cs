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

		private Movement _movement;
		private ActionQueue _actionQueue;
		private GameObjects _gameObjects;
		int id;

		private float _divider = 12;
		private float _radius = 2;
		private Vector3 _aimPosition;
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player, ActionQueue>();
        private float _speed = 4.0f;
        private Vector3 _startingPosition;
		public void Update(GameManager game)
		{
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
            
            foreach (int entity in entities)
			{
			UpdateAim();
			
			float inputX = Input.GetAxis("Horizontal");
			float inputZ = Input.GetAxis("Vertical");
			float inputSign = -Math.Sign(inputX);

			if (inputX > 0)
				_gameObjects.Head.transform.eulerAngles = new Vector3(0,  90, 0);
			if(inputX < 0)
				_gameObjects.Head.transform.eulerAngles = new Vector3(0, 270, 0);
			float inputModifyFactor = (inputX != 0.0f && inputZ != 0.0f) ? .7071f : 1.0f;
            var aq = game.Entities.GetComponentOf<ActionQueue>(entity);
			bool isWalking = Input.GetKey(KeyCode.LeftShift);
			if (inputX != 0 || inputZ != 0)
			{
                Vector2 moveVector = new Vector2(inputX * inputModifyFactor, inputZ * inputModifyFactor);
				if (!isWalking)
				{
					moveVector *= Time.deltaTime * _speed;
					MoveAction a = MoveAction.Make(moveVector);
					aq.Actions.Add(a);
				}
				else
				{
					moveVector *= Time.deltaTime * _speed/2;
					WalkAction a = WalkAction.Make(moveVector);
					aq.Actions.Add(a);
				}

			}
			
			if (Input.GetKeyDown(KeyCode.Space))
			{
                JumpAction a = JumpAction.Make(8f);
                aq.Actions.Add(a);
			}
			
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
			    ShootAction a1 = ShootAction.Make();
                aq.Actions.Add(a1);
			
			}

			Vector2 GunForward = new Vector2(_gameObjects.Aim.transform.forward.x, _gameObjects.Aim.transform.forward.y);
			Vector3 targetTemp = _gameObjects.Target.transform.position;
			Vector3 Target = new Vector2(
				(targetTemp - _gameObjects.Aim.transform.position).x,
				(targetTemp - _gameObjects.Aim.transform.position).y);

			float angle = (180 - Vector2.Angle(GunForward, Target));
			Vector3 cross = Vector3.Cross(new Vector3(GunForward.x, GunForward.y, 0), new Vector3(Target.x, Target.y, 0));
			if (cross.z > 0)
			{
				angle = -angle;
			}
			_gameObjects.Shooter.transform.Rotate(Vector3.forward, angle, Space.World);

            }
			
		}

		void UpdateAim()
		{
			float x = Input.GetAxis("Mouse X") / _divider;
			float y = Input.GetAxis("Mouse Y") / _divider;
            var headPosition = _gameObjects.Head.transform.position - _startingPosition;
            _startingPosition = _gameObjects.Head.transform.position;

            var targetpos = _gameObjects.Target.transform.position + new Vector3(x, y, 0) + headPosition;
            var middlepos = _gameObjects.Shooter.transform.position;

            if ((targetpos - middlepos).magnitude > _radius)
            {
                targetpos = _gameObjects.Shooter.transform.position + ((targetpos - _gameObjects.Shooter.transform.position).normalized * _radius);
            }
            _gameObjects.Target.transform.position = targetpos;
 
        }

		private bool CanMove(Vector3 addedPosition)
		{
			Vector3 newPosition = (_gameObjects.Target.transform.position - _gameObjects.Shooter.transform.position) + addedPosition;
			if (newPosition.magnitude > _radius)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
        public void Initiate(GameManager game)
		{
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
            
            foreach (int entity in entities)
			{
				_gameObjects = game.Entities.GetComponentOf<GameObjects>(entity);
				id = entity;
				_movement = game.Entities.GetComponentOf<Movement>(entity);
                _gameObjects.Target.transform.position = _gameObjects.Shooter.transform.position;
                _startingPosition = _gameObjects.Head.transform.position;
                break;
			}
		}
        public void SendMessage(GameManager game, int reciever, Message message)
        {

        }
	}
}
