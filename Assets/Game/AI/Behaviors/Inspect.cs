using System;
using System.Collections.Generic;
using System.Collections;
using Game;
using Game.Component;
using UnityEngine;
using System.Linq;
using Game.Actions;
using Game.Misc;
namespace Game.AI.Behaviors
{
	public class Inspect : AgentBehaviour
	{
		public float HeightCheck = 0.01f;

		private float _checkedHeight = 0;
		private int sensorDirection = 1;
		private Transform _eyes;

		private int _inspectID;
		private int _hitCounter;
        private Vector3 _lastPlayerPosition;
		public override void EnterState(GameManager game)
		{
            var head = game.Entities.GetComponentOf<GameObjects>(EntityID).Head;
            _eyes = head.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "Eyes");
			//_rayDirection = new Vector2(1, 0);
		}

		public override void ExecuteState(GameManager game)
		{

            bool _seesPlayer = false;
            var gameObjects = game.Entities.GetComponentOf<GameObjects>(EntityID);
			int xDir = -Math.Sign(gameObjects.Head.transform.right.z);

			var inspectTransform = game.Entities.GetComponentOf<GameObjects>(_inspectID).Head.transform;
			var cc = game.Entities.GetComponentOf<GameObjects>(_inspectID).Head.GetComponent<CharacterController>();

			Vector3 eyePos = new Vector3((_eyes.position.x + (xDir * 0.5f)), _eyes.position.y, _eyes.position.z);
			Vector3 rayPosition = new Vector3(eyePos.x, eyePos.y, eyePos.z);
			Vector3 inspectPosition = new Vector3(inspectTransform.position.x, 
													(inspectTransform.position.y + _checkedHeight), 
													inspectTransform.position.z);
			Vector3 rayDirection = inspectPosition - rayPosition;

			_checkedHeight += HeightCheck * sensorDirection;
			if ((_checkedHeight > (cc.height)) || _checkedHeight < 0)
			{
				sensorDirection *= -1;
			}

			

			for (int i = 0; i < 1; i++)
			{
				RaycastHit[] hited3D = Physics.RaycastAll(rayPosition, rayDirection).OrderBy(h => h.distance).ToArray();
				for (int j = 0; j < hited3D.Length; j++)
				{
					
					if (hited3D[j].transform.tag == gameObjects.Head.tag)
					{
						continue;
					}
					if (hited3D[j].transform.tag == "Player")
					{
						Debug.DrawRay(rayPosition, rayDirection, Color.red);

						Vector2 GunForward = new Vector2(gameObjects.Aim.transform.forward.x, gameObjects.Aim.transform.forward.y);
						Vector3 Target = new Vector2(
							(hited3D[j].point - gameObjects.Shooter.transform.position).x,
							(hited3D[j].point.y - gameObjects.Shooter.transform.position.y - 0.09f) + ((hited3D[j].point.z-gameObjects.Shooter.transform.position.z) * (19.2f / 90f)));

						float angle = (180 - Vector2.Angle(GunForward, Target));
						Vector3 cross = Vector3.Cross(new Vector3(GunForward.x, GunForward.y, 0), new Vector3(Target.x, Target.y, 0));
						
						if (cross.z > 0)
							angle = -angle;


						if (_hitCounter % 30 == 0)
						{
							var aq = game.Entities.GetComponentOf<ActionQueue>(EntityID);
                            game.SendMessage(EntityID, ShootPoint.Make(hited3D[j].point));
                            //aq.Actions.Add(ShootAction.Make());
                        }
						_hitCounter++;
                        _seesPlayer = true;
                        _lastPlayerPosition = hited3D[j].transform.position;
                    }
					if (hited3D[j].transform.tag == "Crate")
					{
						return;
					}
					break;
				}
			}
            if (!_seesPlayer)
            {
                game.SendMessage(EntityID, MissPlayer.Make(_lastPlayerPosition));
            }
		}
		public override void ReceiveMessage(GameManager game, Message msg)
		{
			if (msg is FoundPlayer)
			{
				var foundPlayer = (FoundPlayer)msg;
				_inspectID = foundPlayer.ID;
                
                //Debug.Log("YEA");
            }
		}
	}
}
